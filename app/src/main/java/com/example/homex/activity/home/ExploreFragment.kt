package com.example.homex.activity.home

import android.os.Bundle
import android.view.View
import com.example.homex.R
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentExploreBinding


class ExploreFragment : BaseFragment<FragmentExploreBinding>() {
    override val layoutId: Int = R.layout.fragment_explore

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
}