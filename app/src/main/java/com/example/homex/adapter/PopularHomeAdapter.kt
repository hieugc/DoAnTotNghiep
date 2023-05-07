package com.example.homex.adapter

import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.example.homex.R
import com.example.homex.databinding.PopularHomeItemBinding
import com.example.homex.extension.dpToPx
import com.homex.core.model.Home

class PopularHomeAdapter(val homeList: ArrayList<Home>? = arrayListOf(), val onClick: (Int)->Unit): RecyclerView.Adapter<PopularHomeAdapter.PopularHomeViewHolder>() {
    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): PopularHomeViewHolder {
        return PopularHomeViewHolder(
            PopularHomeItemBinding.bind(LayoutInflater.from(parent.context).inflate(
                R.layout.popular_home_item, parent, false
            ))
        )
    }

    override fun onBindViewHolder(holder: PopularHomeViewHolder, position: Int) {
        val item = homeList?.get(position)
        holder.binding.home = item
        if(position == homeList?.size!! - 1 )
        {
            val lastParams = holder.itemView.layoutParams as ViewGroup.MarginLayoutParams
            lastParams.rightMargin = 16f.dpToPx(holder.itemView.context)
            holder.itemView.requestLayout()
        }
        else if(position == 0){
            val lastParams = holder.itemView.layoutParams as ViewGroup.MarginLayoutParams
            lastParams.leftMargin = 16f.dpToPx(holder.itemView.context)
            holder.itemView.requestLayout()
        }
        holder.binding.root.setOnClickListener {
            item?.id?.let(onClick)
        }
    }

    override fun getItemCount(): Int {
        return homeList?.size?:0
    }

    class PopularHomeViewHolder(val binding: PopularHomeItemBinding): RecyclerView.ViewHolder(binding.root)
}