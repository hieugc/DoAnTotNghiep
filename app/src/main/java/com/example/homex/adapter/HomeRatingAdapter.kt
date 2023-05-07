package com.example.homex.adapter

import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.example.homex.R
import com.example.homex.databinding.HomeRatingItemBinding
import com.example.homex.extension.dpToPx
import com.homex.core.model.UserRating

class HomeRatingAdapter(var ratingList: ArrayList<UserRating>? = arrayListOf()): RecyclerView.Adapter<HomeRatingAdapter.HomeRatingViewHolder>() {
    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): HomeRatingViewHolder {
        return HomeRatingViewHolder(
            HomeRatingItemBinding.bind(LayoutInflater.from(parent.context).inflate(
                R.layout.home_rating_item, parent, false
            ))
        )
    }

    override fun onBindViewHolder(holder: HomeRatingViewHolder, position: Int) {
        val item = ratingList?.get(position)
        holder.binding.rating = item
        if(position == ratingList?.size!! - 1 )
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

    override fun getItemCount(): Int {
        return ratingList?.size?:0
    }

    class HomeRatingViewHolder(val binding: HomeRatingItemBinding): RecyclerView.ViewHolder(binding.root)
}