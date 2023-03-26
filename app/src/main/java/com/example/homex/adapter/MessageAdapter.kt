package com.example.homex.adapter

import android.util.Log
import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.bumptech.glide.Glide
import com.example.homex.R
import com.example.homex.databinding.MessageItemBinding
import com.example.homex.databinding.MessageItemNoAvatarBinding
import com.example.homex.databinding.MessageTimeItemBinding
import com.example.homex.databinding.MyMessageItemBinding
import com.example.homex.extension.convertToRelativeDate
import com.example.homex.extension.dpToPx
import com.example.homex.extension.formatIso8601ToFormat
import com.homex.core.model.Message
import com.homex.core.model.UserMessage

private const val MY_MESSAGE = 1
private const val MESSAGE = 2
private const val TIME = 3
private const val MESSAGE_NO_AVATAR = 4

class MessageAdapter(var messageList: ArrayList<Message>?, var userMessages: ArrayList<UserMessage>?, var userAccess: String? = null): RecyclerView.Adapter<RecyclerView.ViewHolder>() {
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
        var msgUser : UserMessage? = null
        if (holder.itemViewType == MESSAGE){
            if (userMessages != null) {
                for(user in userMessages!!){
                    if (user.userAccess == item?.idSend){
                        msgUser = user
                        break
                    }
                }
            }
        }
        when(holder.itemViewType){
            MESSAGE->{
                val tempHolder = holder as MessageViewHolder
                tempHolder.binding.textMsg.text = item?.message
                Glide.with(holder.itemView.context)
                    .load(msgUser?.imageUrl)
                    .placeholder(R.drawable.ic_baseline_image_24)
                    .error(R.mipmap.avatar)
                    .into(tempHolder.binding.userAvatar)
            }
            MY_MESSAGE->{
                val tempHolder = holder as MyMessageViewHolder
                tempHolder.binding.textMsg.text = item?.message
                if(position > 0)
                {
                    val nextItem = messageList?.get(position - 1)
                    if (nextItem?.idSend == userAccess){
                        val lastParams = holder.itemView.layoutParams as ViewGroup.MarginLayoutParams
                        lastParams.bottomMargin = 4f.dpToPx(holder.itemView.context)
                        holder.itemView.requestLayout()
                    }else{
                        val lastParams = holder.itemView.layoutParams as ViewGroup.MarginLayoutParams
                        lastParams.bottomMargin = 8f.dpToPx(holder.itemView.context)
                        holder.itemView.requestLayout()
                    }
                }else{
                    val lastParams = holder.itemView.layoutParams as ViewGroup.MarginLayoutParams
                    lastParams.bottomMargin = 8f.dpToPx(holder.itemView.context)
                    holder.itemView.requestLayout()
                }
            }
            TIME->{
                Log.e("time", "${item?.createdDate}")
                val tempHolder = holder as TimeViewHolder
                tempHolder.binding.msgTimeTV.text = item?.createdDate.convertToRelativeDate()
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
        if(messageList?.get(position)?.idSend == userAccess){
            return MY_MESSAGE
        }
        if (position > 0){
            if(messageList?.get(position - 1)?.idSend == messageList?.get(position)?.idSend){
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