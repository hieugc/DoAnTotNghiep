package com.example.homex.activity.home.pending

import android.os.Bundle
import android.view.View
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.PendingRequestVPAdapter
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentRequestBinding
import com.google.android.material.tabs.TabLayoutMediator


class RequestFragment : BaseFragment<FragmentRequestBinding>() {
    override val layoutId: Int = R.layout.fragment_request
    private lateinit var viewPagerAdapter: PendingRequestVPAdapter

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showBoxChatLayout = Pair(false, null),
            showMenu = false,
            showMessage = false,
            showBottomNav = false,
            showSearchLayout = false,
            showTitleApp = Pair(true, "Yêu cầu trao đổi")
        )
    }

    override fun setView() {
        viewPagerAdapter = PendingRequestVPAdapter(this)
        binding.viewPager2.adapter = viewPagerAdapter
        TabLayoutMediator(binding.tabLayout, binding.viewPager2) { tab, position ->
            when (position) {
                0 -> tab.text = getString(R.string.status_waiting)
                1 -> tab.text = getString(R.string.status_accepted)
                2 -> tab.text = getString(R.string.status_rejected)
                3 -> tab.text = getString(R.string.status_reviewing)
                4 -> tab.text = getString(R.string.status_done)
            }

        }.attach()
    }
}