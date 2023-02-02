package com.example.homex.adapter

import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.example.homex.R
import com.example.homex.databinding.PopularLocationItemBinding
import com.example.homex.extension.dpToPx


class PopularLocationAdapter(var list: ArrayList<String>? = arrayListOf()): RecyclerView.Adapter<PopularLocationAdapter.PopularLocationViewHolder>() {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): PopularLocationViewHolder {
        return PopularLocationViewHolder(
            PopularLocationItemBinding.bind(LayoutInflater.from(parent.context).inflate(
                R.layout.popular_location_item, parent, false)
            )
        )
    }

    override fun onBindViewHolder(holder: PopularLocationViewHolder, position: Int) {
//        val params = holder.itemView.layoutParams as ViewGroup.LayoutParams
//        val displayMetrics = DisplayMetrics()
//        (holder.itemView.context as Activity).windowManager.defaultDisplay.getMetrics(displayMetrics)
//        val width = displayMetrics.widthPixels
//        params.width = (width * 0.9).toInt()
        holder.binding.locationTxt.text = list?.get(position) ?: ""
        if(position == list?.size!! - 1 )
        {
            val lastParams = holder.itemView.layoutParams as ViewGroup.MarginLayoutParams
            lastParams.rightMargin = 16f.dpToPx(holder.itemView.context)
            holder.itemView.requestLayout()
        }
    }

    override fun getItemCount(): Int = list?.size?:0

    class PopularLocationViewHolder(var binding: PopularLocationItemBinding): RecyclerView.ViewHolder(binding.root)
}