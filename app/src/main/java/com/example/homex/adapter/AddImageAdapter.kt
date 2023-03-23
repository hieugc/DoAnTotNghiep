package com.example.homex.adapter

import android.net.Uri
import android.util.Log
import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.bumptech.glide.Glide
import com.bumptech.glide.request.RequestOptions
import com.example.homex.R
import com.example.homex.databinding.AddHomeImageItemBinding
import com.homex.core.model.ImageBase

class AddImageAdapter(var imgList: MutableList<Pair<Uri, Boolean>>? = arrayListOf(), val onClick: (Int)->Unit, var images: MutableList<ImageBase>? = mutableListOf(), val removeItem: (ImageBase)->Unit): RecyclerView.Adapter<AddImageAdapter.ImageViewHolder>() {
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
                onClick.invoke(realPosition)
            }
            Log.e("uri", "${item?.first}")
            Glide.with(holder.itemView.context)
                .load(item?.first)
                .error(R.drawable.ic_baseline_image_24)
                .into(holder.binding.imgView)

        }else{
            val item = images?.get(position)
            holder.binding.deleteImgBtn.setOnClickListener {
                if (item != null) {
                    removeItem.invoke(item)
                }
            }
            Log.e("url", "${item?.data}")
            Glide.with(holder.itemView.context)
                .load(item?.data)
                .error(R.drawable.ic_baseline_image_24)
                .into(holder.binding.imgView)
        }

    }

    override fun getItemCount(): Int {
        if (imgList != null && images != null){
            if(images!!.size + imgList!!.size >= 5)
                return 5
            else
                return images!!.size + imgList!!.size
        }else if(imgList == null){
            return images!!.size
        }else if(images == null){
            return imgList!!.size
        }
        return 0
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