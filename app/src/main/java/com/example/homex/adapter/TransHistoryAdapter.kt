package com.example.homex.adapter

import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.example.homex.R
import com.example.homex.databinding.ItemTransHistoryBinding
import com.example.homex.extension.dpToPx
import com.homex.core.model.TransHistory

class TransHistoryAdapter(
    val notificationList: ArrayList<TransHistory>?,
    private val listener: EventListener
) : RecyclerView.Adapter<TransHistoryAdapter.TransHistoryViewHolder>() {
    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): TransHistoryViewHolder {
        return TransHistoryViewHolder(
            ItemTransHistoryBinding.bind(
                LayoutInflater.from(parent.context).inflate(
                    R.layout.item_trans_history, parent, false
                )
            )
        )
    }

    override fun onBindViewHolder(holder: TransHistoryViewHolder, position: Int) {
        val item = notificationList?.get(position)
        holder.binding.tvTitle.text = item?.title
        holder.itemView.setOnClickListener {
            listener.OnItemTransClicked()
        }
        holder.binding.btnRate.setOnClickListener {
            listener.onBtnRateClick()
        }
        if(position == notificationList?.size!! - 1){
            val lastParams = holder.itemView.layoutParams as ViewGroup.MarginLayoutParams
            lastParams.bottomMargin = 16f.dpToPx(holder.itemView.context)
            holder.itemView.requestLayout()
        }
    }

    override fun getItemCount(): Int {
        return notificationList?.size ?: 0
    }

    class TransHistoryViewHolder(val binding: ItemTransHistoryBinding) :
        RecyclerView.ViewHolder(binding.root)

    interface EventListener {
        fun onBtnRateClick()
        fun OnItemTransClicked()
    }
}