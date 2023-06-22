package com.example.homex.activity.home

import android.os.Handler
import android.os.Looper
import android.widget.ArrayAdapter
import androidx.core.widget.addTextChangedListener
import androidx.navigation.fragment.findNavController
import androidx.navigation.fragment.navArgs
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.homex.R
import com.example.homex.adapter.CalendarAdapter
import com.example.homex.databinding.FragmentBottomSheetChangeDateBinding
import com.example.homex.extension.MILLIS_IN_A_DAY
import com.example.homex.extension.betweenDays
import com.example.homex.extension.convertIso8601ToLong
import com.example.homex.extension.disable
import com.example.homex.extension.enable
import com.example.homex.extension.gone
import com.example.homex.extension.longToDate
import com.example.homex.extension.visible
import com.example.homex.utils.CustomBottomSheet
import com.homex.core.model.CalendarDate
import java.util.Calendar
import java.util.Date


class BottomSheetChangeDateFragment : CustomBottomSheet<FragmentBottomSheetChangeDateBinding>() {
    override val layoutId: Int
        get() = R.layout.fragment_bottom_sheet_change_date
    private lateinit var adapter: CalendarAdapter
    private var loading = true
    private var end = false
    private var selectedDates: Pair<CalendarDate?, CalendarDate?> = Pair(null, null)
    private var numberOfPeople = 1
    private val args: BottomSheetChangeDateFragmentArgs by navArgs()

