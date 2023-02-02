package com.example.homex.activity.auth

import com.example.homex.R
import com.example.homex.app.EMAIL
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentForgotPasswordVerificationBinding


class ForgotPasswordVerificationFragment : BaseFragment<FragmentForgotPasswordVerificationBinding>() {
    override val layoutId: Int = R.layout.fragment_forgot_password_verification

    override fun setEvent() {
        binding.btnContinue.setOnClickListener {
            arguments?.getString(EMAIL)
                ?.let { it1 -> (activity as AuthActivity).redirectToNewPassword(it1) }
        }
    }
}