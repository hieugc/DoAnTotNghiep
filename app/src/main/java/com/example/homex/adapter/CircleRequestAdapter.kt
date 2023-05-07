package com.example.homex.adapter

import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.example.homex.R
import com.example.homex.databinding.ItemCircleRequestBinding
import com.example.homex.extension.dpToPx
import com.homex.core.model.response.CircleRequest

class CircleRequestAdapter(
    val requestList: ArrayList<CircleRequest>? = arrayListOf(),
    val onClick: (CircleRequest) -> Unit,
    val btnClick: (CircleRequest) -> Unit
) : RecyclerView.Adapter<CircleRequestAdapter.CircleRequestViewHolder>() {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): CircleRequestViewHolder {
        return CircleRequestViewHolder(
            ItemCircleRequestBinding.bind(
                LayoutInflater.from(parent.context).inflate(
                    R.layout.item_circle_request, parent, false
                )
            )
        )
    }

    override fun onBindViewHolder(holder: CircleRequestViewHolder, position: Int) {
        val item = requestList?.get(position)
        holder.binding.request = item
        if (position == requestList?.size!! - 1) {
            val lastParams = holder.itemView.layoutParams as ViewGroup.MarginLayoutParams
            lastParams.bottomMargin = 16f.dpToPx(holder.itemView.context)
            holder.itemView.requestLayout()
        }
        holder.binding.root.setOnClickListener {
            item?.let(onClick)
        }
        holder.binding.btnRate.setOnClickListener {
            item?.let(btnClick)
        }
    }

    override fun getItemCount(): Int {
        return requestList?.size ?: 0
    }

    class CircleRequestViewHolder(val binding: ItemCircleRequestBinding) :
        RecyclerView.ViewHolder(binding.root)
}