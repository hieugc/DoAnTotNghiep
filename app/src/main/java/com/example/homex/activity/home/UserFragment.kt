package com.example.homex.activity.home

import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.view.View
import androidx.navigation.fragment.findNavController
import com.example.homex.MainActivity
import com.example.homex.R
import com.example.homex.activity.auth.AuthActivity
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentUserBinding
import com.example.homex.extension.gone
import com.example.homex.extension.visible
import com.homex.core.util.PrefUtil
import org.koin.android.ext.android.inject


class UserFragment : BaseFragment<FragmentUserBinding>() {
    override val layoutId: Int = R.layout.fragment_user
    private val prefUtil: PrefUtil by inject()

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
        if(prefUtil.profile == null && prefUtil.token == null){
            binding.userLayout.gone()
            binding.notLoginLayout.visible()
        }
        else if(prefUtil.profile != null){
            binding.user = prefUtil.profile
        }
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
            startActivity(AuthActivity.open(requireContext()))
        }
        binding.btnPendingRequests.setOnClickListener {
            findNavController().navigate(R.id.action_userFragment_to_requestFragment)
        }
        binding.btnLogout.setOnClickListener {
            prefUtil.clearAllData()
            val i = Intent(requireContext(), HomeActivity::class.java)
            activity?.finish()
            activity?.overridePendingTransition(0, 0)
            startActivity(i)
            activity?.overridePendingTransition(0, 0)
        }
    }
}