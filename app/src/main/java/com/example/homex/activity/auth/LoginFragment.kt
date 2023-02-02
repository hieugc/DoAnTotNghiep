package com.example.homex.activity.auth

import android.os.Build
import android.text.InputType
import android.view.View
import androidx.navigation.fragment.findNavController
import com.example.homex.R
import com.example.homex.app.EMAIL
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentLoginBinding


class LoginFragment : BaseFragment<FragmentLoginBinding>() {
    override val layoutId: Int = R.layout.fragment_login

    override fun setView() {
        binding.emailInputEdtTxt.setText(arguments?.getString(EMAIL))
        binding.emailInputEdtTxt.inputType = InputType.TYPE_NULL
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            binding.emailInputLayout.focusable = View.NOT_FOCUSABLE
            binding.emailInputEdtTxt.focusable = View.NOT_FOCUSABLE
        }
        binding.forgotPassword.setOnClickListener {
            (activity as AuthActivity).redirectToForgotPassword(binding.emailInputEdtTxt.text.toString())
        }
    }

    override fun setEvent() {
        binding.btnBack.setOnClickListener {
            findNavController().navigateUp()
        }
    }
}