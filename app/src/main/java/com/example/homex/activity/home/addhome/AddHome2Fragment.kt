package com.example.homex.activity.home.addhome

import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.core.widget.addTextChangedListener
import androidx.fragment.app.viewModels
import com.example.homex.R
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentAddHome2Binding

class AddHome2Fragment : BaseFragment<FragmentAddHome2Binding>() {
    override val layoutId: Int = R.layout.fragment_add_home2
    private val viewModel: AddHomeViewModel by viewModels({requireParentFragment()})

    override fun setEvent() {
        binding.homeAreaEdtTxt.addTextChangedListener {
            if (it?.toString() == "")
                viewModel.square.postValue(0)
            else
                viewModel.square.postValue(it?.toString()?.toInt())
        }
        binding.homeAddressEdtTxt.addTextChangedListener {
            viewModel.location.postValue(it?.toString()?:"")
        }
        binding.homeDescriptionEdtTxt.addTextChangedListener {
            viewModel.description.postValue(it?.toString()?:"")
        }
        binding.homePriceEdtTxt.addTextChangedListener {
            if (it?.toString() == "")
                viewModel.price.postValue(0)
            else
                viewModel.price.postValue(it?.toString()?.toInt())
        }
        binding.homeNameInputEdtTxt.addTextChangedListener {
            viewModel.name.postValue(it?.toString()?:"")
        }
    }
}