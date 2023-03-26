package com.example.homex.adapter

import android.util.Log
import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.bumptech.glide.Glide
import com.example.homex.R
import com.example.homex.databinding.SearchHomeItemBinding
import com.example.homex.extension.dpToPx
import com.homex.core.model.Home

class SearchHomeAdapter(val searchList: ArrayList<Home>?, private val onClick: (Home)->Unit): RecyclerView.Adapter<SearchHomeAdapter.SearchHomeViewHolder>() {
    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): SearchHomeViewHolder {
        return SearchHomeViewHolder(
            SearchHomeItemBinding.bind(LayoutInflater.from(parent.context).inflate(
                R.layout.search_home_item, parent, false
            ))
        )
    }

    override fun onBindViewHolder(holder: SearchHomeViewHolder, position: Int) {
        val item = searchList?.get(position)
        holder.binding.home = item
        if(position == searchList?.size!! - 1 )
        {
            val lastParams = holder.itemView.layoutParams as ViewGroup.MarginLayoutParams
            lastParams.bottomMargin = 80f.dpToPx(holder.itemView.context)
            holder.itemView.requestLayout()
        }
        holder.binding.root.setOnClickListener {
            if (item != null) {
                onClick.invoke(item)
            }
        }
    }

    override fun getItemCount(): Int {
        return searchList?.size?:0
    }

    class SearchHomeViewHolder(val binding: SearchHomeItemBinding): RecyclerView.ViewHolder(binding.root)
}