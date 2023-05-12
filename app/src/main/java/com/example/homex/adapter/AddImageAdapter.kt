package com.example.homex.adapter

import android.net.Uri
import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.bumptech.glide.Glide
import com.example.homex.R
import com.example.homex.databinding.AddHomeImageItemBinding
import com.homex.core.model.ImageBase

class AddImageAdapter(var imgList: MutableList<Pair<Uri, Boolean>>? = arrayListOf(), val onClick: (Int, Int)->Unit, var images: MutableList<ImageBase>? = mutableListOf(), private val removeItem: (ImageBase, Int)->Unit): RecyclerView.Adapter<AddImageAdapter.ImageViewHolder>() {
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
        if (holder.itemViewType == 1){
            val realPosition = position - images?.size!!
            val item = imgList?.get(realPosition)
            holder.binding.deleteImgBtn.setOnClickListener {
                onClick.invoke(realPosition, position)
            }
            Glide.with(holder.itemView.context)
                .load(item?.first)
                .error(R.drawable.ic_baseline_image_24)
                .into(holder.binding.imgView)

        }else{
            val item = images?.get(position)
            holder.binding.deleteImgBtn.setOnClickListener {
                if (item != null) {
                    removeItem.invoke(item, position)
                }
            }
            Glide.with(holder.itemView.context)
                .load(item?.data)
                .error(R.drawable.ic_baseline_image_24)
                .into(holder.binding.imgView)
        }

    }

    override fun getItemCount(): Int {
        return if (imgList != null && images != null){
            if(images!!.size + imgList!!.size >= 5)
                5
            else
                images!!.size + imgList!!.size
        }else if(imgList == null){
            images!!.size
        }else{
            imgList!!.size
        }
    }

    override fun getItemViewType(position: Int): Int {
        if (images != null){
            if (position >= images!!.size)
                return 1
            return 0
        }
        return 1
    }

    class ImageViewHolder(val binding: AddHomeImageItemBinding): RecyclerView.ViewHolder(binding.root)
}