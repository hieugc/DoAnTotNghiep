package com.example.homex.activity.auth

import com.example.homex.R
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentSignInVerificationBinding


class SignInVerificationFragment : BaseFragment<FragmentSignInVerificationBinding>() {
    override val layoutId: Int = R.layout.fragment_sign_in_verification

    override fun setEvent() {
        binding.btnContinue.setOnClickListener {
            (activity as AuthActivity).redirectToUpdateInformation("")
        }
    }
}