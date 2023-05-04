package com.example.homex.adapter

import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.example.homex.R
import com.example.homex.databinding.NotificationItemBinding
import com.example.homex.extension.getFormatDateTime
import com.example.homex.extension.gone
import com.example.homex.extension.loadImage
import com.example.homex.extension.visible
import com.homex.core.model.Notification

class NotificationAdapter(
    private val notificationList: ArrayList<Notification>?, val onClick: (Int) -> Unit
) : RecyclerView.Adapter<NotificationAdapter.NotificationViewHolder>() {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): NotificationViewHolder {
        return NotificationViewHolder(
            NotificationItemBinding.bind(
                LayoutInflater.from(parent.context).inflate(
                    R.layout.notification_item, parent, false
                )
            )
        )
    }

    override fun onBindViewHolder(holder: NotificationViewHolder, position: Int) {
        val item = notificationList?.get(position)
        holder.binding.notificationTitle.text = item?.title
        holder.binding.notificationContent.text = item?.content
        holder.binding.notificationDate.getFormatDateTime(item?.createdDate)
        if (item?.isSeen == true) {
            holder.binding.notificationIndicator.gone()
        } else {
            holder.binding.notificationIndicator.visible()
        }
        holder.binding.root.setOnClickListener {
            item?.idType?.let { onClick.invoke(it) }
        }
        holder.binding.notificationImg.loadImage(item?.imageUrl)
    }

    public fun setList(notificationList: ArrayList<Notification>) {
        this.notificationList?.clear()
        this.notificationList?.addAll(notificationList)
        notifyDataSetChanged()
    }

    public fun add(notification: Notification) {
        this.notificationList?.add(0, notification)
        notifyDataSetChanged()
    }

    override fun getItemCount(): Int {
        return notificationList?.size ?: 0
    }

    class NotificationViewHolder(val binding: NotificationItemBinding) :
        RecyclerView.ViewHolder(binding.root)
}