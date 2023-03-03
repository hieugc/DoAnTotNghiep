package com.example.homex.adapter

import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.BaseAdapter
import androidx.core.content.ContextCompat
import androidx.recyclerview.widget.RecyclerView
import com.example.homex.R
import com.example.homex.databinding.DateCellItemBinding
import com.example.homex.extension.longToDate
import com.homex.core.model.CalendarDate
import java.util.*
import kotlin.collections.ArrayList


class MonthGridAdapter(val dayOfMonth: ArrayList<CalendarDate> = arrayListOf(), val selectedDates: Pair<CalendarDate?, CalendarDate?> = Pair(null, null), val onClick: (CalendarDate)->Unit): BaseAdapter(){
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
        viewHolder.binding.root.text = item.dateOfMonth
        viewHolder.binding.root.setOnClickListener {
            if(item.dateOfMonth != "" && item.time != null)
                onClick.invoke(item)
        }

        item.time?.apply {
            val date = Date()
            if(this.time.longToDate() == selectedDates.first?.time?.time?.longToDate() ){
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
            else{
                if (selectedDates.first?.time != null && selectedDates.second?.time != null){
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
            }
        }
        return viewHolder.view
    }

    class DateItemViewHolder(val binding: DateCellItemBinding): RecyclerView.ViewHolder(binding.root){
        lateinit var view: View
    }
}

