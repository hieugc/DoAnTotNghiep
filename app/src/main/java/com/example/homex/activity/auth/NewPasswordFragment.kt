package com.example.homex.activity.auth

import android.widget.Toast
import androidx.appcompat.content.res.AppCompatResources
import androidx.core.widget.addTextChangedListener
import androidx.navigation.fragment.findNavController
import com.example.homex.R
import com.example.homex.app.EMAIL
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentNewPasswordBinding
import com.example.homex.extension.*
import com.example.homex.viewmodel.AuthViewModel
import com.homex.core.param.auth.PasswordParam
import org.koin.androidx.viewmodel.ext.android.viewModel


class NewPasswordFragment : BaseFragment<FragmentNewPasswordBinding>() {
    override val layoutId: Int = R.layout.fragment_new_password
    private val viewModel: AuthViewModel by viewModel()

    override fun setEvent() {
        binding.btnBack.setOnClickListener {
            findNavController().navigateUp()
        }

        binding.newPasswordInputEdtTxt.addTextChangedListener {
            checkNewPassword()
            checkMatchPassword()
            if (it.isNullOrEmpty()) {
                binding.pwdCheckGroup.gone()
            }
        }

        binding.confirmPasswordInputEdtTxt.addTextChangedListener {
            checkMatchPassword()
        }

        binding.btnContinue.setOnClickListener {
            viewModel.updatePassword(param = PasswordParam(password = binding.newPasswordInputEdtTxt.text.toString(), confirmPassword = binding.confirmPasswordInputEdtTxt.text.toString()))
        }
    }

    override fun setViewModel() {
        viewModel.passwordLiveData.observe(viewLifecycleOwner){
            Toast.makeText(requireContext(), "Đổi mật khẩu thành công", Toast.LENGTH_SHORT).show()
            (activity as AuthActivity).redirectToLoginAfterUpdatePassword(arguments?.getString(EMAIL))
        }
    }

    override fun setView() {
        binding.btnContinue.disable()
    }

    private fun checkMatchPassword(){
        val newPassword = binding.newPasswordInputEdtTxt.text.toString()
        val confirmPassword = binding.confirmPasswordInputEdtTxt.text.toString()

        if(newPassword == confirmPassword && newPassword.checkPassword())
        {
            binding.btnContinue.enable()
        }
        else{
            binding.btnContinue.disable()
        }
    }

    private fun checkNewPassword(){
        val character = binding.newPasswordInputEdtTxt.text.toString().length in 8..32
        val upperAndLowCase = binding.newPasswordInputEdtTxt.text.toString().checkPassword()

        if (!character && !upperAndLowCase) {
            binding.pwdCheckGroup.visible()
            binding.ivCheck1.setImageDrawable(
                AppCompatResources.getDrawable(requireContext(),
                R.drawable.ic_uncheck_circle
            ))
            binding.ivCheck2.setImageDrawable(
                AppCompatResources.getDrawable(requireContext(),
                R.drawable.ic_uncheck_circle
            ))
        }
        else {
            if(character){
                binding.pwdCheckGroup.visible()
                binding.ivCheck1.setImageDrawable(
                    AppCompatResources.getDrawable(requireContext(),
                    R.drawable.ic_check_circle
                ))
            }else{
                binding.ivCheck1.setImageDrawable(
                    AppCompatResources.getDrawable(requireContext(),
                    R.drawable.ic_uncheck_circle
                ))
            }
            if(upperAndLowCase){
                binding.pwdCheckGroup.visible()
                binding.ivCheck2.setImageDrawable(
                    AppCompatResources.getDrawable(requireContext(),
                    R.drawable.ic_check_circle
                ))
            }else{
                binding.ivCheck2.setImageDrawable(
                    AppCompatResources.getDrawable(requireContext(),
                    R.drawable.ic_uncheck_circle
                ))
            }

        }
    }

    override fun onResume() {
        super.onResume()
        binding.newPasswordInputEdtTxt.text?.clear()
        binding.confirmPasswordInputEdtTxt.text?.clear()
    }
}