package com.example.homex

import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.navigation.fragment.findNavController
import com.example.homex.activity.home.HomeActivity
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentCreateRequestBinding
import com.example.homex.extension.gone
import com.example.homex.extension.visible


class CreateRequestFragment : BaseFragment<FragmentCreateRequestBinding>() {
    override val layoutId: Int = R.layout.fragment_create_request

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showTitleApp = Pair(true, "Tạo yêu cầu"),
            showMenu = false,
            showMessage = false,
            showBoxChatLayout = Pair(false, ""),
            showBottomNav = false,
            showLogo = false
        )
    }

    override fun setEvent() {
        binding.homeRB.setOnCheckedChangeListener { _, b ->
            if (b){
                binding.pointRB.isChecked = false
                checkUI()
            }
        }

        binding.pointRB.setOnCheckedChangeListener { _, b ->
            if (b){
                binding.homeRB.isChecked = false
                checkUI()
            }
        }

        binding.addYourHomeBtn.setOnClickListener {
            findNavController().navigate(R.id.action_createRequestFragment_to_pickYourHomeFragment)
        }
        binding.addTargetHomeBtn.setOnClickListener {
            findNavController().navigate(R.id.action_createRequestFragment_to_pickHomeFragment)
        }
    }

    private fun checkUI(){
        if(!binding.homeRB.isChecked && binding.pointRB.isChecked){
            binding.appCompatTextView26.text = getString(R.string.point_txt)
            binding.pointLayout.visible()
            binding.addYourHomeBtn.gone()
            binding.yourPointTV.visible()
        }
        else{
            binding.appCompatTextView26.text = getString(R.string.pick_your_home)
            binding.pointLayout.gone()
            binding.yourPointTV.gone()
            binding.addYourHomeBtn.visible()
        }
    }
}