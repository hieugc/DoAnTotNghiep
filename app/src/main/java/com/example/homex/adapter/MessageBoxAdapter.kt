package com.example.homex.adapter

import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.example.homex.R
import com.example.homex.databinding.MessageBoxItemBinding
import com.homex.core.model.MessageBox

class MessageBoxAdapter(private val messageBoxList: ArrayList<MessageBox>?, private val onClick: ()->Unit): RecyclerView.Adapter<MessageBoxAdapter.MessageBoxViewHolder>() {
    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): MessageBoxViewHolder {
        return MessageBoxViewHolder(
            MessageBoxItemBinding.bind(
                LayoutInflater.from(parent.context).inflate(
                    R.layout.message_box_item, parent, false
                )
            )
        )
    }

    override fun onBindViewHolder(holder: MessageBoxViewHolder, position: Int) {
        val item = messageBoxList?.get(position)
        holder.binding.boxName.text = item?.user
        holder.binding.lastMsg.text = item?.preview
        holder.binding.root.setOnClickListener {
            onClick.invoke()
        }
    }

    override fun getItemCount(): Int {
        return messageBoxList?.size?:0
    }

    class MessageBoxViewHolder(val binding: MessageBoxItemBinding): RecyclerView.ViewHolder(binding.root)
}