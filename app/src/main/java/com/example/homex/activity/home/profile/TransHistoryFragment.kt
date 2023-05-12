package com.example.homex.activity.home.profile

import android.os.Bundle
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.TransHistoryViewPager
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentTransHistoryBinding
import com.example.homex.viewmodel.RequestViewModel
import com.google.android.material.tabs.TabLayoutMediator
import org.koin.androidx.viewmodel.ext.android.sharedViewModel

class TransHistoryFragment : BaseFragment<FragmentTransHistoryBinding>() {
    override val layoutId: Int = R.layout.fragment_trans_history
    private lateinit var viewPagerAdapter: TransHistoryViewPager
    private val viewModel: RequestViewModel by sharedViewModel()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showMenu = false,
            showMessage = false,
            showTitleApp = Pair(true, getString(R.string.swap_history)),
            showBottomNav = false,
            showBoxChatLayout = Pair(false, null),
        )

        viewModel.getRequestHistory()
    }

    override fun setView() {
        viewPagerAdapter = TransHistoryViewPager(this)
        binding.pager.adapter = viewPagerAdapter
        TabLayoutMediator(binding.tabLayout, binding.pager) { tab, position ->
            when (position) {
                0 -> tab.text = getString(R.string.status_waiting)
                1 -> tab.text = getString(R.string.status_accepted)
                2 -> tab.text = getString(R.string.status_rejected)
                3 -> tab.text = getString(R.string.status_checkin)
                4 -> tab.text = getString(R.string.status_reviewing)
                5 -> tab.text = getString(R.string.status_done)
            }

        }.attach()
    }
}