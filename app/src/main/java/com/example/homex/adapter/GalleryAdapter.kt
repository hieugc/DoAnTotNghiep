package com.example.homex.adapter

import android.net.Uri
import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.bumptech.glide.Glide
import com.bumptech.glide.request.RequestOptions
import com.example.homex.R
import com.example.homex.databinding.GalleryItemBinding

class GalleryAdapter(private val imgList: List<Uri>? = arrayListOf(), val onClick: (Uri?) -> Unit): RecyclerView.Adapter<GalleryAdapter.GalleryItemViewHolder>() {
    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): GalleryItemViewHolder {
        return GalleryItemViewHolder(
            GalleryItemBinding.bind(
                LayoutInflater.from(parent.context).inflate(
                    R.layout.gallery_item, parent,false
                )
            )
        )
    }

    override fun onBindViewHolder(holder: GalleryItemViewHolder, position: Int) {
        val item = imgList?.get(position)
        holder.binding.root.setOnClickListener {
            onClick.invoke(item)
        }
        Glide.with(holder.itemView.context)
            .load(item)
            .apply(RequestOptions().centerCrop())
            .into(holder.binding.ivPhoto)
    }

    override fun getItemCount(): Int {
        return imgList?.size?:0
    }

    class GalleryItemViewHolder(val binding: GalleryItemBinding): RecyclerView.ViewHolder(binding.root)
}