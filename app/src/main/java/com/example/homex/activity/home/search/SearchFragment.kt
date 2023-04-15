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
import com.example.homex.extension.betweenDays
import com.example.homex.extension.longToDate
import com.example.homex.extension.longToFormat
import com.example.homex.viewmodel.YourHomeViewModel
import com.homex.core.model.BingLocation
import com.homex.core.model.CalendarDate
import org.koin.androidx.viewmodel.ext.android.viewModel
import java.util.*


class SearchFragment : BaseFragment<FragmentSearchBinding>() {
    override val layoutId: Int = R.layout.fragment_search
    private lateinit var adapter: RecentSearchAdapter
    private val viewModel: SearchViewModel by viewModels()
    private val locationViewModel: YourHomeViewModel by viewModel()

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

        locationViewModel.getCity()

        binding.viewModel = viewModel

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
    }

    override fun setView() {
        cityAdapter = LocationAdapter(requireContext(), R.layout.sex_item, cityList)
        binding.cityTV.setAdapter(cityAdapter)

        districtAdapter = LocationAdapter(requireContext(), R.layout.sex_item, districtList)
        binding.districtTV.setAdapter(districtAdapter)

        adapter = RecentSearchAdapter(
            arrayListOf(
                "Hồ Chí Minh",
                "Hà nội",
                "Nhà của Hiếu",
                "Nhà của Nhật",
                "Nhà của Nhật",
                "Nhà của Nhật",
                "Nhà của Nhật",
                "Nhà của Nhật",
                "Nhà của Nhật"
            )
        )
        binding.recentSearchRecView.adapter = adapter
        val layoutManager = LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.recentSearchRecView.layoutManager = layoutManager

        val cal = Calendar.getInstance()
        val first = CalendarDate(cal.time, cal.get(Calendar.DAY_OF_MONTH).toString())
        Log.e("first", cal.get(Calendar.DAY_OF_MONTH).toString())
        cal.add(Calendar.DATE, 7)
        val second = CalendarDate(cal.time, cal.get(Calendar.DAY_OF_MONTH).toString())
        Log.e("second", cal.get(Calendar.DAY_OF_MONTH).toString())
        viewModel.startDate.postValue(first)
        viewModel.endDate.postValue(second)
//        selection = Pair(
//            first, second
//        )
//        val from = first.time?.time?.longToDate()
//        val to = second.time?.time?.longToDate()
//        binding.fromDateTV.text = from
//        binding.toDateTV.text = to
    }

    override fun setEvent() {
        binding.iconMapPin.setOnClickListener {
            Log.e("hello", "click")
        }
        binding.btnSearch.setOnClickListener {
            val idCity = viewModel.idCity.value
            if (idCity == 0)
                return@setOnClickListener
            val idDistrict = viewModel.idDistrict.value
            val people = viewModel.people.value
            val startDate = viewModel.startDate.value?.time?.time?.longToFormat("yyyy-MM-dd")
            val endDate = viewModel.endDate.value?.time?.time?.longToFormat("yyyy-MM-dd")

            Log.e("cittyName", "${binding.cityTV.text}")
            Log.e("cittyName", "${binding.districtTV.text}")
            Log.e("district", "$idDistrict")
            val cityName = binding.cityTV.text.toString()
            val districtName = binding.districtTV.text.toString()

            findNavController().navigate(R.id.action_searchFragment_to_searchResultFragment, bundleOf(
                CITY to idCity,
                DISTRICT to idDistrict,
                PEOPLE to people,
                START_DATE to startDate,
                END_DATE to endDate,
                CITY_NAME to cityName,
                DISTRICT_NAME to districtName
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
    }
}