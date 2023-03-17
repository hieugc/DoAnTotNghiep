package com.example.homex.adapter

import android.util.Log
import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.bumptech.glide.Glide
import com.example.homex.R
import com.example.homex.databinding.ImgSlideShowItemBinding
import com.homex.core.model.ImageBase

class ImageSlideAdapter(var imgList: List<ImageBase>? = listOf()): RecyclerView.Adapter<ImageSlideAdapter.ImageViewHolder>() {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ImageViewHolder {
        return ImageViewHolder(
            ImgSlideShowItemBinding.bind(
                LayoutInflater.from(parent.context).inflate(
                    R.layout.img_slide_show_item, parent, false
                )
            )
        )
    }

    override fun onBindViewHolder(holder: ImageViewHolder, position: Int) {
        val url = imgList?.get(position)
        Log.e("url", url?.data.toString())
        Glide.with(holder.itemView.context)
            .asBitmap()
            .placeholder(R.drawable.ic_baseline_image_24)
            .error(R.mipmap.location)
            .load(url?.data)
            .into(holder.binding.img)
    }

    override fun getItemCount(): Int {
        return imgList?.size?:0
    }

    class ImageViewHolder(val binding: ImgSlideShowItemBinding): RecyclerView.ViewHolder(binding.root)
}