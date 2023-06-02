package com.example.homex.adapter

import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.bumptech.glide.Glide
import com.example.homex.R
import com.example.homex.databinding.PopularLocationItemBinding
import com.homex.core.model.Location


class PopularLocationAdapter(var list: ArrayList<Location>? = arrayListOf(), val onClick: (Location)->Unit): RecyclerView.Adapter<PopularLocationAdapter.PopularLocationViewHolder>() {

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
            .into(holder.binding.locationImg)
        holder.binding.root.setOnClickListener {
            item?.let(onClick)
        }
    }

    override fun getItemCount(): Int = list?.size?:0

    class PopularLocationViewHolder(var binding: PopularLocationItemBinding): RecyclerView.ViewHolder(binding.root)
}