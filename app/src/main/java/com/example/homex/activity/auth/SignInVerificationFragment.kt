package com.example.homex.activity.auth

import com.example.homex.R
import com.example.homex.app.EMAIL
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentSignInVerificationBinding
import com.example.homex.viewmodel.AuthViewModel
import com.homex.core.CoreApplication
import com.homex.core.param.auth.OTPParam
import org.koin.androidx.viewmodel.ext.android.viewModel


class SignInVerificationFragment : BaseFragment<FragmentSignInVerificationBinding>() {
    override val layoutId: Int = R.layout.fragment_sign_in_verification
    private val viewModel: AuthViewModel by viewModel()

    override fun setEvent() {
        binding.btnContinue.setOnClickListener {
            viewModel.checkOTPSignUp(param = OTPParam(binding.codeInputEdtTxt.text.toString()))
        }

        binding.resendOTP.setOnClickListener {

        }
    }

    override fun setViewModel() {
        viewModel.otpLiveData.observe(viewLifecycleOwner){
            if(it?.token != null){
                CoreApplication.instance.saveToken(it.token)
                (activity as AuthActivity).redirectToUpdateInformation(arguments?.getString(EMAIL))
            }
        }
    }
}