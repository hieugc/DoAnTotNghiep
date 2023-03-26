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


class SearchFragment : BaseFragment<FragmentSearchBinding>() {
    override val layoutId: Int = R.layout.fragment_search
    private lateinit var adapter: RecentSearchAdapter

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
    }

    override fun setEvent() {
        binding.iconMapPin.setOnClickListener {
            Log.e("hello", "click")
        }
        binding.btnSearch.setOnClickListener {
            findNavController().navigate(R.id.action_searchFragment_to_searchResultFragment)
        }
    }
}