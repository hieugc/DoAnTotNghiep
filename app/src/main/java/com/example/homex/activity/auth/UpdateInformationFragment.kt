package com.example.homex.activity.auth

import android.widget.ArrayAdapter
import com.example.homex.R
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentUpdateInformationBinding


class UpdateInformationFragment : BaseFragment<FragmentUpdateInformationBinding>() {
    override val layoutId: Int = R.layout.fragment_update_information

    override fun setView() {
        val items = listOf("Nam", "Nữ")
        val adapter = ArrayAdapter(requireContext(), R.layout.sex_item, items)
        binding.autoCompleteTV.setAdapter(adapter)
    }
}