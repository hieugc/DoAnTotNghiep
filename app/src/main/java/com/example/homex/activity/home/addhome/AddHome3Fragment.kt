package com.example.homex.activity.home.addhome

import androidx.core.widget.addTextChangedListener
import androidx.fragment.app.viewModels
import com.example.homex.R
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentAddHome3Binding
import com.example.homex.extension.Rules
import com.example.homex.extension.Utilities


class AddHome3Fragment : BaseFragment<FragmentAddHome3Binding>() {
    override val layoutId: Int = R.layout.fragment_add_home3
    private val viewModel: AddHomeViewModel by viewModels({requireParentFragment()})
    private lateinit var utilsList : List<Int>
    private lateinit var rulesList: List<Int>

    override fun setView() {
        viewModel.apply {
            binding.edtBedRoom.setText(this.bedroom.value.toString())
            binding.edtBathRoom.setText(this.bathroom.value.toString())
            binding.edtPeople.setText(this.people.value.toString())
            binding.viewModel = viewModel
        }
    }

    override fun setEvent() {
        setEventAddMinus()

        binding.chipGroup.setOnCheckedStateChangeListener { group, checkedIds ->
            utilsList = checkedIds
            updateUtilsList()
        }

        binding.rulesGroup.setOnCheckedStateChangeListener { group, checkedIds ->
            rulesList = checkedIds
            updateRulesList()
        }

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
                viewModel.people.postValue(1)
            }
            else if(it?.toString()?.toInt() == 0){
                binding.edtPeople.setText("1")
                viewModel.people.postValue(1)
            }else{
                viewModel.people.postValue(it?.toString()?.toInt()?:1)
            }
        }

        binding.edtBathRoom.addTextChangedListener {
            if (it?.toString() == "")
            {
                binding.edtBathRoom.setText("1")
                viewModel.bathroom.postValue(1)
            }
            else if(it?.toString()?.toInt() == 0){
                binding.edtBathRoom.setText("1")
                viewModel.bathroom.postValue(1)
            }else{
                viewModel.bathroom.postValue(it?.toString()?.toInt()?:1)
            }
        }

        binding.edtBedRoom.addTextChangedListener {
            if (it?.toString() == "")
            {
                binding.edtBedRoom.setText("1")
                viewModel.bedroom.postValue(1)
            }
            else if(it?.toString()?.toInt() == 0){
                binding.edtBedRoom.setText("1")
                viewModel.bedroom.postValue(1)
            }else{
                viewModel.bedroom.postValue(it?.toString()?.toInt()?:1)
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

    private fun updateUtilsList(){
        val tmp: MutableList<Int> = mutableListOf()
        for (id in utilsList){
            when(id){
                R.id.wifiChip->{
                    tmp.add(element = Utilities.WIFI.ordinal + 1)
                }
                R.id.computerChip->{
                    tmp.add(element = Utilities.COMPUTER.ordinal + 1)
                }
                R.id.smartTVChip->{
                    tmp.add(element = Utilities.TV.ordinal + 1)
                }
                R.id.bathTubChip->{
                    tmp.add(element = Utilities.BATHTUB.ordinal + 1)
                }
                R.id.parkingLotChip->{
                    tmp.add(element = Utilities.PARKING_LOT.ordinal + 1)
                }
                R.id.airConditionerChip->{
                    tmp.add(element = Utilities.AIR_CONDITIONER.ordinal + 1)
                }
                R.id.washingMachineChip->{
                    tmp.add(element = Utilities.WASHING_MACHINE.ordinal + 1)
                }
                R.id.fridgeChip->{
                    tmp.add(element = Utilities.FRIDGE.ordinal+ 1)
                }
                R.id.poolChip->{
                    tmp.add(element = Utilities.POOL.ordinal+ 1)
                }
            }
        }
        viewModel.utilities.postValue(tmp)
    }

    private fun updateRulesList(){
        val tmp = mutableListOf<Int>()
        for(id in rulesList){
            when(id){
                R.id.noSmokingChip->{
                    tmp.add(element = Rules.NO_SMOKING.ordinal+ 1)
                }
                R.id.noPetChip->{
                    tmp.add(element = Rules.NO_PET.ordinal + 1)
                }
            }
        }
        viewModel.rules.postValue(tmp)
    }
}