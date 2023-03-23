package com.example.homex

import android.os.Bundle
import android.util.Log
import android.view.View
import androidx.navigation.fragment.findNavController
import androidx.navigation.fragment.navArgs
import com.example.homex.activity.home.HomeActivity
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentCreateRequestBinding
import com.example.homex.extension.betweenDays
import com.example.homex.extension.gone
import com.example.homex.extension.longToDate
import com.example.homex.extension.visible
import com.google.android.material.datepicker.CalendarConstraints
import com.google.android.material.datepicker.DateValidatorPointForward
import com.google.android.material.datepicker.MaterialDatePicker
import com.homex.core.model.CalendarDate
import java.util.*


class CreateRequestFragment : BaseFragment<FragmentCreateRequestBinding>() {
    companion object{
        const val MILLIS_IN_A_DAY = 1000 * 60 * 60 * 24
    }
    override val layoutId: Int = R.layout.fragment_create_request
    private lateinit var dateRangePicker: MaterialDatePicker<androidx.core.util.Pair<Long, Long>>
    private lateinit var builder: MaterialDatePicker.Builder<androidx.core.util.Pair<Long, Long>>
    private lateinit var constraintBuilder: CalendarConstraints.Builder
    private var selectedDate: androidx.core.util.Pair<Long, Long> = androidx.core.util.Pair(MaterialDatePicker.todayInUtcMilliseconds(), MaterialDatePicker.todayInUtcMilliseconds() + 7 * MILLIS_IN_A_DAY)
    private var selection: Pair<CalendarDate?, CalendarDate?> = Pair(null, null)
    private val args: CreateRequestFragmentArgs by navArgs()

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showTitleApp = Pair(true, "Tạo yêu cầu"),
            showMenu = false,
            showMessage = false,
            showBoxChatLayout = Pair(false, null),
            showBottomNav = false,
            showLogo = false
        )

        findNavController().currentBackStackEntry?.savedStateHandle?.getLiveData<Pair<CalendarDate?, CalendarDate?>>("DATE")?.observe(viewLifecycleOwner){
            dates->
            val startDate = dates.first?.time?.time?.longToDate()
            val endDate = dates.second?.time?.time?.longToDate()
            binding.fromDateTV.text =  startDate
            binding.toDateTV.text = endDate
            Log.e("betweenDate",  "${startDate.betweenDays(endDate)}")
            selection = dates
        }
    }

    override fun setView() {

        //Date Range Picker
        val calendar = Calendar.getInstance(TimeZone.getDefault())
        calendar.clear()

        val today = MaterialDatePicker.todayInUtcMilliseconds()
        calendar.timeInMillis = today

        calendar.add(Calendar.YEAR, 5)
        val next_year = calendar.timeInMillis

        constraintBuilder = CalendarConstraints.Builder()
        constraintBuilder.setOpenAt(selectedDate.first)
        constraintBuilder.setStart(today)
        constraintBuilder.setEnd(next_year)
        constraintBuilder.setValidator(DateValidatorPointForward.now())

        builder = MaterialDatePicker.Builder.dateRangePicker()
            .setTitleText("Chọn khoảng thời gian trao đổi")
            .setSelection(selectedDate)
            .setCalendarConstraints(constraintBuilder.build())
        dateRangePicker = builder.build()

        if(args.startDate != null && args.endDate != null){
            selection = Pair(
                args.startDate,
                args.endDate
            )
            binding.fromDateTV.text = selection.first?.time?.time?.longToDate()
            binding.toDateTV.text = selection.second?.time?.time?.longToDate()
        }
        else {
            val cal = Calendar.getInstance()
            val first = CalendarDate(cal.time, cal.get(Calendar.DAY_OF_MONTH).toString())
            Log.e("first", cal.get(Calendar.DAY_OF_MONTH).toString())
            cal.add(Calendar.DATE, 7)
            val second = CalendarDate(cal.time, cal.get(Calendar.DAY_OF_MONTH).toString())
            Log.e("second", cal.get(Calendar.DAY_OF_MONTH).toString())
            selection = Pair(
                first, second
            )
            binding.fromDateTV.text = first.time?.time?.longToDate()
            binding.toDateTV.text = second.time?.time?.longToDate()
        }
    }



    override fun setEvent() {
        binding.homeRB.setOnCheckedChangeListener { _, b ->
            if (b){
                binding.pointRB.isChecked = false
                checkUI()
            }
        }

        binding.pointRB.setOnCheckedChangeListener { _, b ->
            if (b){
                binding.homeRB.isChecked = false
                checkUI()
            }
        }

        binding.changeDateBtn.setOnClickListener {
//            dateRangePicker.show(parentFragmentManager, "Chọn ngày trao đổi")
            val action = CreateRequestFragmentDirections.actionCreateRequestFragmentToBottomSheetChangeDateFragment(selection.first, selection.second)
            findNavController().navigate(action)
//            findNavController().navigate(R.id.action_createRequestFragment_to_bottomSheetChangeDateFragment)
        }

        dateRangePicker.addOnPositiveButtonClickListener { sel->
            updatePicker(sel)
        }

        binding.addYourHomeBtn.setOnClickListener {
            findNavController().navigate(R.id.action_createRequestFragment_to_pickYourHomeFragment)
        }
        binding.addTargetHomeBtn.setOnClickListener {
            findNavController().navigate(R.id.action_createRequestFragment_to_pickHomeFragment)
        }
    }

    private fun updatePicker(sel: androidx.core.util.Pair<Long, Long>) {
        binding.fromDateTV.text = sel.first.longToDate()
        binding.toDateTV.text = sel.second.longToDate()
        selectedDate = sel
        constraintBuilder.setOpenAt(sel.first)
        dateRangePicker =
            builder.setSelection(sel).setCalendarConstraints(constraintBuilder.build())
                .build()
        dateRangePicker.addOnPositiveButtonClickListener {
            updatePicker(it)
        }
    }

    private fun checkUI(){
        if(!binding.homeRB.isChecked && binding.pointRB.isChecked){
            binding.appCompatTextView26.text = getString(R.string.point_txt)
            binding.pointLayout.visible()
            binding.addYourHomeBtn.gone()
            binding.yourPointTV.visible()
        }
        else{
            binding.appCompatTextView26.text = getString(R.string.pick_your_home)
            binding.pointLayout.gone()
            binding.yourPointTV.gone()
            binding.addYourHomeBtn.visible()
        }
    }
}