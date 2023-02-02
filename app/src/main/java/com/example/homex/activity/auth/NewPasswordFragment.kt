package com.example.homex.activity.auth

import androidx.navigation.fragment.findNavController
import com.example.homex.R
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentNewPasswordBinding


class NewPasswordFragment : BaseFragment<FragmentNewPasswordBinding>() {
    override val layoutId: Int = R.layout.fragment_new_password

    override fun setEvent() {
        binding.btnBack.setOnClickListener {
            findNavController().navigateUp()
        }
    }
}