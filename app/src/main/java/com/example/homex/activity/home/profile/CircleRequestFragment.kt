package com.example.homex.activity.home.profile

import android.os.Bundle
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.CircleRequestViewPager
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentCircleRequestBinding
import com.google.android.material.tabs.TabLayoutMediator

class CircleRequestFragment : BaseFragment<FragmentCircleRequestBinding>() {
    override val layoutId: Int = R.layout.fragment_circle_request
    private lateinit var viewPagerAdapter: CircleRequestViewPager

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showMenu = false,
            showMessage = false,
            showTitleApp = Pair(true, getString(R.string.circle_request)),
            showBottomNav = false,
            showBoxChatLayout = Pair(false, null),
        )

    }

    override fun setView() {
        viewPagerAdapter = CircleRequestViewPager(this)
        binding.pager.adapter = viewPagerAdapter
        TabLayoutMediator(binding.tabLayout, binding.pager) { tab, position ->
            when (position) {
                0 -> tab.text = getString(R.string.new_request)
                1 -> tab.text = getString(R.string.status_accepted)
                2 -> tab.text = getString(R.string.status_checkin)
                3 -> tab.text = getString(R.string.status_reviewing)
                4 -> tab.text = getString(R.string.status_done)
            }

        }.attach()
    }
}