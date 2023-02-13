package com.example.homex.activity.home

import android.os.Bundle
import com.example.homex.R
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentUpdateProfileBinding

class UpdateProfileFragment : BaseFragment<FragmentUpdateProfileBinding>() {
    override val layoutId: Int = R.layout.fragment_update_profile
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showMenu = false,
            showMessage = false,
            showTitleApp = Pair(true, getString(R.string.user_profile)),
            showBottomNav = false,
            showBoxChatLayout = Pair(false, ""),
        )
    }

}