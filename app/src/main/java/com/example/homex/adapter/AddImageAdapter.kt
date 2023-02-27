package com.example.homex.adapter

import android.net.Uri
import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.bumptech.glide.Glide
import com.bumptech.glide.request.RequestOptions
import com.example.homex.R
import com.example.homex.databinding.AddHomeImageItemBinding

class AddImageAdapter(val imgList: MutableList<Uri>? = arrayListOf(), val onClick: (Int)->Unit): RecyclerView.Adapter<AddImageAdapter.ImageViewHolder>() {
    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ImageViewHolder {
        return ImageViewHolder(
            AddHomeImageItemBinding.bind(
                LayoutInflater.from(parent.context).inflate(
                    R.layout.add_home_image_item, parent,false
                )
            )
        )
    }

    override fun onBindViewHolder(holder: ImageViewHolder, position: Int) {
        val item = imgList?.get(position)
        holder.binding.deleteImgBtn.setOnClickListener {
            onClick.invoke(position)
        }
        Glide.with(holder.itemView.context)
            .load(item)
            .into(holder.binding.imgView)

    }

    override fun getItemCount(): Int {
        return imgList?.size?:0
    }

    class ImageViewHolder(val binding: AddHomeImageItemBinding): RecyclerView.ViewHolder(binding.root)
}