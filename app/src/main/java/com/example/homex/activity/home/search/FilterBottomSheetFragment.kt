package com.example.homex.activity.home.search

import android.app.Dialog
import android.graphics.Color
import android.graphics.drawable.ColorDrawable
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.view.WindowManager
import android.widget.CompoundButton
import android.widget.FrameLayout
import androidx.appcompat.widget.AppCompatCheckBox
import androidx.appcompat.widget.AppCompatRadioButton
import androidx.core.content.ContextCompat
import androidx.core.text.HtmlCompat
import androidx.navigation.fragment.findNavController
import androidx.navigation.fragment.navArgs
import com.example.homex.R
import com.example.homex.app.FILTER
import com.example.homex.databinding.FragmentFilterBottomSheetBinding
import com.example.homex.extension.Utilities
import com.google.android.material.bottomsheet.BottomSheetBehavior
import com.google.android.material.bottomsheet.BottomSheetDialog
import com.google.android.material.bottomsheet.BottomSheetDialogFragment
import com.google.android.material.slider.RangeSlider
import com.homex.core.model.Filter


class FilterBottomSheetFragment : BottomSheetDialogFragment() {
    private lateinit var binding: FragmentFilterBottomSheetBinding
    private lateinit var previousChecked: AppCompatRadioButton
    private var utilList = arrayListOf<AppCompatCheckBox>()
    private var priceList : List<Float> = listOf()
    private val navArgs: FilterBottomSheetFragmentArgs by navArgs()

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        dialog?.window?.addFlags(WindowManager.LayoutParams.FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS)
        dialog?.window?.navigationBarColor = ContextCompat.getColor(requireContext(), R.color.white)

        binding = FragmentFilterBottomSheetBinding.inflate(layoutInflater)

        binding.deleteFilterTxt.text = HtmlCompat.fromHtml(resources.getString(R.string.delete_filter), HtmlCompat.FROM_HTML_MODE_LEGACY)

        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        val f = navArgs.filter
        if (f != null){
            when(f.option){
                0 -> {
                    binding.closestRB.isChecked = true
                    previousChecked = binding.closestRB
                }
                1 -> {
                    binding.ratingRB.isChecked = true
                    previousChecked = binding.ratingRB
                }
                2 -> {
                    binding.lowToHighRB.isChecked = true
                    previousChecked = binding.lowToHighRB
                }
                3 -> {
                    binding.highToLowRB.isChecked = true
                    previousChecked = binding.highToLowRB
                }
            }
            val values = arrayListOf(f.priceStart.toFloat(), f.priceEnd.toFloat())
            binding.priceRangeSlider.values = values
            priceList = values
            binding.minPriceTV.text = getString(R.string.points, f.priceStart)
            binding.maxPriceTV.text = getString(R.string.points, f.priceEnd)

            for (util in f.utils){
                when(util){
                    1 -> {
                        binding.wifiCB.isChecked = true
                        utilList.add(binding.wifiCB)
                    }
                    2 -> {
                        binding.computerCB.isChecked = true
                        utilList.add(binding.computerCB)
                    }
                    3 -> {
                        binding.smartTVCB.isChecked = true
                        utilList.add(binding.smartTVCB)
                    }
                    4 -> {
                        binding.bathTubCB.isChecked = true
                        utilList.add(binding.bathTubCB)
                    }
                    5 -> {
                        binding.parkingLotCB.isChecked = true
                        utilList.add(binding.parkingLotCB)
                    }
                    6 -> {
                        binding.airConditionerCB.isChecked = true
                        utilList.add(binding.airConditionerCB)
                    }
                    7 -> {
                        binding.washingMachineCB.isChecked = true
                        utilList.add(binding.washingMachineCB)
                    }
                    8 -> {
                        binding.fridgeCB.isChecked = true
                        utilList.add(binding.fridgeCB)
                    }
                    9 -> {
                        binding.parkingLotCB.isChecked = true
                        utilList.add(binding.parkingLotCB)
                    }
                }
            }
        }else{
            previousChecked = binding.closestRB
            previousChecked.isChecked = true
        }
        binding.applyButton.setOnClickListener {
            var option = 0
            when(previousChecked.id){
                R.id.closestRB->{
                    option = 0
                }
                R.id.ratingRB->{
                    option = 1
                }
                R.id.lowToHighRB->{
                    option = 2
                }
                R.id.highToLowRB->{
                    option = 3
                }
            }
            var priceStart = 100
            var priceEnd = 1000
            if (priceList.size >= 2){
                priceStart = priceList[0].toInt()
                priceEnd = priceList[1].toInt()
            }
            val utils = arrayListOf<Int>()
            for (util in utilList){
                when(util.id){
                    R.id.wifiCB->{
                        utils.add(Utilities.WIFI.ordinal + 1)
                    }
                    R.id.computerCB->{
                        utils.add(Utilities.COMPUTER.ordinal + 1)
                    }
                    R.id.smartTVCB->{
                        utils.add(Utilities.TV.ordinal + 1)
                    }
                    R.id.bathTubCB->{
                        utils.add(Utilities.BATHTUB.ordinal + 1)
                    }
                    R.id.parkingLotCB->{
                        utils.add(Utilities.PARKING_LOT.ordinal + 1)
                    }
                    R.id.airConditionerCB->{
                        utils.add(Utilities.AIR_CONDITIONER.ordinal + 1)
                    }
                    R.id.washingMachineCB->{
                        utils.add(Utilities.WASHING_MACHINE.ordinal + 1)
                    }
                    R.id.fridgeCB->{
                        utils.add(Utilities.FRIDGE.ordinal + 1)
                    }
                    R.id.poolCB->{
                        utils.add(Utilities.POOL.ordinal + 1)
                    }
                }
            }
            val filter = Filter(option, priceStart, priceEnd, utils)
            findNavController().previousBackStackEntry?.savedStateHandle?.set(FILTER, filter)
            findNavController().popBackStack()
        }
        binding.closeBtn.setOnClickListener {
            dismiss()
        }

