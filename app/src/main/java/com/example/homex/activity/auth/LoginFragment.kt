package com.example.homex.activity.auth

import androidx.core.widget.addTextChangedListener
import androidx.navigation.fragment.findNavController
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.app.EMAIL
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentLoginBinding
import com.example.homex.extension.disable
import com.example.homex.extension.enable
import com.example.homex.viewmodel.AuthViewModel
import com.homex.core.CoreApplication
import com.homex.core.param.auth.LoginParam
import org.koin.androidx.viewmodel.ext.android.viewModel


class LoginFragment : BaseFragment<FragmentLoginBinding>() {
    override val layoutId: Int = R.layout.fragment_login
    private val viewModel: AuthViewModel by viewModel()

    override fun setView() {
        binding.btnContinue.disable()
        binding.emailInputEdtTxt.setText(arguments?.getString(EMAIL))
        binding.emailInputEdtTxt.isEnabled = false
    }

    override fun setViewModel() {
        viewModel.loginLiveData.observe(viewLifecycleOwner){ user->
            if (user != null){
                CoreApplication.instance.saveToken(user.token)
                CoreApplication.instance.saveProfile(user.userInfo)
                activity?.finishAffinity()
                startActivity(HomeActivity.open(requireContext()))
            }
        }
    }

    override fun setEvent() {
        binding.passwordInputEdtTxt.addTextChangedListener {
            if(it?.isNotEmpty() == true)
                binding.btnContinue.enable()
            else
                binding.btnContinue.disable()
        }
        binding.btnBack.setOnClickListener {
            findNavController().popBackStack()
        }
        binding.btnContinue.setOnClickListener {
            viewModel.login(param = LoginParam(binding.emailInputEdtTxt.text.toString(), binding.passwordInputEdtTxt.text.toString()))
        }

        binding.forgotPassword.setOnClickListener {
            (activity as AuthActivity).redirectToForgotPassword(binding.emailInputEdtTxt.text.toString())
        }
    }

    override fun onResume() {
        super.onResume()
        binding.passwordInputEdtTxt.text?.clear()
    }
}