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

class MyHomeAdapter(val homeList: ArrayList<Home>?, val onClick: (Home)->Unit): RecyclerView.Adapter<MyHomeAdapter.MyHomeViewHolder>() {

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
        item?.images?.let {
            if (it.isNotEmpty()){
                Log.e("url", it[0].data.toString())
                Glide.with(holder.itemView.context)
                    .load(it[0].data)
                    .into(holder.binding.homeImg)
            }
        }
        holder.binding.homeName.text = item?.name
        holder.binding.homeLocation.text = item?.location
        holder.binding.homeNOP.text = "${item?.people} người"
        holder.binding.homePriceTV.text = "${item?.price} point/ngày"
        holder.binding.ratingTV.text = "${item?.rating}"
        holder.binding.homeStatus.text = item?.getHomeStatus()
        when(item?.status){
            HomeStatus.VALID.ordinal->{
                holder.binding.homeStatus.setTextColor(ContextCompat.getColor(holder.itemView.context, R.color.green))
            }
            HomeStatus.PENDING.ordinal->{
                holder.binding.homeStatus.setTextColor(ContextCompat.getColor(holder.itemView.context, R.color.orange))
            }
            HomeStatus.DISABLE.ordinal->{
                holder.binding.homeStatus.setTextColor(ContextCompat.getColor(holder.itemView.context, R.color.gray))
            }
            HomeStatus.SWAPPED.ordinal->{
                holder.binding.homeStatus.setTextColor(ContextCompat.getColor(holder.itemView.context, R.color.yellow))
            }
        }
        holder.binding.root.setOnClickListener {
            if (item != null) {
                onClick.invoke(item)
            }
        }
        if(position == homeList?.size!! - 1 )
        {
            val lastParams = holder.itemView.layoutParams as ViewGroup.MarginLayoutParams
            lastParams.bottomMargin = 80f.dpToPx(holder.itemView.context)
            holder.itemView.requestLayout()
        }else{
            val lastParams = holder.itemView.layoutParams as ViewGroup.MarginLayoutParams
            lastParams.bottomMargin = 16f.dpToPx(holder.itemView.context)
            holder.itemView.requestLayout()
        }
    }

    override fun getItemCount(): Int {
        return homeList?.size?:0
    }

    class MyHomeViewHolder(val binding: MyHomeItemBinding): RecyclerView.ViewHolder(binding.root)
}