        binding.closestRB.setOnCheckedChangeListener { compoundButton, b ->
            if (b){
                updateRadioButton(compoundButton)
            }
        }

        binding.ratingRB.setOnCheckedChangeListener { compoundButton, b ->
            if (b){
                updateRadioButton(compoundButton)
            }
        }

        binding.highToLowRB.setOnCheckedChangeListener { compoundButton, b ->
            if (b){
                updateRadioButton(compoundButton)
            }
        }

        binding.lowToHighRB.setOnCheckedChangeListener { compoundButton, b ->
            if (b){
                updateRadioButton(compoundButton)
            }
        }

        binding.highToLowLayout.setOnClickListener {
            binding.highToLowRB.isChecked = true
        }
        binding.lowToHighLayout.setOnClickListener {
            binding.lowToHighRB.isChecked = true
        }
        binding.closestLayout.setOnClickListener {
            binding.closestRB.isChecked = true
        }
        binding.ratingLayout.setOnClickListener {
            binding.ratingRB.isChecked = true
        }

        binding.wifiCB.setOnCheckedChangeListener { compoundButton, b ->
            updateUtilCheckBox(compoundButton, b)
        }
        binding.computerCB.setOnCheckedChangeListener { compoundButton, b ->
            updateUtilCheckBox(compoundButton, b)
        }
        binding.smartTVCB.setOnCheckedChangeListener { compoundButton, b ->
            updateUtilCheckBox(compoundButton, b)
        }
        binding.bathTubCB.setOnCheckedChangeListener { compoundButton, b ->
            updateUtilCheckBox(compoundButton, b)
        }
        binding.parkingLotCB.setOnCheckedChangeListener { compoundButton, b ->
            updateUtilCheckBox(compoundButton, b)
        }
        binding.airConditionerCB.setOnCheckedChangeListener { compoundButton, b ->
            updateUtilCheckBox(compoundButton, b)
        }
        binding.washingMachineCB.setOnCheckedChangeListener { compoundButton, b ->
            updateUtilCheckBox(compoundButton, b)
        }
        binding.fridgeCB.setOnCheckedChangeListener { compoundButton, b ->
            updateUtilCheckBox(compoundButton, b)
        }
        binding.poolCB.setOnCheckedChangeListener { compoundButton, b ->
            updateUtilCheckBox(compoundButton, b)
        }

