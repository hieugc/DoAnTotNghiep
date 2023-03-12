package com.example.homex.activity.auth

import com.example.homex.R
import com.example.homex.app.EMAIL
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentForgotPasswordVerificationBinding
import com.example.homex.viewmodel.AuthViewModel
import com.homex.core.CoreApplication
import com.homex.core.param.auth.OTPParam
import org.koin.androidx.viewmodel.ext.android.viewModel


class ForgotPasswordVerificationFragment : BaseFragment<FragmentForgotPasswordVerificationBinding>() {
    override val layoutId: Int = R.layout.fragment_forgot_password_verification
    private val viewModel: AuthViewModel by viewModel()

    override fun setEvent() {
        binding.btnContinue.setOnClickListener {
            viewModel.checkOTPForgotPassword(param = OTPParam(binding.codeInputEdtTxt.text.toString()))
        }
    }

    override fun setViewModel() {
        viewModel.otpLiveData.observe(viewLifecycleOwner){
            if(it?.token != null){
                CoreApplication.instance.saveToken(it.token)
                arguments?.getString(EMAIL)
                    ?.let { it1 -> (activity as AuthActivity).redirectToNewPassword(it1) }
            }
        }
    }
}