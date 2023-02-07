package com.example.homex

import android.os.Bundle
import android.util.Log
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.navigation.fragment.findNavController
import com.example.homex.activity.home.HomeActivity
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentSearchBinding


class SearchFragment : BaseFragment<FragmentSearchBinding>() {
    override val layoutId: Int = R.layout.fragment_search

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showMessage = false,
            showBottomNav = false,
            showMenu = false,
            showTitleApp = Pair(true, "Tìm kiếm"),
            showBoxChatLayout = Pair(false, "")
        )
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