        binding.wifiLayout.setOnClickListener {
            binding.wifiCB.isChecked = !binding.wifiCB.isChecked
        }
        binding.computerLayout.setOnClickListener {
            binding.computerCB.isChecked = !binding.computerCB.isChecked
        }
        binding.smartTVLayout.setOnClickListener {
            binding.smartTVCB.isChecked = !binding.smartTVCB.isChecked
        }
        binding.bathTubLayout.setOnClickListener {
            binding.bathTubCB.isChecked = !binding.bathTubCB.isChecked
        }
        binding.parkingLotLayout.setOnClickListener {
            binding.parkingLotCB.isChecked = !binding.parkingLotCB.isChecked
        }
        binding.airConditionerLayout.setOnClickListener {
            binding.airConditionerCB.isChecked = !binding.airConditionerCB.isChecked
        }
        binding.washingMachineLayout.setOnClickListener {
            binding.washingMachineCB.isChecked = !binding.washingMachineCB.isChecked
        }
        binding.fridgeLayout.setOnClickListener {
            binding.fridgeCB.isChecked = !binding.fridgeCB.isChecked
        }
        binding.poolLayout.setOnClickListener {
            binding.poolCB.isChecked = !binding.poolCB.isChecked
        }

        binding.priceRangeSlider.addOnSliderTouchListener(object : RangeSlider.OnSliderTouchListener {
            override fun onStartTrackingTouch(slider: RangeSlider) {
                // Responds to when slider's touch event is being started
            }

            override fun onStopTrackingTouch(slider: RangeSlider) {
                // Responds to when slider's touch event is being stopped
                priceList = slider.values
                binding.minPriceTV.text = getString(R.string.points, slider.values[0].toInt())
                binding.maxPriceTV.text = getString(R.string.points, slider.values[1].toInt())
            }
        })

        binding.priceRangeSlider.addOnChangeListener { rangeSlider, _, _ ->
            // Responds to when slider's value is changed
            priceList = rangeSlider.values
            binding.minPriceTV.text = getString(R.string.points, rangeSlider.values[0].toInt())
            binding.maxPriceTV.text = getString(R.string.points, rangeSlider.values[1].toInt())
        }

        binding.deleteFilterTxt.setOnClickListener {
            deleteFilter()
        }
    }

    private fun deleteFilter(){
        binding.closestRB.isChecked = true
        binding.priceRangeSlider.values = listOf(100f, 1000f)
        val tmp = arrayListOf<AppCompatCheckBox>()
        tmp.addAll(utilList)
        utilList.clear()
        val iterator: MutableIterator<AppCompatCheckBox> = tmp.iterator()
        while(iterator.hasNext()){
            val box = iterator.next()
            box.isChecked = false
        }
    }

    private fun updateUtilCheckBox(compoundButton: CompoundButton, b: Boolean){
        if(b){
            for(box in utilList){
                if(compoundButton == box){
                    return
                }
            }
            utilList.add(compoundButton as AppCompatCheckBox)
        }
        else{
            for (box in utilList){
                if(compoundButton == box){
                    utilList.remove(box)
                    return
                }
            }
        }
    }

    private fun updateRadioButton(compoundButton: CompoundButton){
        previousChecked.isChecked = false
        previousChecked = compoundButton as AppCompatRadioButton
    }


    override fun onCreateDialog(savedInstanceState: Bundle?): Dialog {
        val dialog : BottomSheetDialog = super.onCreateDialog(savedInstanceState) as BottomSheetDialog
        dialog.setOnShowListener {
            val d: BottomSheetDialog = it as BottomSheetDialog
            val bottomSheet : FrameLayout? =
                d.findViewById(com.google.android.material.R.id.design_bottom_sheet)
            bottomSheet?.apply {
                BottomSheetBehavior.from(bottomSheet).state = BottomSheetBehavior.STATE_EXPANDED
                dialog.window?.setLayout(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.MATCH_PARENT)
                dialog.window?.setBackgroundDrawable(ColorDrawable(Color.TRANSPARENT))
            }

        }
        return dialog
    }
}