package com.example.homex.activity.home.search

import android.os.Bundle
import android.view.View
import androidx.core.os.bundleOf
import androidx.fragment.app.viewModels
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.RecentSearchAdapter
import com.example.homex.app.CITY
import com.example.homex.app.DISTRICT
import com.example.homex.app.END_DATE
import com.example.homex.app.LOCATION
import com.example.homex.app.PEOPLE
import com.example.homex.app.START_DATE
import com.example.homex.app.SUGGEST
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentSearchBinding
import com.example.homex.extension.disable
import com.example.homex.extension.enable
import com.example.homex.extension.gone
import com.example.homex.extension.longToFormat
import com.example.homex.extension.visible
import com.homex.core.CoreApplication
import com.homex.core.model.CalendarDate
import com.homex.core.model.Location
import com.homex.core.model.LocationSuggestion
import com.homex.core.util.PrefUtil
import org.koin.android.ext.android.inject


class SearchFragment : BaseFragment<FragmentSearchBinding>() {
    override val layoutId: Int = R.layout.fragment_search
    private lateinit var adapter: RecentSearchAdapter
    private val viewModel: SearchViewModel by viewModels()
    private val prefUtil: PrefUtil by inject()
    private val searchList = arrayListOf<LocationSuggestion>()


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
            viewModel.startDate.postValue(dates.first)
            viewModel.endDate.postValue(dates.second)
        }

        findNavController().currentBackStackEntry?.savedStateHandle?.getLiveData<Int>("NOP")?.observe(viewLifecycleOwner){
            ppl->
            viewModel.people.postValue(ppl)
            binding.numberOfPeopleTV.text = getString(R.string.people, ppl)
        }

        findNavController().currentBackStackEntry?.savedStateHandle?.getLiveData<LocationSuggestion>(SUGGEST)?.observe(viewLifecycleOwner){
            suggest->
            if (suggest.districtName == null){
                val loc = if (suggest.cityName == null) "" else suggest.cityName
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
        searchList.clear()
        prefUtil.listSearch?.let {
            searchList.addAll(it)
        }
        if (searchList.isEmpty())
            binding.recentSearchTV.gone()
        else
            binding.recentSearchTV.visible()
        adapter = RecentSearchAdapter(
            searchList,
            recentSearch = true,
            onClick =
            { suggest->
                findNavController().currentBackStackEntry?.savedStateHandle?.set(SUGGEST, suggest)
            },
            deleteOnClick = {
                searchList.removeAt(it)
                adapter.notifyItemRemoved(it)
                CoreApplication.instance.saveListSearch(searchList)
                if (searchList.isEmpty())
                    binding.recentSearchTV.gone()
                else
                    binding.recentSearchTV.visible()
            }
        )
        binding.recentSearchRecView.adapter = adapter
        val layoutManager = LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, true)
        binding.recentSearchRecView.layoutManager = layoutManager
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
            val people = viewModel.people.value?:1
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
            if (searchList.isEmpty())
                binding.recentSearchTV.gone()
            else
                binding.recentSearchTV.visible()
        }
        binding.pickDateLayout.setOnClickListener {
            val numberOfPeople = viewModel.people.value?:1
            val startDate = viewModel.startDate.value
            val endDate = viewModel.endDate.value
            val action = SearchFragmentDirections.actionSearchFragmentToBottomSheetChangeDateFragment(numberOfPeople = numberOfPeople, startDate = startDate, endDate = endDate)
            findNavController().navigate(action)
        }
    }

    override fun setViewModel() {
        viewModel.idCity.observe(this){
            if (it == null || it == 0){
                binding.btnSearch.disable()
            }else{
                binding.btnSearch.enable()
            }
        }
    }
}