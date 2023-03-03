package com.example.homex.adapter

import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.example.homex.R
import com.example.homex.databinding.RequestItemBinding
import com.example.homex.extension.dpToPx

class RequestItemAdapter(val requestList: List<String>? = arrayListOf(), val onClick: ()->Unit): RecyclerView.Adapter<RequestItemAdapter.RequestItemViewHolder>() {
    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): RequestItemViewHolder {
        return RequestItemViewHolder(
            RequestItemBinding.bind(
                LayoutInflater.from(parent.context).inflate(
                    R.layout.request_item, parent, false
                )
            )
        )
    }

    override fun onBindViewHolder(holder: RequestItemViewHolder, position: Int) {
        val item = requestList?.get(position)
        holder.binding.tvTitle.text = item
        if(position == requestList?.size!! - 1){
            val lastParams = holder.itemView.layoutParams as ViewGroup.MarginLayoutParams
            lastParams.bottomMargin = 16f.dpToPx(holder.itemView.context)
            holder.itemView.requestLayout()
        }
        holder.binding.root.setOnClickListener {
            onClick.invoke()
        }
        holder.binding.btnRate.setOnClickListener {
            onClick.invoke()
        }
    }

    override fun getItemCount(): Int {
        return requestList?.size?:0
    }

    class RequestItemViewHolder(val binding: RequestItemBinding): RecyclerView.ViewHolder(binding.root)
}