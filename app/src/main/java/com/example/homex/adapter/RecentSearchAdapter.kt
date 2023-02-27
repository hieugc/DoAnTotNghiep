package com.example.homex.adapter

import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.example.homex.R
import com.example.homex.databinding.RecentSearchHomeItemBinding
import com.example.homex.databinding.RecentSearchLocationItemBinding

private const val LOCATION_ITEM = 1
private const val HOME_ITEM = 2

class RecentSearchAdapter(val searchList: ArrayList<String>?): RecyclerView.Adapter<RecyclerView.ViewHolder>() {
    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): RecyclerView.ViewHolder {
        if (viewType == LOCATION_ITEM){
            return RecentSearchLocationViewHolder(RecentSearchLocationItemBinding.bind(
                LayoutInflater.from(parent.context).inflate(R.layout.recent_search_location_item, parent, false)
            ))
        }
        return RecentSearchHomeViewHolder(RecentSearchHomeItemBinding.bind(
            LayoutInflater.from(parent.context).inflate(R.layout.recent_search_home_item, parent, false)
        ))
    }

    override fun onBindViewHolder(holder: RecyclerView.ViewHolder, position: Int) {
        if (holder.itemViewType == HOME_ITEM)
        {
            val tmp = holder as RecentSearchHomeViewHolder
            tmp.binding.homeNameTV.text = searchList?.get(position) ?: ""
        }
        else{
            val tmp = holder as RecentSearchLocationViewHolder
            tmp.binding.locationNameTV.text = searchList?.get(position) ?: ""
        }
    }

    override fun getItemCount(): Int {
        return  searchList?.size?:0
    }

    override fun getItemViewType(position: Int): Int {
        if (position > 2)
            return HOME_ITEM
        return LOCATION_ITEM
    }

    class RecentSearchLocationViewHolder(val binding: RecentSearchLocationItemBinding): RecyclerView.ViewHolder(binding.root)
    class RecentSearchHomeViewHolder(val binding: RecentSearchHomeItemBinding): RecyclerView.ViewHolder(binding.root)

}