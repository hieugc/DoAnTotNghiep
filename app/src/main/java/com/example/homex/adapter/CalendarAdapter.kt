package com.example.homex.adapter

import android.view.LayoutInflater
import android.view.ViewGroup
import android.widget.GridView
import androidx.recyclerview.widget.RecyclerView
import com.example.homex.R
import com.example.homex.databinding.MonthGridItemBinding
import com.homex.core.model.CalendarDate
import com.homex.core.model.DateRange
import java.util.Calendar
import java.util.Date


class CalendarAdapter(val monthList: ArrayList<Date> = arrayListOf(), var selectedDates: Pair<CalendarDate?, CalendarDate?> = Pair(null, null), private val invalid: MutableList<DateRange>? = null, val onClick: (CalendarDate)->Unit): RecyclerView.Adapter<CalendarAdapter.MonthViewHolder>() {
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
        holder.binding.monthTV.text = holder.itemView.context.getString(R.string.month, calendar.get(Calendar.MONTH) + 1, calendar.get(Calendar.YEAR))
        val arrayList = arrayListOf<CalendarDate>()
        val daysInMonth = calendar.getActualMaximum(Calendar.DATE)
        calendar.set(Calendar.DAY_OF_MONTH, 1)
        val dayOfWeek = calendar.get(Calendar.DAY_OF_WEEK) - 2

        for(i in 1..42){
            if(i <= dayOfWeek || i > daysInMonth + dayOfWeek){
                arrayList.add(CalendarDate(null, ""))
            }else{
                calendar.set(Calendar.DAY_OF_MONTH, i-dayOfWeek)
                arrayList.add(CalendarDate(calendar.time ,(i - dayOfWeek).toString()))
            }
        }

        (holder.binding.monthDateGridView as GridView).adapter = MonthGridAdapter(arrayList, selectedDates, invalid){
            val cal = Calendar.getInstance()
            cal.clear()
            cal.set(Calendar.YEAR, calendar.get(Calendar.YEAR))
            cal.set(Calendar.MONTH, calendar.get(Calendar.MONTH))
            onClick.invoke(it)

        }
    }

    override fun getItemCount(): Int {
        return  monthList.size
    }

    class MonthViewHolder(val binding: MonthGridItemBinding): RecyclerView.ViewHolder(binding.root)
}