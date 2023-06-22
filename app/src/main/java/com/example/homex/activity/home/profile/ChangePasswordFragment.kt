package com.example.homex.activity.home.profile

import android.os.Bundle
import androidx.appcompat.content.res.AppCompatResources
import androidx.core.widget.addTextChangedListener
import androidx.navigation.fragment.findNavController
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.base.BaseActivity
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentChangePasswordBinding
import com.example.homex.extension.checkPassword
import com.example.homex.extension.disable
import com.example.homex.extension.enable
import com.example.homex.viewmodel.ProfileViewModel
import com.homex.core.param.auth.PasswordParam
import org.koin.androidx.viewmodel.ext.android.viewModel

class ChangePasswordFragment : BaseFragment<FragmentChangePasswordBinding>() {
    override val layoutId: Int = R.layout.fragment_change_password
    private val viewModel: ProfileViewModel by viewModel()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showMenu = false,
            showMessage = false,
            showTitleApp = Pair(true, getString(R.string.change_password)),
            showBottomNav = false,
            showBoxChatLayout = Pair(false, null),
        )
    }

    override fun setEvent() {

        binding.newPasswordInputEdtTxt.addTextChangedListener {
            checkNewPassword()
            checkMatchPassword()
        }

        binding.confirmPasswordInputEdtTxt.addTextChangedListener {
            checkMatchPassword()
        }

        binding.btnChangePwd.setOnClickListener {
            viewModel.updatePassword(param = PasswordParam(
                password = binding.newPasswordInputEdtTxt.text.toString()
                , confirmPassword = binding.confirmPasswordInputEdtTxt.text.toString()
            ))
        }
    }

    override fun setViewModel() {
        viewModel.passwordLiveData.observe(viewLifecycleOwner){
            (activity as BaseActivity).displayMessage(getString(R.string.change_password_success))
            findNavController().popBackStack()
        }
    }

    private fun checkMatchPassword(){
        val newPassword = binding.newPasswordInputEdtTxt.text.toString()
        val confirmPassword = binding.confirmPasswordInputEdtTxt.text.toString()
        val character = newPassword.length in 8..32
        val upperAndLowCase = newPassword.checkPassword()

        if (!character || !upperAndLowCase){
            binding.btnChangePwd.disable()
        }
        else if(newPassword == confirmPassword && newPassword.checkPassword())
        {
            binding.btnChangePwd.enable()
        }
        else{
            binding.btnChangePwd.disable()
        }
    }

    private fun checkNewPassword(){
        val character = binding.newPasswordInputEdtTxt.text.toString().length in 8..32
        val upperAndLowCase = binding.newPasswordInputEdtTxt.text.toString().checkPassword()

        if (!character && !upperAndLowCase) {
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

    override fun setView() {
        binding.btnChangePwd.disable()
    }

    override fun onResume() {
        super.onResume()
        binding.newPasswordInputEdtTxt.text?.clear()
        binding.confirmPasswordInputEdtTxt.text?.clear()
    }

}