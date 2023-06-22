package com.example.homex.adapter

import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.example.homex.R
import com.example.homex.databinding.RequestItemBinding
import com.homex.core.model.response.RequestResponse

class RequestItemAdapter(val requestList: ArrayList<RequestResponse>? = arrayListOf(), val onClick: (Int)->Unit, val btnClick: (RequestResponse)->Unit): RecyclerView.Adapter<RequestItemAdapter.RequestItemViewHolder>() {
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
        holder.binding.request = item
        holder.binding.root.setOnClickListener {
            item?.request?.id?.let(onClick)
        }
        holder.binding.btnRate.setOnClickListener {
            item?.let(btnClick)
        }
    }

    override fun getItemCount(): Int {
        return requestList?.size?:0
    }

    class RequestItemViewHolder(val binding: RequestItemBinding): RecyclerView.ViewHolder(binding.root)
}