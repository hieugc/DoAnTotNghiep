package com.example.homex.adapter

import android.util.Log
import android.view.LayoutInflater
import android.view.ViewGroup
import android.widget.GridView
import androidx.recyclerview.widget.RecyclerView
import com.example.homex.R
import com.example.homex.databinding.MonthGridItemBinding
import com.example.homex.extension.longToDate
import com.homex.core.model.CalendarDate
import java.util.*


class CalendarAdapter(val monthList: ArrayList<Date> = arrayListOf(), var selectedDates: Pair<CalendarDate?, CalendarDate?> = Pair(null, null), val onClick: (CalendarDate)->Unit): RecyclerView.Adapter<CalendarAdapter.MonthViewHolder>() {
    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): MonthViewHolder {
        return MonthViewHolder(
            MonthGridItemBinding.bind(
                LayoutInflater.from(parent.context).inflate(
                    R.layout.month_grid_item, parent, false
                )
            )
        )
    }

    override fun onBindViewHolder(holder: MonthViewHolder, position: Int) {
        val date = monthList[position]

        val calendar = Calendar.getInstance()
        calendar.time = date
        Log.e("year", "${calendar.get(Calendar.YEAR)}")
        Log.e("month", "${calendar.get(Calendar.MONTH)}")
        Log.e("dayOfMonth", "${calendar.get(Calendar.DAY_OF_MONTH)}")
        Log.e("dayOfWeek", "${calendar.get(Calendar.DAY_OF_WEEK)}")
        Log.e("maxDay", "${calendar.getActualMaximum(Calendar.DATE)}")
        holder.binding.monthTV.text = "Tháng ${calendar.get(Calendar.MONTH) + 1}, ${calendar.get(Calendar.YEAR)}"
        val arrayList = arrayListOf<CalendarDate>()
        val daysInMonth = calendar.getActualMaximum(Calendar.DATE)
        calendar.set(Calendar.DAY_OF_MONTH, 1)
        val dayOfWeek = calendar.get(Calendar.DAY_OF_WEEK) - 2
        Log.e("dayOfWeek", "$dayOfWeek")
        Log.e("daysInMonth", "$daysInMonth")


        for(i in 1..42){
            if(i <= dayOfWeek || i > daysInMonth + dayOfWeek){
                arrayList.add(CalendarDate(null, ""))
            }else{
                calendar.set(Calendar.DAY_OF_MONTH, i-dayOfWeek)
                arrayList.add(CalendarDate(calendar.time ,(i - dayOfWeek).toString()))
            }
        }

        (holder.binding.monthDateGridView as GridView).adapter = MonthGridAdapter(arrayList, selectedDates){
            val cal = Calendar.getInstance()
            cal.clear()
            cal.set(Calendar.YEAR, calendar.get(Calendar.YEAR))
            cal.set(Calendar.MONTH, calendar.get(Calendar.MONTH))
            Log.e("click", "${it.dateOfMonth}/${calendar.get(Calendar.MONTH) + 1}/${calendar.get(Calendar.YEAR)}")
            it.time?.time?.longToDate()?.let { it1 -> Log.e("convert", it1) }
            onClick.invoke(it)

        }
    }

    override fun getItemCount(): Int {
        return  monthList.size
    }

    class MonthViewHolder(val binding: MonthGridItemBinding): RecyclerView.ViewHolder(binding.root)
}