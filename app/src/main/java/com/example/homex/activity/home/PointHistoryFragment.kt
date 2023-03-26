package com.example.homex.activity.home

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import com.example.homex.R
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentPointHistoryBinding

class PointHistoryFragment : BaseFragment<FragmentPointHistoryBinding>() {
    override val layoutId: Int = R.layout.fragment_point_history

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
    }

}