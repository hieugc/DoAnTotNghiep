package com.example.homex.adapter

import android.content.Context
import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.core.content.ContextCompat
import androidx.recyclerview.widget.RecyclerView
import com.bumptech.glide.Glide
import com.example.homex.R
import com.example.homex.databinding.MessageBoxItemBinding
import com.example.homex.extension.convertToRelativeDateTime
import com.example.homex.extension.gone
import com.example.homex.extension.visible
import com.homex.core.model.MessageRoom
import kotlin.collections.ArrayList

class MessageBoxAdapter(private var messageBoxList: ArrayList<MessageRoom>?, private val onClick: (MessageRoom, Int)->Unit, var userAccess : String? = null): RecyclerView.Adapter<MessageBoxAdapter.MessageBoxViewHolder>() {
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
        val context = holder.itemView.context
        if (item?.userMessages?.isNotEmpty() == true){
            holder.binding.boxName.text = item.userMessages?.get(0)?.userName
            Glide.with(context)
                .load(item.userMessages?.get(0)?.imageUrl)
                .placeholder(R.drawable.ic_baseline_image_24)
                .error(R.mipmap.avatar)
                .into(holder.binding.appCompatImageView2)
        }
        if (item?.messages?.isNotEmpty() == true){
            if (item.messages?.get(0)?.idSend == userAccess){
                holder.binding.lastMsg.text = context.getString(R.string.your_message, item.messages?.get(0)?.message)
            }else{
                holder.binding.lastMsg.text = item.messages?.get(0)?.message
            }
            holder.binding.msgTime.text = item.messages?.get(0)?.createdDate.convertToRelativeDateTime()
            if(item.messages?.get(0)?.isSeen == true){
                notificationWasRead(holder.binding, context)
            }else{
                notificationWasNotRead(holder.binding, context)
            }
        }else{
            holder.binding.lastMsg.text = context.getString(R.string.you_are_connected)
            notificationWasRead(holder.binding, context)
        }
        holder.binding.root.setOnClickListener {
            if (item != null) {
                onClick.invoke(item, position)
            }
        }
    }

    override fun getItemCount(): Int {
        return messageBoxList?.size?:0
    }

    private fun notificationWasRead(binding: MessageBoxItemBinding, context: Context){
        binding.boxName.setTextColor(ContextCompat.getColor(context, R.color.gray))
        binding.lastMsg.setTextColor(ContextCompat.getColor(context, R.color.gray))
        binding.msgTime.setTextColor(ContextCompat.getColor(context, R.color.gray))
        binding.isReadIndicator.gone()
    }

    private fun notificationWasNotRead(binding: MessageBoxItemBinding, context: Context){
        binding.boxName.setTextColor(ContextCompat.getColor(context, R.color.black))
        binding.lastMsg.setTextColor(ContextCompat.getColor(context, R.color.black))
        binding.msgTime.setTextColor(ContextCompat.getColor(context, R.color.gray6))
        binding.isReadIndicator.visible()
    }

    class MessageBoxViewHolder(val binding: MessageBoxItemBinding): RecyclerView.ViewHolder(binding.root)
}