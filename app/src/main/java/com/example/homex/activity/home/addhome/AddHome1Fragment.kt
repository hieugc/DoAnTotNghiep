package com.example.homex.activity.home.addhome

import androidx.fragment.app.viewModels
import com.example.homex.R
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentAddHome1Binding


class AddHome1Fragment : BaseFragment<FragmentAddHome1Binding>() {
    override val layoutId: Int = R.layout.fragment_add_home1
    private val viewModel: AddHomeViewModel by viewModels({requireParentFragment()})

    override fun setView() {
        binding.viewModel = viewModel
    }

    override fun setEvent() {
        binding.homeTypeRadioGroup.setOnCheckedChangeListener { group, id ->
            when(id){
                R.id.homeRB->{
                    viewModel.option.postValue(1)
                }
                R.id.apartmentRB->{
                    viewModel.option.postValue(2)
                }
            }
        }
    }
}