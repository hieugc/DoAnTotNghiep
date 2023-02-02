package com.example.homex.activity.auth

import androidx.core.widget.addTextChangedListener
import com.example.homex.R
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentGetStartedBinding
import com.example.homex.extension.disable
import com.example.homex.extension.enable
import com.example.homex.extension.isValidEmail


class GetStartedFragment : BaseFragment<FragmentGetStartedBinding>() {
    override val layoutId: Int = R.layout.fragment_get_started

    override fun setView() {
        binding.btnContinue.disable()
    }

    override fun setEvent() {
        binding.emailInputEdtTxt.addTextChangedListener { t->
            if (t.toString().isValidEmail()){
                binding.btnContinue.enable()
            }else{
                binding.btnContinue.disable()
            }
        }

        binding.btnContinue.setOnClickListener {
            (activity as AuthActivity).redirectToLogin(binding.emailInputEdtTxt.text.toString())
        }
    }
}