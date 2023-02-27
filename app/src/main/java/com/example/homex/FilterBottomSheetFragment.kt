package com.example.homex

import android.app.Dialog
import android.graphics.Color
import android.graphics.drawable.ColorDrawable
import android.os.Bundle
import android.util.Log
import android.view.*
import android.widget.CompoundButton
import android.widget.FrameLayout
import androidx.appcompat.widget.AppCompatCheckBox
import androidx.appcompat.widget.AppCompatRadioButton
import androidx.core.content.ContextCompat
import androidx.core.text.HtmlCompat
import com.example.homex.databinding.FragmentFilterBottomSheetBinding
import com.google.android.material.bottomsheet.BottomSheetBehavior
import com.google.android.material.bottomsheet.BottomSheetDialog
import com.google.android.material.bottomsheet.BottomSheetDialogFragment
import com.google.android.material.slider.RangeSlider


class FilterBottomSheetFragment : BottomSheetDialogFragment() {
    private lateinit var binding: FragmentFilterBottomSheetBinding
    private lateinit var previousChecked: AppCompatRadioButton
    private var utilList = arrayListOf<AppCompatCheckBox>()
    private var priceList : List<Float> = listOf()

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        dialog?.window?.addFlags(WindowManager.LayoutParams.FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS)
        dialog?.window?.navigationBarColor = ContextCompat.getColor(requireContext(), R.color.white)

        binding = FragmentFilterBottomSheetBinding.inflate(layoutInflater)

        binding.deleteFilterTxt.text = HtmlCompat.fromHtml(resources.getString(R.string.delete_filter), HtmlCompat.FROM_HTML_MODE_LEGACY)

        previousChecked = binding.closestRB

        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
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
            }
        })

        binding.priceRangeSlider.addOnChangeListener { rangeSlider, value, fromUser ->
            // Responds to when slider's value is changed
            Log.e("value", "${rangeSlider.values}")
            binding.minPriceTV.text = "${rangeSlider.values[0].toInt()} Points"
            binding.maxPriceTV.text = "${rangeSlider.values[1].toInt()} Points"
        }

        binding.deleteFilterTxt.setOnClickListener {
            deleteFilter()
        }
    }

    private fun deleteFilter(){
        binding.closestRB.isChecked = true
        binding.priceRangeSlider.values = listOf(100f, 1000f)
        var tmp = arrayListOf<AppCompatCheckBox>()
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