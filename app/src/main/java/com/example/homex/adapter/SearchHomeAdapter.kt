package com.example.homex.adapter

import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.example.homex.R
import com.example.homex.databinding.SearchHomeItemBinding
import com.homex.core.model.Home

class SearchHomeAdapter(var searchList: ArrayList<Home>?, private val onClick: (Home)->Unit): RecyclerView.Adapter<SearchHomeAdapter.SearchHomeViewHolder>() {
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