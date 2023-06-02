package com.example.homex.activity.home.addhome

import androidx.core.widget.addTextChangedListener
import androidx.fragment.app.viewModels
import com.example.homex.R
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentAddHome2Binding

class AddHome2Fragment : BaseFragment<FragmentAddHome2Binding>() {
    override val layoutId: Int = R.layout.fragment_add_home2
    private val viewModel: AddHomeViewModel by viewModels({requireParentFragment()})

    override fun setView() {
        viewModel.apply {
            binding.homeNameInputEdtTxt.setText(this.name.value)
            if(this.square.value != 0)
                binding.homeAreaEdtTxt.setText(this.square.value.toString())
            binding.homeDescriptionEdtTxt.setText(this.description.value)
            if (this.price.value != 0)
                binding.homePriceEdtTxt.setText(this.price.value.toString())
        }
    }

    override fun setEvent() {
        binding.homeAreaEdtTxt.addTextChangedListener {
            if (it?.toString() == "")
                viewModel.square.postValue(0)
            else
                viewModel.square.postValue(it?.toString()?.toInt())
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

    override fun setViewModel() {
        viewModel.predict.observe(this){
            if (it != null)
                binding.homePredictTxt.text = getString(R.string.points_per_day, it)
        }
    }
}