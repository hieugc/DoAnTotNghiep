package com.example.homex.activity.home.profile

import android.content.Intent
import android.os.Bundle
import android.view.View
import androidx.navigation.fragment.findNavController
import com.bumptech.glide.Glide
import com.example.homex.R
import com.example.homex.activity.auth.AuthActivity
import com.example.homex.activity.home.HomeActivity
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentUserBinding
import com.example.homex.extension.gone
import com.example.homex.extension.visible
import com.google.android.material.dialog.MaterialAlertDialogBuilder
import com.homex.core.util.PrefUtil
import org.koin.android.ext.android.inject

class UserFragment : BaseFragment<FragmentUserBinding>() {
    override val layoutId: Int = R.layout.fragment_user
    private val prefUtil: PrefUtil by inject()
    private var isAuthenticated = true

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = true,
            showMenu = false,
            showMessage = true,
            showTitleApp = Pair(false, ""),
            showBottomNav = true,
            showBoxChatLayout = Pair(false, null),
        )
    }

    override fun setView() {
        if(prefUtil.profile == null && prefUtil.token == null){
            binding.userLayout.gone()
            binding.llProfileNoLogin.visible()
            isAuthenticated = false
        }
        else if(prefUtil.profile != null){
            binding.user = prefUtil.profile
            Glide.with(requireContext())
                .load(prefUtil.profile?.urlImage)
                .placeholder(R.drawable.ic_baseline_image_24)
                .error(R.mipmap.avatar)
                .into(binding.ivAvatar)
        }
    }

    override fun setEvent() {
        binding.btnMyProfile.setOnClickListener {
            if (isAuthenticated)
                findNavController().navigate(R.id.action_userFragment_to_myProfileFragment)
        }
        binding.btnYourHouse.setOnClickListener {
            if (isAuthenticated)
                findNavController().navigate(R.id.action_userFragment_to_myHomeFragment)
        }

        binding.btnHistory.setOnClickListener{
            if (isAuthenticated)
                findNavController().navigate(R.id.action_userFragment_to_transHistoryFragment)
        }
        binding.btnPoint.setOnClickListener{
            if (isAuthenticated)
                findNavController().navigate(R.id.action_userFragment_to_pointHistoryFragment)
        }
        binding.btnLogin.setOnClickListener {
            startActivity(AuthActivity.open(requireContext()))
        }
        binding.btnPendingRequests.setOnClickListener {
            if (isAuthenticated)
                findNavController().navigate(R.id.action_userFragment_to_requestFragment)
        }
        binding.btnLogout.setOnClickListener {
            if (!isAuthenticated)
                return@setOnClickListener
            MaterialAlertDialogBuilder(requireContext())
                .setTitle(getString(R.string.logout))
                .setMessage(getString(R.string.logout_message))
                .setNegativeButton(resources.getString(R.string.cancel)) { _, _ ->
                    // Respond to negative button press
                }
                .setPositiveButton(resources.getString(R.string.confirm)) { dialog, _ ->
                    // Respond to positive button press
                    dialog.dismiss()
                    prefUtil.clearAllData()
                    val i = Intent(requireContext(), HomeActivity::class.java)
                    activity?.finish()
                    activity?.overridePendingTransition(0, 0)
                    startActivity(i)
                    activity?.overridePendingTransition(0, 0)
                }
                .show()
        }

        binding.btnCircleRequest.setOnClickListener{
            if (isAuthenticated)
                findNavController().navigate(R.id.action_userFragment_to_circleRequestFragment)
        }
    }
}