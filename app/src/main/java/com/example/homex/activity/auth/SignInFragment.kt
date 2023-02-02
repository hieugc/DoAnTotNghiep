package com.example.homex.activity.auth

import android.os.Build
import android.text.InputType
import android.text.method.LinkMovementMethod
import android.view.View
import androidx.appcompat.content.res.AppCompatResources
import androidx.core.text.HtmlCompat
import androidx.core.widget.addTextChangedListener
import androidx.navigation.fragment.findNavController
import com.example.homex.R
import com.example.homex.app.EMAIL
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentSignInBinding
import com.example.homex.extension.*


class SignInFragment : BaseFragment<FragmentSignInBinding>() {
    override val layoutId: Int = R.layout.fragment_sign_in

    override fun setEvent() {
        binding.btnBack.setOnClickListener {
            findNavController().navigateUp()
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
                (activity as AuthActivity).redirectToVerification(email = binding.emailInputEdtTxt.text.toString())
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

    override fun setView() {
        binding.emailInputEdtTxt.setText(arguments?.getString(EMAIL))
        binding.emailInputEdtTxt.inputType = InputType.TYPE_NULL
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            binding.emailInputLayout.focusable = View.NOT_FOCUSABLE
            binding.emailInputEdtTxt.focusable = View.NOT_FOCUSABLE
        }
        binding.btnContinue.disable()
        binding.policyText.movementMethod = LinkMovementMethod.getInstance()
        binding.policyText.text = HtmlCompat.fromHtml(resources.getString(R.string.sign_in_txt), HtmlCompat.FROM_HTML_MODE_LEGACY)
    }
}