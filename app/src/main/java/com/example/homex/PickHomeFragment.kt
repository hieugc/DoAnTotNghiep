package com.example.homex

import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import com.example.homex.activity.home.HomeActivity
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentPickHomeBinding


class PickHomeFragment : BaseFragment<FragmentPickHomeBinding>() {
    override val layoutId: Int = R.layout.fragment_pick_home

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showBottomNav = false,
            showMessage = false,
            showMenu = false,
            showTitleApp = Pair(true, "Chọn nhà"),
            showBoxChatLayout = Pair(false, "")
        )
    }
}