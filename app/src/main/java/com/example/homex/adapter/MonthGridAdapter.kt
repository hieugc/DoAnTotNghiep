package com.example.homex.adapter

import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.BaseAdapter
import androidx.core.content.ContextCompat
import androidx.recyclerview.widget.RecyclerView
import com.example.homex.R
import com.example.homex.databinding.DateCellItemBinding
import com.example.homex.extension.convertIso8601ToLong
import com.example.homex.extension.formatIso8601ToFormat
import com.example.homex.extension.longToDate
import com.homex.core.model.CalendarDate
import com.homex.core.model.DateRange
import java.text.SimpleDateFormat
import java.util.*
import kotlin.collections.ArrayList


class MonthGridAdapter(val dayOfMonth: ArrayList<CalendarDate> = arrayListOf(), val selectedDates: Pair<CalendarDate?, CalendarDate?> = Pair(null, null), val invalid: MutableList<DateRange>? = null, val onClick: (CalendarDate)->Unit): BaseAdapter(){
    private lateinit var viewHolder: DateItemViewHolder
    override fun getCount(): Int {
        return dayOfMonth.size
    }

    override fun getItem(position: Int): Any {
        return dayOfMonth[position]
    }

    override fun getItemId(position: Int): Long {
        return position.toLong()
    }

    override fun getView(position: Int, convertView: View?, parent: ViewGroup?): View {
        if(convertView == null){
            val binding = DateCellItemBinding.bind(
                    LayoutInflater.from(parent?.context).inflate(
                        R.layout.date_cell_item, parent, false
                    )
                )
            viewHolder = DateItemViewHolder(binding)
            viewHolder.view = binding.root
            viewHolder.view.tag = viewHolder
        }else{
            viewHolder = (convertView.tag as DateItemViewHolder)
        }

        val item = dayOfMonth[position]

        var isInvalid = false
        if (invalid != null) {
            for (day in invalid){
                if (item.time?.time?.longToDate() == day.startDate?.formatIso8601ToFormat("dd/MM/yyyy")
                    || item.time?.time?.longToDate() == day.endDate?.formatIso8601ToFormat("dd/MM/yyyy")
                ){
                    isInvalid = true
                    break
                }
                val time = item.time
                if (time != null){
                    val start = day.startDate?.convertIso8601ToLong()
                    val end = day.endDate?.convertIso8601ToLong()
                    if (start != null && end != null && time.time > start && time.time < end){
                        isInvalid = true
                        break
                    }
                }
            }
        }

        val date = Date()
        viewHolder.binding.root.text = item.dateOfMonth
        viewHolder.binding.root.setOnClickListener {
            if (item.time?.time?.longToDate() != date.time.longToDate()){
                item.time?.time?.let {
                    if (it < date.time)
                        return@setOnClickListener
                }
            }
            if (isInvalid)
                return@setOnClickListener
            if(item.dateOfMonth != "" && item.time != null)
                onClick.invoke(item)
        }

        item.time?.apply {
            if (isInvalid){
                viewHolder.binding.root.setBackgroundColor(ContextCompat.getColor(
                    viewHolder.itemView.context,
                    R.color.grey_light
                ))
                viewHolder.binding.root.setTextColor(
                    ContextCompat.getColorStateList(
                        viewHolder.itemView.context,
                        R.color.grey_ba
                    )
                )
            }
            else if(this.time.longToDate() == selectedDates.first?.time?.time?.longToDate() ){
                if(selectedDates.second != null){
                    viewHolder.binding.root.background = ContextCompat.getDrawable(viewHolder.itemView.context, R.drawable.start_date_bg)
                    viewHolder.binding.root.setTextColor(
                        ContextCompat.getColorStateList(
                            viewHolder.itemView.context,
                            R.color.white
                        )
                    )
                }
                else {
                    viewHolder.binding.root.background = ContextCompat.getDrawable(
                        viewHolder.itemView.context,
                        R.drawable.selected_date_bg
                    )
                    viewHolder.binding.root.setTextColor(
                        ContextCompat.getColorStateList(
                            viewHolder.itemView.context,
                            R.color.white
                        )
                    )
                }
            }else if(this.time.longToDate() == selectedDates.second?.time?.time?.longToDate()){
                if(selectedDates.second != null){
                    viewHolder.binding.root.background = ContextCompat.getDrawable(viewHolder.itemView.context, R.drawable.end_date_bg)
                    viewHolder.binding.root.setTextColor(
                        ContextCompat.getColorStateList(
                            viewHolder.itemView.context,
                            R.color.white
                        )
                    )
                }else {
                    viewHolder.binding.root.background = ContextCompat.getDrawable(
                        viewHolder.itemView.context,
                        R.drawable.selected_date_bg
                    )
                    viewHolder.binding.root.setTextColor(
                        ContextCompat.getColorStateList(
                            viewHolder.itemView.context,
                            R.color.white
                        )
                    )
                }
            }
            else if(this.time.longToDate() == date.time.longToDate()){
                viewHolder.binding.root.background = ContextCompat.getDrawable(
                    viewHolder.itemView.context,
                    R.drawable.today_bg
                )
                viewHolder.binding.root.setTextColor(
                    ContextCompat.getColorStateList(
                        viewHolder.itemView.context,
                        R.color.black
                    )
                )
            }
            else if (selectedDates.first?.time != null && selectedDates.second?.time != null){
                if(this.time < selectedDates.second?.time?.time!! && this.time > selectedDates.first?.time?.time!!){
                    viewHolder.binding.root.background = ContextCompat.getDrawable(viewHolder.itemView.context, R.drawable.between_date_bg)
                    viewHolder.binding.root.setTextColor(
                        ContextCompat.getColorStateList(
                            viewHolder.itemView.context,
                            R.color.primary
                        )
                    )
                }
            }

            if (this.time.longToDate() != date.time.longToDate() && this.time < date.time){
                viewHolder.binding.root.setBackgroundColor(ContextCompat.getColor(
                    viewHolder.itemView.context,
                    R.color.white
                ))
                viewHolder.binding.root.setTextColor(
                    ContextCompat.getColorStateList(
                        viewHolder.itemView.context,
                        R.color.grey_ba
                    )
                )
            }

        }
        return viewHolder.view
    }

    class DateItemViewHolder(val binding: DateCellItemBinding): RecyclerView.ViewHolder(binding.root){
        lateinit var view: View
    }
}

