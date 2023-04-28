package com.example.homex.activity.home.profile

import android.os.Bundle
import android.view.View
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.PaymentHistoryViewPager
import com.example.homex.adapter.TransHistoryViewPager
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentPointHistoryBinding
import com.example.homex.extension.thousandSeparator
import com.example.homex.viewmodel.ProfileViewModel
import com.google.android.material.tabs.TabLayoutMediator
import com.homex.core.util.AppEvent
import org.koin.androidx.viewmodel.ext.android.viewModel

class PointHistoryFragment: BaseFragment<FragmentPointHistoryBinding>(), PointInputDialogFragment.EventListener {
    override val layoutId: Int = R.layout.fragment_point_history
    private val viewModel: ProfileViewModel by viewModel()
    private lateinit var viewPagerAdapter: PaymentHistoryViewPager

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showBottomNav = false,
            showTitleApp = Pair(true, "Điểm tích lũy"),
            showMessage = false,
            showMenu = false,
            showBoxChatLayout = Pair(false, null),
            showSearchLayout = false
        )

        viewModel.getPoint()
    }

    override fun setView() {
        viewPagerAdapter = PaymentHistoryViewPager(this)
        binding.pager.adapter = viewPagerAdapter
        TabLayoutMediator(binding.tabLayout, binding.pager) { tab, position ->
            when (position) {
                0 -> tab.text = getString(R.string.history)
                1 -> tab.text = getString(R.string.received)
                2 -> tab.text = getString(R.string.used)
            }

        }.attach()
    }

    override fun setEvent() {
        binding.btnTopUp.setOnClickListener{
            PointInputDialogFragment(this).show(parentFragmentManager, "PointInputDialogFragment")
        }
    }

    override fun setViewModel() {
        viewModel.pointLiveData.observe(viewLifecycleOwner){
            if (it != null){
                binding.tvPointAvailable.text = it.thousandSeparator()
            }
            AppEvent.closePopup()
        }
    }

    override fun onPaymentSuccess() {
        viewModel.getPoint()
    }

}