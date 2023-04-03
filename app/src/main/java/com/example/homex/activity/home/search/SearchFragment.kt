package com.example.homex.activity.home.search

import android.os.Bundle
import android.util.Log
import android.view.View
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.RecentSearchAdapter
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentSearchBinding
import com.example.homex.extension.betweenDays
import com.example.homex.extension.longToDate
import com.homex.core.model.CalendarDate
import java.util.*


class SearchFragment : BaseFragment<FragmentSearchBinding>() {
    override val layoutId: Int = R.layout.fragment_search
    private lateinit var adapter: RecentSearchAdapter
    private var selection: Pair<CalendarDate?, CalendarDate?> = Pair(null, null)
    private var numberOfPeople = 1

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
        findNavController().currentBackStackEntry?.savedStateHandle?.getLiveData<Pair<CalendarDate?, CalendarDate?>>("DATE")?.observe(viewLifecycleOwner){
                dates->
            val startDate = dates.first?.time?.time?.longToDate()
            val endDate = dates.second?.time?.time?.longToDate()
            binding.fromDateTV.text =  startDate
            binding.toDateTV.text = endDate
            selection = dates
        }

        findNavController().currentBackStackEntry?.savedStateHandle?.getLiveData<Int>("NOP")?.observe(viewLifecycleOwner){
            ppl->
            numberOfPeople = ppl
            binding.numberOfPeopleTV.text = "$ppl người"
        }
    }

    override fun setView() {
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
        selection = Pair(
            first, second
        )
        val from = first.time?.time?.longToDate()
        val to = second.time?.time?.longToDate()
        binding.fromDateTV.text = from
        binding.toDateTV.text = to
    }

    override fun setEvent() {
        binding.iconMapPin.setOnClickListener {
            Log.e("hello", "click")
        }
        binding.btnSearch.setOnClickListener {
            findNavController().navigate(R.id.action_searchFragment_to_searchResultFragment)
        }
        binding.pickDateLayout.setOnClickListener {
            val action = SearchFragmentDirections.actionSearchFragmentToBottomSheetChangeDateFragment(numberOfPeople = numberOfPeople, startDate = selection.first, endDate = selection.second)
            findNavController().navigate(action)
        }
    }
}