package com.example.homex.adapter

import android.util.Log
import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.core.content.ContextCompat
import androidx.recyclerview.widget.RecyclerView
import com.bumptech.glide.Glide
import com.example.homex.R
import com.example.homex.databinding.MyHomeItemBinding
import com.example.homex.extension.dpToPx
import com.example.homex.extension.setHomeStatus
import com.homex.core.model.Home
import com.homex.core.model.HomeStatus

class MyHomeAdapter(val homeList: ArrayList<Home>?, val onClick: (Int)->Unit): RecyclerView.Adapter<MyHomeAdapter.MyHomeViewHolder>() {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): MyHomeViewHolder {
        return MyHomeViewHolder(
            MyHomeItemBinding.bind(
                LayoutInflater.from(parent.context).inflate(
                    R.layout.my_home_item, parent, false
                )
            )
        )
    }

    override fun onBindViewHolder(holder: MyHomeViewHolder, position: Int) {
        val item = homeList?.get(position)
        holder.binding.home = item
        holder.binding.root.setOnClickListener {
            item?.id?.let { onClick.invoke(it) }
        }
        if(position == homeList?.size!! - 1 )
        {
            val lastParams = holder.itemView.layoutParams as ViewGroup.MarginLayoutParams
            lastParams.bottomMargin = 80f.dpToPx(holder.itemView.context)
            holder.itemView.requestLayout()
        }
    }

    override fun getItemCount(): Int {
        return homeList?.size?:0
    }

    class MyHomeViewHolder(val binding: MyHomeItemBinding): RecyclerView.ViewHolder(binding.root)
}