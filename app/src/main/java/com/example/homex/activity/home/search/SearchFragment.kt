package com.example.homex.activity.home.search

import android.os.Bundle
import android.util.Log
import android.view.View
import android.widget.ArrayAdapter
import androidx.core.os.bundleOf
import androidx.fragment.app.viewModels
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.LocationAdapter
import com.example.homex.adapter.RecentSearchAdapter
import com.example.homex.app.*
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentSearchBinding
import com.example.homex.extension.*
import com.example.homex.viewmodel.YourHomeViewModel
import com.homex.core.CoreApplication
import com.homex.core.model.BingLocation
import com.homex.core.model.CalendarDate
import com.homex.core.model.Location
import com.homex.core.model.LocationSuggestion
import com.homex.core.util.AppEvent
import com.homex.core.util.PrefUtil
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel
import java.util.*
import kotlin.collections.ArrayList


class SearchFragment : BaseFragment<FragmentSearchBinding>() {
    override val layoutId: Int = R.layout.fragment_search
    private lateinit var adapter: RecentSearchAdapter
    private val viewModel: SearchViewModel by viewModels()
    private val locationViewModel: YourHomeViewModel by viewModel()
    private val prefUtil: PrefUtil by inject()
    private val searchList = arrayListOf<LocationSuggestion>()

    private val cityList = arrayListOf<BingLocation>()
    private val districtList = arrayListOf<BingLocation>()

    private lateinit var cityAdapter: ArrayAdapter<BingLocation>
    private lateinit var districtAdapter: ArrayAdapter<BingLocation>


    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showMessage = false,
            showBottomNav = false,
            showMenu = false,
            showTitleApp = Pair(true, "Tìm kiếm"),
            showBoxChatLayout = Pair(false, null),
        )

        //locationViewModel.getCity()

        binding.viewModel = viewModel

        val location = arguments?.getParcelable<Location>(LOCATION)
        if (location != null){
            viewModel.location.postValue(location.name)
            binding.searchEdtTxt.text = location.name
            viewModel.idDistrict.postValue(null)
            viewModel.idCity.postValue(location.id)
            val search = LocationSuggestion(location.id, null, location.name, null)
            viewModel.search.postValue(search)
        }

        findNavController().currentBackStackEntry?.savedStateHandle?.getLiveData<Pair<CalendarDate?, CalendarDate?>>("DATE")?.observe(viewLifecycleOwner){
                dates->
//            val startDate = dates.first?.time?.time?.longToDate()
//            val endDate = dates.second?.time?.time?.longToDate()
//            binding.fromDateTV.text =  startDate
//            binding.toDateTV.text = endDate
            viewModel.startDate.postValue(dates.first)
            viewModel.endDate.postValue(dates.second)
        }

        findNavController().currentBackStackEntry?.savedStateHandle?.getLiveData<Int>("NOP")?.observe(viewLifecycleOwner){
            ppl->
            viewModel.people.postValue(ppl)
            binding.numberOfPeopleTV.text = "$ppl người"
        }

        findNavController().currentBackStackEntry?.savedStateHandle?.getLiveData<LocationSuggestion>(SUGGEST)?.observe(viewLifecycleOwner){
            suggest->
            if (suggest.districtName == null){
                val loc = "${suggest.cityName?:""}"
                viewModel.location.postValue(loc)
                binding.searchEdtTxt.text = loc
                viewModel.idDistrict.postValue(null)
                viewModel.idCity.postValue(suggest.idCity)
            }else{
                val loc = "${suggest.districtName}, ${suggest.cityName?:""}"
                binding.searchEdtTxt.text = loc
                viewModel.location.postValue(loc)
                viewModel.idDistrict.postValue(suggest.idDistrict)
                viewModel.idCity.postValue(suggest.idCity)
            }
            viewModel.search.postValue(suggest)
        }
    }

    override fun setView() {
        cityAdapter = LocationAdapter(requireContext(), R.layout.sex_item, cityList)
        binding.cityTV.setAdapter(cityAdapter)

        districtAdapter = LocationAdapter(requireContext(), R.layout.sex_item, districtList)
        binding.districtTV.setAdapter(districtAdapter)

        searchList.clear()
        prefUtil.listSearch?.let {
            searchList.addAll(it)
        }
        adapter = RecentSearchAdapter(
            searchList,
            recentSearch = true,
            onClick =
            { suggest->
                if (suggest.districtName == null){
                    val loc = "${suggest.cityName?:""}"
                    viewModel.location.postValue(loc)
                    binding.searchEdtTxt.text = loc
                    viewModel.idDistrict.postValue(null)
                    viewModel.idCity.postValue(suggest.idCity)
                }else{
                    val loc = "${suggest.districtName}, ${suggest.cityName?:""}"
                    binding.searchEdtTxt.text = loc
                    viewModel.location.postValue(loc)
                    viewModel.idDistrict.postValue(suggest.idDistrict)
                    viewModel.idCity.postValue(suggest.idCity)
                }
                viewModel.search.postValue(suggest)
            },
            deleteOnClick = {
                searchList.removeAt(it)
                adapter.notifyItemRemoved(it)
                CoreApplication.instance.saveListSearch(searchList)
            }
        )
        binding.recentSearchRecView.adapter = adapter
        val layoutManager = LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, true)
        binding.recentSearchRecView.layoutManager = layoutManager

