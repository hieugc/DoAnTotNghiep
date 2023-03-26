package com.example.homex.activity.home.profile

import android.net.Uri
import android.os.Bundle
import android.view.View
import androidx.lifecycle.Lifecycle
import androidx.lifecycle.LifecycleEventObserver
import androidx.navigation.fragment.findNavController
import com.bumptech.glide.Glide
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.app.IMAGE
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentUpdateProfileBinding

class UpdateProfileFragment : BaseFragment<FragmentUpdateProfileBinding>() {
    override val layoutId: Int = R.layout.fragment_update_profile

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
        showLogo = false,
        showMenu = false,
        showMessage = false,
        showTitleApp = Pair(true, getString(R.string.user_profile)),
        showBottomNav = false,
        showBoxChatLayout = Pair(false, null),
        )
        val navController = findNavController();
        // After a configuration change or process death, the currentBackStackEntry
        // points to the dialog destination, so you must use getBackStackEntry()
        // with the specific ID of your destination to ensure we always
        // get the right NavBackStackEntry
        val navBackStackEntry = navController.getBackStackEntry(R.id.updateProfileFragment)

        // Create our observer and add it to the NavBackStackEntry's lifecycle
        val observer = LifecycleEventObserver { _, event ->
            if (event == Lifecycle.Event.ON_RESUME
                && navBackStackEntry.savedStateHandle.contains(IMAGE)) {
                val result = navBackStackEntry.savedStateHandle.get<Uri>(IMAGE);
                // Do something with the result
                Glide.with(requireContext())
                    .load(result)
                    .placeholder(R.drawable.ic_baseline_image_24)
                    .error(R.mipmap.avatar)
                    .into(binding.ivAvatar)
            }
        }
        navBackStackEntry.lifecycle.addObserver(observer)

        // As addObserver() does not automatically remove the observer, we
        // call removeObserver() manually when the view lifecycle is destroyed
        viewLifecycleOwner.lifecycle.addObserver(LifecycleEventObserver { _, event ->
            if (event == Lifecycle.Event.ON_DESTROY) {
                navBackStackEntry.lifecycle.removeObserver(observer)
            }
        })
    }

    override fun setEvent() {
        binding.tvEditProfile.setOnClickListener {
            findNavController().navigate(R.id.action_updateProfileFragment_to_bottomSheetDialogSelectImage)
        }
    }

}