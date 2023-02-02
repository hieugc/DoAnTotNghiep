package com.example.homex.activity.home

import android.os.Bundle
import android.view.View
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.LinearSnapHelper
import com.example.homex.R
import com.example.homex.adapter.PopularLocationAdapter
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentHomeBinding
import com.example.homex.utils.CenterZoomLayoutManager


class HomeFragment : BaseFragment<FragmentHomeBinding>() {
    override val layoutId: Int = R.layout.fragment_home
    private lateinit var adapter: PopularLocationAdapter

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = true,
            showMenu = false,
            showMessage = true,
            showTitleApp = Pair(false, ""),
            showBottomNav = true,
            showBoxChatLayout = Pair(false, ""),
        )
    }

    override fun setView() {
        adapter =
            PopularLocationAdapter(arrayListOf("London", "Paris", "New York", "Brooklyn"))
        binding.popularLocationRecView.adapter = adapter
        binding.popularLocationRecView.layoutManager = CenterZoomLayoutManager(context, LinearLayoutManager.HORIZONTAL, false, mShrinkAmount = 0f, mShrinkDistance = 1f)
        binding.popularHomeRecView.adapter = adapter
        binding.popularHomeRecView.layoutManager = CenterZoomLayoutManager(context, LinearLayoutManager.HORIZONTAL, false, mShrinkAmount = 0.05f, mShrinkDistance = 0.8f)
        //Add snap helper to recyclerview to make it focus at 1 item
        val snapHelper = LinearSnapHelper()
        snapHelper.attachToRecyclerView(binding.popularHomeRecView)
        val snapHelper2 = LinearSnapHelper()
        snapHelper2.attachToRecyclerView(binding.popularLocationRecView)
    }
}