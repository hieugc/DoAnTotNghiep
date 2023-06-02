package com.example.homex.adapter

import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.example.homex.R
import com.example.homex.databinding.SimilarHomeItemBinding

class SimilarHomeAdapter(private val homeList: ArrayList<String>?): RecyclerView.Adapter<SimilarHomeAdapter.SimilarHomeViewHolder>() {
    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): SimilarHomeViewHolder {
        return SimilarHomeViewHolder(
            SimilarHomeItemBinding.bind(LayoutInflater.from(parent.context).inflate(
                R.layout.similar_home_item, parent, false
            ))
        )
    }

    override fun onBindViewHolder(holder: SimilarHomeViewHolder, position: Int) {
        val item = homeList?.get(position)
        holder.binding.homeName.text = item
    }

    override fun getItemCount(): Int {
        return homeList?.size?:0
    }

    class SimilarHomeViewHolder(val binding: SimilarHomeItemBinding): RecyclerView.ViewHolder(binding.root)
}