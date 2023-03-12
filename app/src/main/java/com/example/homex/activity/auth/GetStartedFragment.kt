package com.example.homex.activity.auth

import androidx.core.widget.addTextChangedListener
import com.example.homex.R
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentGetStartedBinding
import com.example.homex.extension.disable
import com.example.homex.extension.enable
import com.example.homex.extension.isValidEmail
import com.example.homex.viewmodel.AuthViewModel
import com.homex.core.param.auth.EmailParam
import org.koin.androidx.viewmodel.ext.android.viewModel


class GetStartedFragment : BaseFragment<FragmentGetStartedBinding>() {
    override val layoutId: Int = R.layout.fragment_get_started
    private val viewModel: AuthViewModel by viewModel()

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
            viewModel.checkEmailExisted(EmailParam(binding.emailInputEdtTxt.text.toString()))
        }
        binding.btnBack.setOnClickListener {
            activity?.finish()
        }
    }

    override fun setViewModel() {
        viewModel.checkEmailLiveData.observe(viewLifecycleOwner){
            if(it?.isExisted == true){
                (activity as AuthActivity).redirectToLogin(binding.emailInputEdtTxt.text.toString())
            }else if(it?.isExisted == false){
                (activity as AuthActivity).redirectToSignIn(binding.emailInputEdtTxt.text.toString())
            }
        }
    }
}