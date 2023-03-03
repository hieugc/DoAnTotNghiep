package com.example.homex.activity.home

import android.content.Intent
import android.os.Bundle
import android.view.View
import androidx.navigation.fragment.findNavController
import com.example.homex.R
import com.example.homex.activity.auth.AuthActivity
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentUserBinding
import com.example.homex.extension.gone
import com.example.homex.extension.visible


class UserFragment : BaseFragment<FragmentUserBinding>() {
    override val layoutId: Int = R.layout.fragment_user

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

    override fun setView() {
        binding.userLayout.gone()
        binding.notLoginLayout.visible()
    }

    override fun setEvent() {
        binding.btnMyProfile.setOnClickListener {
            findNavController().navigate(R.id.action_userFragment_to_myProfileFragment)
        }
        binding.btnYourHouse.setOnClickListener {
            findNavController().navigate(R.id.action_userFragment_to_myHomeFragment)
        }

        binding.btnHistory.setOnClickListener{
            findNavController().navigate(R.id.action_userFragment_to_transHistoryFragment)
        }
        binding.btnPoint.setOnClickListener{
            findNavController().navigate(R.id.action_userFragment_to_pointHistoryFragment)
        }
        binding.goToAuthBtn.setOnClickListener {
            startActivity(Intent(requireContext(), AuthActivity::class.java))
            activity?.finish()
        }
        binding.btnPendingRequests.setOnClickListener {
            findNavController().navigate(R.id.action_userFragment_to_requestFragment)
        }
    }
}