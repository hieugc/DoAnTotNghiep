package com.example.homex.adapter

import android.graphics.Color
import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.example.homex.R
import com.example.homex.databinding.ItemPaymentHistoryBinding
import com.example.homex.extension.formatIso8601ToFormat
import com.example.homex.extension.thousandSeparator
import com.homex.core.model.response.PaymentHistory

class PaymentHistoryAdapter : RecyclerView.Adapter<PaymentHistoryAdapter.PaymentHistoryViewHolder>() {

    private val paymentHistory: ArrayList<PaymentHistory> = ArrayList()

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): PaymentHistoryViewHolder {
        return PaymentHistoryViewHolder(
            ItemPaymentHistoryBinding.bind(
                LayoutInflater.from(parent.context).inflate(
                    R.layout.item_payment_history, parent, false
                )
            )
        )
    }

    fun setList(paymentHistory: ArrayList<PaymentHistory>) {
        this.paymentHistory.clear()
        this.paymentHistory.addAll(paymentHistory.reversed())
        notifyDataSetChanged()
    }

    override fun onBindViewHolder(holder: PaymentHistoryViewHolder, position: Int) {
        val item = paymentHistory[position]
        holder.binding.tvContent.text = item.content
        holder.binding.tvDate.text = item.createdDate.formatIso8601ToFormat("HH:mm - dd/MM/yyyy")
        if(item.status){
            holder.binding.tvAmount.text = holder.itemView.context.getString(R.string.minus_point, item.amount.thousandSeparator())
            holder.binding.tvAmount.setTextColor(Color.parseColor("#FF0000"))
        } else {
            holder.binding.tvAmount.text = holder.itemView.context.getString(R.string.add_point, item.amount.thousandSeparator())
            holder.binding.tvAmount.setTextColor(Color.parseColor("#008000"))
        }
    }

    override fun getItemCount(): Int {
        return paymentHistory.size
    }

    class PaymentHistoryViewHolder(val binding: ItemPaymentHistoryBinding) :
        RecyclerView.ViewHolder(binding.root)

}