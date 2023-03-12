package com.example.homex.activity.auth

import androidx.navigation.fragment.findNavController
import com.example.homex.R
import com.example.homex.app.EMAIL
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentForgotPasswordBinding
import com.example.homex.viewmodel.AuthViewModel
import com.homex.core.CoreApplication
import com.homex.core.param.auth.EmailParam
import org.koin.androidx.viewmodel.ext.android.viewModel


class ForgotPasswordFragment : BaseFragment<FragmentForgotPasswordBinding>() {
    override val layoutId: Int = R.layout.fragment_forgot_password
    private val viewModel: AuthViewModel by viewModel()

    override fun setEvent() {
        binding.btnBackToLogin.setOnClickListener {
            findNavController().navigateUp()
        }
        binding.btnBack.setOnClickListener {
            findNavController().navigateUp()
        }
        binding.btnContinue.setOnClickListener {
            viewModel.forgotPassword(EmailParam(binding.emailInputEdtTxt.text.toString()))
        }
    }

    override fun setViewModel() {
        viewModel.forgotLiveData.observe(viewLifecycleOwner){
            if (it?.token != null){
                CoreApplication.instance.saveToken(it.token)
                (activity as AuthActivity).redirectToVerificationForgotPassword(binding.emailInputEdtTxt.text.toString())
            }
        }
    }

    override fun setView() {
        binding.emailInputEdtTxt.setText(arguments?.getString(EMAIL))
        binding.emailInputEdtTxt.isEnabled = false
    }
}