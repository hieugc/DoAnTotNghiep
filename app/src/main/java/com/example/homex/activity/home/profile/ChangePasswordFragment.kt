package com.example.homex.activity.home.profile

import android.os.Bundle
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentChangePasswordBinding

class ChangePasswordFragment : BaseFragment<FragmentChangePasswordBinding>() {
    override val layoutId: Int = R.layout.fragment_change_password
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showMenu = false,
            showMessage = false,
            showTitleApp = Pair(true, getString(R.string.change_password)),
            showBottomNav = false,
            showBoxChatLayout = Pair(false, null),
        )
    }

}