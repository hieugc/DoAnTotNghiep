package com.example.homex.activity.auth

import android.text.InputType
import androidx.navigation.fragment.findNavController
import com.example.homex.R
import com.example.homex.app.EMAIL
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentForgotPasswordBinding


class ForgotPasswordFragment : BaseFragment<FragmentForgotPasswordBinding>() {
    override val layoutId: Int = R.layout.fragment_forgot_password

    override fun setEvent() {
        binding.btnBackToLogin.setOnClickListener {
            findNavController().navigateUp()
        }
        binding.btnBack.setOnClickListener {
            findNavController().navigateUp()
        }
        binding.btnContinue.setOnClickListener {
            (activity as AuthActivity).redirectToVerificationForgotPassword(binding.emailInputEdtTxt.text.toString())
        }
    }

    override fun setView() {
        binding.emailInputEdtTxt.setText(arguments?.getString(EMAIL))
        binding.emailInputEdtTxt.inputType = InputType.TYPE_NULL
    }
}