package com.example.homex.activity.home

import android.os.Bundle
import androidx.navigation.fragment.findNavController
import com.example.homex.R
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentMyProfileBinding

class MyProfileFragment : BaseFragment<FragmentMyProfileBinding>() {
    override val layoutId: Int = R.layout.fragment_my_profile

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showMenu = false,
            showMessage = false,
            showTitleApp = Pair(true, getString(R.string.my_profile)),
            showBottomNav = false,
            showBoxChatLayout = Pair(false, null),
        )
    }

    override fun setEvent() {
        binding.btnUpdateProfile.setOnClickListener {
            findNavController().navigate(R.id.action_myProfileFragment_to_updateProfileFragment)
        }
        binding.btnChangePassword.setOnClickListener {
            findNavController().navigate(R.id.action_myProfileFragment_to_changePasswordFragment)
        }
    }
}