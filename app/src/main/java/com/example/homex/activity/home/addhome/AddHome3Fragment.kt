package com.example.homex.activity.home.addhome

import androidx.core.widget.addTextChangedListener
import com.example.homex.R
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentAddHome3Binding


class AddHome3Fragment : BaseFragment<FragmentAddHome3Binding>() {
    override val layoutId: Int = R.layout.fragment_add_home3

    override fun setEvent() {
        setEventAddMinus()

        binding.edtBed.addTextChangedListener {
            if (it?.toString() == "")
            {
                binding.edtBed.setText("1")
            }
            else if(it?.toString()?.toInt() == 0){
                binding.edtBed.setText("1")
            }
        }

        binding.edtPeople.addTextChangedListener {
            if (it?.toString() == "")
            {
                binding.edtPeople.setText("1")
            }
            else if(it?.toString()?.toInt() == 0){
                binding.edtPeople.setText("1")
            }
        }

        binding.edtBathRoom.addTextChangedListener {
            if (it?.toString() == "")
            {
                binding.edtBathRoom.setText("1")
            }
            else if(it?.toString()?.toInt() == 0){
                binding.edtBathRoom.setText("1")
            }
        }

        binding.edtBedRoom.addTextChangedListener {
            if (it?.toString() == "")
            {
                binding.edtBedRoom.setText("1")
            }
            else if(it?.toString()?.toInt() == 0){
                binding.edtBedRoom.setText("1")
            }
        }
    }

    private fun setEventAddMinus() {
        binding.addBed.setOnClickListener {
            val num = binding.edtBed.text.toString().toInt()
            binding.edtBed.setText((num + 1).toString())
        }
        binding.minusBed.setOnClickListener {
            val num = binding.edtBed.text.toString().toInt()
            if(num == 0) return@setOnClickListener
            binding.edtBed.setText((num - 1).toString())
        }
        binding.addPeople.setOnClickListener {
            val num = binding.edtPeople.text.toString().toInt()
            binding.edtPeople.setText((num + 1).toString())
        }
        binding.minusPeople.setOnClickListener {
            val num = binding.edtPeople.text.toString().toInt()
            if(num == 0) return@setOnClickListener
            binding.edtPeople.setText((num - 1).toString())
        }
        binding.addBathRoom.setOnClickListener {
            val num = binding.edtBathRoom.text.toString().toInt()
            binding.edtBathRoom.setText((num + 1).toString())
        }
        binding.minusBathRoom.setOnClickListener {
            val num = binding.edtBathRoom.text.toString().toInt()
            if(num == 0) return@setOnClickListener
            binding.edtBathRoom.setText((num - 1).toString())
        }
        binding.addBedRoom.setOnClickListener {
            val num = binding.edtBedRoom.text.toString().toInt()
            binding.edtBedRoom.setText((num + 1).toString())
        }
        binding.minusBedRoom.setOnClickListener {
            val num = binding.edtBedRoom.text.toString().toInt()
            if(num == 0) return@setOnClickListener
            binding.edtBedRoom.setText((num - 1).toString())
        }
    }
}