package com.example.homex.adapter

import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.example.homex.R
import com.example.homex.databinding.MyHomeItemBinding
import com.example.homex.extension.dpToPx

class MyHomeAdapter(val homeList: List<String>?, val onClick: ()->Unit): RecyclerView.Adapter<MyHomeAdapter.MyHomeViewHolder>() {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): MyHomeViewHolder {
        return MyHomeViewHolder(
            MyHomeItemBinding.bind(
                LayoutInflater.from(parent.context).inflate(
                    R.layout.my_home_item, parent, false
                )
            )
        )
    }

    override fun onBindViewHolder(holder: MyHomeViewHolder, position: Int) {
        val item = homeList?.get(position)
        holder.binding.homeName.text = item
        holder.binding.root.setOnClickListener {
            onClick.invoke()
        }
        if(position == homeList?.size!! - 1 )
        {
            val lastParams = holder.itemView.layoutParams as ViewGroup.MarginLayoutParams
            lastParams.bottomMargin = 80f.dpToPx(holder.itemView.context)
            holder.itemView.requestLayout()
        }
    }

    override fun getItemCount(): Int {
        return homeList?.size?:0
    }

    class MyHomeViewHolder(val binding: MyHomeItemBinding): RecyclerView.ViewHolder(binding.root)
}