package com.example.homex.adapter

import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.example.homex.R
import com.example.homex.databinding.MessageItemBinding
import com.example.homex.databinding.MessageItemNoAvatarBinding
import com.example.homex.databinding.MessageTimeItemBinding
import com.example.homex.databinding.MyMessageItemBinding
import com.homex.core.model.Message

private const val MY_MESSAGE = 1
private const val MESSAGE = 2
private const val TIME = 3
private const val MESSAGE_NO_AVATAR = 4

class MessageAdapter(val messageList: ArrayList<Message>?): RecyclerView.Adapter<RecyclerView.ViewHolder>() {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): RecyclerView.ViewHolder {
        if (viewType == TIME){
            return TimeViewHolder(
                MessageTimeItemBinding.bind(LayoutInflater.from(parent.context).inflate(
                    R.layout.message_time_item, parent, false
                ))
            )
        }
        if (viewType == MESSAGE_NO_AVATAR){
            return MessageNoAvatarViewHolder(
                MessageItemNoAvatarBinding.bind(LayoutInflater.from(parent.context).inflate(
                    R.layout.message_item_no_avatar, parent, false
                ))
            )
        }
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
            TIME->{
                val tempHolder = holder as TimeViewHolder
                tempHolder.binding.msgTimeTV.text = item?.date
            }
            MESSAGE_NO_AVATAR->{
                val tempHolder = holder as MessageNoAvatarViewHolder
                tempHolder.binding.textMsg.text = item?.message
            }
        }
    }

    override fun getItemCount(): Int {
        return messageList?.size?:0
    }

    override fun getItemViewType(position: Int): Int {
        if(messageList?.get(position)?.isDateItem == true){
            return TIME
        }
        if(messageList?.get(position)?.isMyMessage == true){
            return MY_MESSAGE
        }
        if (position > 0){
            if(messageList?.get(position - 1)?.userID == messageList?.get(position)?.userID){
                return MESSAGE_NO_AVATAR
            }
        }
        return MESSAGE
    }

    class MessageViewHolder(val binding: MessageItemBinding): RecyclerView.ViewHolder(binding.root)
    class MyMessageViewHolder(val binding: MyMessageItemBinding): RecyclerView.ViewHolder(binding.root)
    class TimeViewHolder(val binding: MessageTimeItemBinding): RecyclerView.ViewHolder(binding.root)
    class MessageNoAvatarViewHolder(val binding: MessageItemNoAvatarBinding): RecyclerView.ViewHolder(binding.root)
}