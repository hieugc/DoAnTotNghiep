package com.example.homex.activity.auth

import androidx.core.text.HtmlCompat
import com.example.homex.R
import com.example.homex.app.EMAIL
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentForgotPasswordVerificationBinding
import com.example.homex.extension.disable
import com.example.homex.extension.enable
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
        binding.resendOTP.setOnClickListener {
            viewModel.resendOTPForgotPassword()
        }
    }

    override fun setViewModel() {
        viewModel.resendLiveData.observe(viewLifecycleOwner){}
        viewModel.otpLiveData.observe(viewLifecycleOwner){
            if(it?.token != null){
                CoreApplication.instance.saveToken(it.token)
                arguments?.getString(EMAIL)
                    ?.let { it1 -> (activity as AuthActivity).redirectToNewPassword(it1) }
            }
        }

        viewModel.seconds.observe(this){
            if(it == 0)
            {
                binding.resendOTP.enable()
                binding.resendOTP.text = HtmlCompat.fromHtml(String.format(getString(R.string.resend_otp_html)), HtmlCompat.FROM_HTML_MODE_LEGACY)
                return@observe
            }
            binding.resendOTP.text = "Đã gửi mã xác thực (${it}s)"
            binding.resendOTP.disable()
        }
    }
}