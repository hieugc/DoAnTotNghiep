package com.example.homex.adapter

import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.bumptech.glide.Glide
import com.bumptech.glide.load.resource.bitmap.RoundedCorners
import com.bumptech.glide.request.RequestOptions
import com.example.homex.R
import com.example.homex.databinding.PopularLocationItemBinding
import com.example.homex.extension.dpToPx
import com.homex.core.model.Location


class PopularLocationAdapter(var list: ArrayList<Location>? = arrayListOf()): RecyclerView.Adapter<PopularLocationAdapter.PopularLocationViewHolder>() {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): PopularLocationViewHolder {
        return PopularLocationViewHolder(
            PopularLocationItemBinding.bind(LayoutInflater.from(parent.context).inflate(
                R.layout.popular_location_item, parent, false)
            )
        )
    }

    override fun onBindViewHolder(holder: PopularLocationViewHolder, position: Int) {
        val item = list?.get(position)
        holder.binding.locationTxt.text = item?.name ?: ""
        Glide.with(holder.itemView.context)
            .load(item?.imageUrl)
            .apply(RequestOptions.bitmapTransform(RoundedCorners(5f.dpToPx(holder.itemView.context))))
            .into(holder.binding.locationImg)
        if(position == list?.size!! - 1 )
        {
            val lastParams = holder.itemView.layoutParams as ViewGroup.MarginLayoutParams
            lastParams.rightMargin = 16f.dpToPx(holder.itemView.context)
            holder.itemView.requestLayout()
        }
        if(position == 0){
            val lastParams = holder.itemView.layoutParams as ViewGroup.MarginLayoutParams
            lastParams.leftMargin = 16f.dpToPx(holder.itemView.context)
            holder.itemView.requestLayout()
        }
    }

    override fun getItemCount(): Int = list?.size?:0

    class PopularLocationViewHolder(var binding: PopularLocationItemBinding): RecyclerView.ViewHolder(binding.root)
}