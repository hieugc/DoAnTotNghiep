package com.example.homex.activity.auth

import android.content.Context
import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import androidx.core.os.bundleOf
import androidx.databinding.DataBindingUtil
import androidx.navigation.findNavController
import com.example.homex.R
import com.example.homex.app.EMAIL
import com.example.homex.base.BaseActivity
import com.example.homex.databinding.ActivityAuthBinding

class AuthActivity : BaseActivity() {
    private lateinit var binding: ActivityAuthBinding

    companion object{
        fun open(context: Context) = Intent(context, AuthActivity::class.java)
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = DataBindingUtil.setContentView(this, R.layout.activity_auth)
    }

    fun redirectToLogin(email: String){
        findNavController(R.id.nav_auth_fragment).navigate(R.id.action_getStartedFragment_to_loginFragment, bundleOf(EMAIL to email))
    }

    fun redirectToSignIn(email: String){
        findNavController(R.id.nav_auth_fragment).navigate(R.id.action_getStartedFragment_to_signInFragment, bundleOf(EMAIL to email))
    }

    fun redirectToVerification(email: String){
        findNavController(R.id.nav_auth_fragment).navigate(R.id.action_signInFragment_to_signInVerificationFragment, bundleOf(EMAIL to email))
    }

    fun redirectToUpdateInformation(email: String?){
        findNavController(R.id.nav_auth_fragment).navigate(R.id.action_signInVerificationFragment_to_updateInformationFragment, bundleOf(EMAIL to email))
    }
    fun redirectToLoginAfterUpdatePassword(email: String?){
        findNavController(R.id.nav_auth_fragment).navigate(R.id.action_newPasswordFragment_to_loginFragment, bundleOf(EMAIL to email))
    }

    fun redirectToForgotPassword(email: String){
        findNavController(R.id.nav_auth_fragment).navigate(R.id.action_loginFragment_to_forgotPasswordFragment, bundleOf(EMAIL to email))
    }

    fun redirectToVerificationForgotPassword(email: String){
        findNavController(R.id.nav_auth_fragment).navigate(R.id.action_forgotPasswordFragment_to_forgotPasswordVerificationFragment, bundleOf(EMAIL to email))
    }

    fun redirectToNewPassword(email: String){
        findNavController(R.id.nav_auth_fragment).navigate(R.id.action_forgotPasswordVerificationFragment_to_newPasswordFragment, bundleOf(EMAIL to email))
    }
}