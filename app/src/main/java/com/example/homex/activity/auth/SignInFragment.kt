package com.example.homex.activity.auth

import android.text.method.LinkMovementMethod
import androidx.appcompat.content.res.AppCompatResources
import androidx.core.text.HtmlCompat
import androidx.core.widget.addTextChangedListener
import androidx.navigation.fragment.findNavController
import com.example.homex.R
import com.example.homex.app.EMAIL
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentSignInBinding
import com.example.homex.extension.*
import com.example.homex.viewmodel.AuthViewModel
import com.homex.core.CoreApplication
import com.homex.core.param.auth.LoginParam
import org.koin.androidx.viewmodel.ext.android.viewModel


class SignInFragment : BaseFragment<FragmentSignInBinding>() {
    override val layoutId: Int = R.layout.fragment_sign_in
    private val viewModel: AuthViewModel by viewModel()

    override fun setEvent() {
        binding.btnBack.setOnClickListener {
            findNavController().navigate(R.id.action_signInFragment_to_getStartedFragment)
        }
        binding.passwordInputEdtTxt.addTextChangedListener {
            checkPassword()
            if(it.isNullOrEmpty()){
                binding.pwdCheckGroup.gone()
                binding.btnContinue.disable()
            }
        }
        binding.btnContinue.setOnClickListener {
            if (checkPassword()){
                viewModel.signup(LoginParam(email = binding.emailInputEdtTxt.text.toString(), password = binding.passwordInputEdtTxt.text.toString()))
            }
        }
    }

    private fun checkPassword(): Boolean{
        val lengthCheck = binding.passwordInputEdtTxt.text.toString().length in 8..32
        val upperLowerCheck = binding.passwordInputEdtTxt.text.toString().checkUpperLower()


        if(!upperLowerCheck && !lengthCheck){
            binding.pwdCheckGroup.visible()
            binding.ivCheck1.setImageDrawable(AppCompatResources.getDrawable(requireContext(),
                R.drawable.ic_uncheck_circle
            ))
            binding.ivCheck2.setImageDrawable(AppCompatResources.getDrawable(requireContext(),
                R.drawable.ic_uncheck_circle
            ))
        }
        else{
            if(upperLowerCheck){
                binding.pwdCheckGroup.visible()
                binding.ivCheck2.setImageDrawable(AppCompatResources.getDrawable(requireContext(),
                    R.drawable.ic_check_circle
                ))
            }
            else{
                binding.ivCheck1.setImageDrawable(AppCompatResources.getDrawable(requireContext(),
                    R.drawable.ic_uncheck_circle
                ))
            }
            if(lengthCheck){
                binding.pwdCheckGroup.visible()
                binding.ivCheck1.setImageDrawable(AppCompatResources.getDrawable(requireContext(),
                    R.drawable.ic_check_circle
                ))
            }else{
                binding.ivCheck1.setImageDrawable(AppCompatResources.getDrawable(requireContext(),
                    R.drawable.ic_uncheck_circle
                ))
            }
            if(upperLowerCheck && lengthCheck) {
                binding.btnContinue.enable()
                return true
            }
            else
                binding.btnContinue.disable()
        }
        return false
    }

    override fun setViewModel() {
        viewModel.signupLiveData.observe(viewLifecycleOwner){
            if (it?.token != null){
                CoreApplication.instance.saveToken(token = it.token)
                (activity as AuthActivity).redirectToVerification(email = binding.emailInputEdtTxt.text.toString())
            }
        }
    }

    override fun setView() {
        binding.emailInputEdtTxt.setText(arguments?.getString(EMAIL))
        binding.btnContinue.disable()
        binding.policyText.movementMethod = LinkMovementMethod.getInstance()
        binding.emailInputEdtTxt.isEnabled = false
        binding.policyText.text = HtmlCompat.fromHtml(resources.getString(R.string.sign_in_txt), HtmlCompat.FROM_HTML_MODE_LEGACY)
    }

    override fun onResume() {
        super.onResume()
        binding.passwordInputEdtTxt.text?.clear()
    }
}