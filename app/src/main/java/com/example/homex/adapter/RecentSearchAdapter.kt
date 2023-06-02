package com.example.homex.adapter

import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.example.homex.R
import com.example.homex.databinding.RecentSearchHomeItemBinding
import com.example.homex.databinding.RecentSearchLocationItemBinding
import com.example.homex.extension.gone
import com.example.homex.extension.visible
import com.homex.core.model.LocationSuggestion

private const val LOCATION_ITEM = 1
private const val HOME_ITEM = 2

class RecentSearchAdapter(private val searchList: ArrayList<LocationSuggestion>?, val recentSearch: Boolean = true, val onClick: (LocationSuggestion)->Unit, val deleteOnClick: (Int)->Unit): RecyclerView.Adapter<RecyclerView.ViewHolder>() {
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
        val item = searchList?.get(position)
        if (holder.itemViewType == HOME_ITEM)
        {
            val tmp = holder as RecentSearchHomeViewHolder
        }
        else{
            val tmp = holder as RecentSearchLocationViewHolder
            val ctx = holder.itemView.context
            if (item?.districtName == null){
                tmp.binding.locationNameTV.text = if (item?.cityName == null) "" else item.cityName
            }else{
                tmp.binding.locationNameTV.text = if (item.cityName == null) ctx.getString(R.string.city_district, item.districtName, "") else ctx.getString(R.string.city_district, item.districtName, item.cityName)
            }
            tmp.binding.root.setOnClickListener {
                item?.let(onClick)
            }
            tmp.binding.closeBtn.setOnClickListener {
                position.let(deleteOnClick)
            }
            if (recentSearch){
                tmp.binding.closeBtn.visible()
            }else
                tmp.binding.closeBtn.gone()
        }
    }

    override fun getItemCount(): Int {
        return  searchList?.size?:0
    }

    override fun getItemViewType(position: Int): Int {
        return LOCATION_ITEM
    }

    class RecentSearchLocationViewHolder(val binding: RecentSearchLocationItemBinding): RecyclerView.ViewHolder(binding.root)
    class RecentSearchHomeViewHolder(val binding: RecentSearchHomeItemBinding): RecyclerView.ViewHolder(binding.root)

}