    override fun setView() {
        numberOfPeople = if (args.numberOfPeople != 0){
            binding.pickPeopleLayout.visible()
            binding.edtPeople.setText(args.numberOfPeople.toString())
            args.numberOfPeople
        }else{
            binding.pickPeopleLayout.gone()
            0
        }

        selectedDates = Pair(args.startDate, args.endDate)
        if (args.startDate != null && args.endDate != null){
            binding.applyButton.enable()
            val first = selectedDates.first?.time?.time?.longToDate()
            val second = selectedDates.second?.time?.time?.longToDate()
            binding.fromDateTV.text = first
            binding.toDateTV.text = second
            binding.numberOfDayTV.text = first.betweenDays(second).toString()
        }
        else
            binding.applyButton.disable()

        val dayOfWeekList =
            listOf(
            "T2", "T3", "T4", "T5", "T6", "T7", "CN"
        )

        val arrayAdapter = ArrayAdapter(requireContext(), R.layout.date_cell_item, dayOfWeekList)

        binding.gridLayout.adapter = arrayAdapter

        val calendar = Calendar.getInstance()

        val arrayList = arrayListOf<Date>()
        arrayList.add(calendar.time)
        calendar.add(Calendar.MONTH, 1)
        arrayList.add(calendar.time)
        calendar.add(Calendar.MONTH, 1)
        arrayList.add(calendar.time)
        calendar.add(Calendar.MONTH, 1)
        arrayList.add(calendar.time)

        val invalid = args.inValidRangeDates?.toMutableList()?: mutableListOf()

        adapter = CalendarAdapter(
            arrayList,
            selectedDates,
            invalid
        ){
            date->
            if(selectedDates.first == null ||  date.time!! <= selectedDates.first?.time){
                selectedDates = Pair(date, null)
                binding.applyButton.disable()
                val first = selectedDates.first?.time?.time?.longToDate()
                binding.fromDateTV.text = first
                binding.toDateTV.text = ""
                binding.numberOfDayTV.text = ""
            }else if(selectedDates.second == null){
                var valid = false
                for (day in invalid){
                    val s = date.time?.time
                    val f = selectedDates.first?.time?.time
                    if (s != null && f != null){
                        val start = day.startDate?.convertIso8601ToLong()
                        val end = day.endDate?.convertIso8601ToLong()
                        if (start != null && end != null && s > start && s < end){
                            valid = true
                        }
                        if (start != null && end != null && f > start && f < end){
                            valid = true
                        }
                        if (start != null && end != null && f <  start && s > end){
                            valid = true
                        }
                        if (valid){
                            selectedDates = Pair(date, null)
                            binding.applyButton.disable()
                            val first = selectedDates.first?.time?.time?.longToDate()
                            binding.fromDateTV.text = first
                            binding.toDateTV.text = ""
                            binding.numberOfDayTV.text = ""
                            adapter.selectedDates = selectedDates
                            adapter.notifyDataSetChanged()
                            return@CalendarAdapter
                        }
                    }
                }
                val f = selectedDates.first
                selectedDates = Pair(f, date)
                binding.applyButton.enable()
                val first = selectedDates.first?.time?.time?.longToDate()
                val second = selectedDates.second?.time?.time?.longToDate()
                binding.fromDateTV.text = first
                binding.toDateTV.text = second
                binding.numberOfDayTV.text = first.betweenDays(second).toString()
            }else{
                selectedDates = Pair(date, null)
                binding.applyButton.disable()
                val first = selectedDates.first?.time?.time?.longToDate()
                binding.fromDateTV.text = first
                binding.toDateTV.text = ""
                binding.numberOfDayTV.text = ""
            }
            adapter.selectedDates = selectedDates
            adapter.notifyDataSetChanged()
        }
        binding.calendarRecView.adapter = adapter
        val layoutManager = LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.calendarRecView.layoutManager = layoutManager

        binding.calendarRecView.addOnScrollListener(object : RecyclerView.OnScrollListener(){
            private var visibleItemCount: Int = 0
            private var totalItemCount: Int = 0
            private var pastVisibleItems: Int = 0
            override fun onScrolled(recyclerView: RecyclerView, dx: Int, dy: Int) {
                if(dy > 0){
                    //Scroll down
                    visibleItemCount = layoutManager.childCount
                    totalItemCount = layoutManager.itemCount
                    pastVisibleItems = layoutManager.findFirstVisibleItemPosition()

                    if(loading)
                    {
                        if(!end)
                        {
                            if((visibleItemCount + pastVisibleItems) >= totalItemCount)
                            {
                                loading = false
                                // Fetch new data
                                val c = Calendar.getInstance()
                                c.add(Calendar.YEAR, 1)
                                for (i in 1..3){
                                    if (calendar.time.time >= c.time.time + MILLIS_IN_A_DAY)
                                    {
                                        end = true
                                        break
                                    }
                                    calendar.add(Calendar.MONTH, 1)
                                    adapter.monthList.add(calendar.time)
                                }
                                adapter.notifyDataSetChanged()
                                Handler(Looper.getMainLooper()).post {
                                    loading = true
                                }
                            }
                        }
                    }
                }
            }
        })
    }

    override fun setEvent() {
        binding.edtPeople.addTextChangedListener {
            numberOfPeople = if (it?.toString() == "") {
                binding.edtPeople.setText("1")
                1
            } else if(it?.toString()?.toInt() == 0){
                binding.edtPeople.setText("1")
                1
            }else{
                it?.toString()?.toInt()?:1
            }
        }

        binding.addPeople.setOnClickListener {
            val num = binding.edtPeople.text.toString().toInt()
            binding.edtPeople.setText(getString(R.string.int_input_string, num + 1))
        }
        binding.minusPeople.setOnClickListener {
            val num = binding.edtPeople.text.toString().toInt()
            if(num == 0) return@setOnClickListener
            binding.edtPeople.setText(getString(R.string.int_input_string, num - 1))
        }
        binding.closeBtn.setOnClickListener {
            dismiss()
        }

        binding.applyButton.setOnClickListener {
            findNavController().previousBackStackEntry?.savedStateHandle?.set("DATE", selectedDates)
            if (numberOfPeople != 0)
                findNavController().previousBackStackEntry?.savedStateHandle?.set("NOP", numberOfPeople)
            dialog?.dismiss()
        }
    }
}