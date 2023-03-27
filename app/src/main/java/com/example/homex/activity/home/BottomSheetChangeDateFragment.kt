package com.example.homex.activity.home

import android.app.Dialog
import android.graphics.Color
import android.graphics.drawable.ColorDrawable
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.view.WindowManager
import android.widget.ArrayAdapter
import android.widget.FrameLayout
import androidx.core.content.ContextCompat
import androidx.navigation.fragment.findNavController
import androidx.navigation.fragment.navArgs
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.homex.R
import com.example.homex.adapter.CalendarAdapter
import com.example.homex.databinding.FragmentBottomSheetChangeDateBinding
import com.example.homex.extension.*
import com.google.android.material.bottomsheet.BottomSheetBehavior
import com.google.android.material.bottomsheet.BottomSheetDialog
import com.google.android.material.bottomsheet.BottomSheetDialogFragment
import com.homex.core.model.CalendarDate
import java.util.*


class BottomSheetChangeDateFragment : BottomSheetDialogFragment() {
    private lateinit var binding: FragmentBottomSheetChangeDateBinding
    private lateinit var adapter: CalendarAdapter
    private var loading = true
    private var end = false
    private var selectedDates: Pair<CalendarDate?, CalendarDate?> = Pair(null, null)
    private val args: BottomSheetChangeDateFragmentArgs by navArgs()

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        dialog?.window?.addFlags(WindowManager.LayoutParams.FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS)
        dialog?.window?.navigationBarColor = ContextCompat.getColor(requireContext(), R.color.white)

        binding = FragmentBottomSheetChangeDateBinding.inflate(layoutInflater)

        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)


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


        binding.closeBtn.setOnClickListener {
            dismiss()
        }

        val dayOfWeekList =
            listOf(
            "T2", "T3", "T4", "T5", "T6", "T7", "CN"
        )

        val arrayAdapter = ArrayAdapter(requireContext(), R.layout.date_cell_item, dayOfWeekList)

        binding.gridLayout.adapter = arrayAdapter

        val calendar = Calendar.getInstance()

        val arrayList = arrayListOf<Date>()
        arrayList.add(calendar.time)
        Log.e("arraylist", "$arrayList")
        calendar.add(Calendar.MONTH, 1)
        arrayList.add(calendar.time)
        Log.e("arraylist", "$arrayList")
        calendar.add(Calendar.MONTH, 1)
        arrayList.add(calendar.time)
        Log.e("arraylist", "$arrayList")
        calendar.add(Calendar.MONTH, 1)
        arrayList.add(calendar.time)
        Log.e("arraylist", "$arrayList")

        adapter = CalendarAdapter(
            arrayList,
            selectedDates
        ){
            date->
            Log.e("selectedDatesBefore", "$selectedDates")
            if(selectedDates.first == null ||  date.time!! < selectedDates.first?.time ){
                selectedDates = Pair(date, null)
                binding.applyButton.disable()
                val first = selectedDates.first?.time?.time?.longToDate()
                binding.fromDateTV.text = first
                binding.toDateTV.text = ""
                binding.numberOfDayTV.text = ""
            }else if(selectedDates.second == null){
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
            Log.e("selectedDatesAfter", "$selectedDates")
            Log.e("date", "$date")
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
                                Log.e("Loading", "...")
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

        binding.applyButton.setOnClickListener {
            findNavController().previousBackStackEntry?.savedStateHandle?.set("DATE", selectedDates)
            dialog?.dismiss()
        }
    }

    override fun onCreateDialog(savedInstanceState: Bundle?): Dialog {
        val dialog : BottomSheetDialog = super.onCreateDialog(savedInstanceState) as BottomSheetDialog
        dialog.setOnShowListener {
            val d: BottomSheetDialog = it as BottomSheetDialog
            val bottomSheet : FrameLayout? =
                d.findViewById(com.google.android.material.R.id.design_bottom_sheet)
            bottomSheet?.apply {
                BottomSheetBehavior.from(bottomSheet).state = BottomSheetBehavior.STATE_EXPANDED
                BottomSheetBehavior.from(bottomSheet).isDraggable = false
                dialog.window?.setLayout(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.MATCH_PARENT)
                dialog.window?.setBackgroundDrawable(ColorDrawable(Color.TRANSPARENT))
            }

        }
        return dialog
    }
}