//        val cal = Calendar.getInstance()
//        val first = CalendarDate(cal.time, cal.get(Calendar.DAY_OF_MONTH).toString())
//        Log.e("first", cal.get(Calendar.DAY_OF_MONTH).toString())
//        cal.add(Calendar.DATE, 7)
//        val second = CalendarDate(cal.time, cal.get(Calendar.DAY_OF_MONTH).toString())
//        Log.e("second", cal.get(Calendar.DAY_OF_MONTH).toString())
//        viewModel.startDate.postValue(first)
//        viewModel.endDate.postValue(second)
//        selection = Pair(
//            first, second
//        )
//        val from = first.time?.time?.longToDate()
//        val to = second.time?.time?.longToDate()
//        binding.fromDateTV.text = from
//        binding.toDateTV.text = to
    }

    override fun setEvent() {
        binding.searchEdtTxt.setOnClickListener {
            findNavController().navigate(R.id.action_searchFragment_to_bottomSheetSearchFragment)
        }
        binding.cardView.setOnClickListener{
            findNavController().navigate(R.id.action_searchFragment_to_bottomSheetSearchFragment)
        }
        binding.btnSearch.setOnClickListener {
            val idCity = viewModel.idCity.value
            if (idCity == 0 || idCity == null){
                return@setOnClickListener
            }
            val idDistrict = viewModel.idDistrict.value
            val people = viewModel.people.value?:2
            val startDate = viewModel.startDate.value?.time?.time?.longToFormat("yyyy-MM-dd")
            val endDate = viewModel.endDate.value?.time?.time?.longToFormat("yyyy-MM-dd")
            val location = viewModel.location.value?:""

            val newSearch = viewModel.search.value
            if (newSearch != null){
                var found = false
                var i : Int? = null
                for ((index, s) in searchList.withIndex()){
                    if (newSearch.idCity == s.idCity && newSearch.idDistrict == s.idDistrict){
                        found = true
                        i = index
                        break
                    }
                }
                if (found && i != null){
                    searchList.removeAt(i)
                }

                if (searchList.size >= 5){
                    searchList.removeAt(0)
                }
                searchList.add(newSearch)
            }
            CoreApplication.instance.saveListSearch(searchList)

            findNavController().navigate(R.id.action_searchFragment_to_searchResultFragment, bundleOf(
                CITY to idCity,
                DISTRICT to idDistrict,
                PEOPLE to people,
                START_DATE to startDate,
                END_DATE to endDate,
                LOCATION to location
            ))
        }
        binding.pickDateLayout.setOnClickListener {
            val numberOfPeople = viewModel.people.value?:2
            val startDate = viewModel.startDate.value
            val endDate = viewModel.endDate.value
            val action = SearchFragmentDirections.actionSearchFragmentToBottomSheetChangeDateFragment(numberOfPeople = numberOfPeople, startDate = startDate, endDate = endDate)
            findNavController().navigate(action)
        }

        binding.cityTV.setOnItemClickListener { parent, view, position, id ->
            val item = cityList[position]
            binding.cityTV.setText(item.name, false)
            binding.districtTV.text.clear()
            districtList.clear()
            viewModel.idCity.postValue(item.id)
            districtAdapter.notifyDataSetChanged()
            item.id?.let {
                locationViewModel.getDistrict(id = it)
            }
        }

        binding.districtTV.setOnItemClickListener { parent, view, position, id ->
            val item = districtList[position]
            binding.districtTV.setText(item.name, false)
            viewModel.idDistrict.postValue(item.id)
        }
    }

    override fun setViewModel() {
        locationViewModel.cityLiveData.observe(this){
            if (it != null){
                cityList.clear()
                cityList.addAll(it)
                cityAdapter.notifyDataSetChanged()
            }
        }

        locationViewModel.districtLiveData.observe(this){
            if (it != null){
                districtList.clear()
                districtList.addAll(it)
                districtAdapter.notifyDataSetChanged()
            }
        }

        viewModel.idCity.observe(this){
            if (it == null || it == 0){
                binding.btnSearch.disable()
            }else{
                binding.btnSearch.enable()
            }
        }
    }
}