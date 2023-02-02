package com.example.homex.adapter

import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.example.homex.R
import com.example.homex.databinding.MessageItemBinding
import com.example.homex.databinding.MyMessageItemBinding
import com.homex.core.model.Message

private const val MY_MESSAGE = 1
private const val MESSAGE = 2

class MessageAdapter(val messageList: ArrayList<Message>?): RecyclerView.Adapter<RecyclerView.ViewHolder>() {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): RecyclerView.ViewHolder {
        if (viewType == MY_MESSAGE)
            return MyMessageViewHolder(
                MyMessageItemBinding.bind(LayoutInflater.from(parent.context).inflate(
                    R.layout.my_message_item, parent, false
                ))
            )
        return MessageViewHolder(
            MessageItemBinding.bind(LayoutInflater.from(parent.context).inflate(
                R.layout.message_item, parent, false
            ))
        )
    }

    override fun onBindViewHolder(holder: RecyclerView.ViewHolder, position: Int) {
        val item = messageList?.get(position)
        when(holder.itemViewType){
            MESSAGE->{
                val tempHolder = holder as MessageViewHolder
                tempHolder.binding.textMsg.text = item?.message
            }
            MY_MESSAGE->{
                val tempHolder = holder as MyMessageViewHolder
                tempHolder.binding.textMsg.text = item?.message
            }
        }
    }

    override fun getItemCount(): Int {
        return messageList?.size?:0
    }

    override fun getItemViewType(position: Int): Int {
        if(messageList?.get(position)?.isMyMessage == true){
            return MY_MESSAGE
        }
        return MESSAGE
    }

    class MessageViewHolder(val binding: MessageItemBinding): RecyclerView.ViewHolder(binding.root)
    class MyMessageViewHolder(val binding: MyMessageItemBinding): RecyclerView.ViewHolder(binding.root)
}