package com.example.homex.adapter

import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.example.homex.R
import com.example.homex.databinding.NotificationItemBinding
import com.homex.core.model.Notification

class NotificationAdapter(private val notificationList: ArrayList<Notification>?): RecyclerView.Adapter<NotificationAdapter.NotificationViewHolder>() {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): NotificationViewHolder {
        return NotificationViewHolder(NotificationItemBinding.bind(
            LayoutInflater.from(parent.context).inflate(
                R.layout.notification_item, parent, false
            )
        ))
    }

    override fun onBindViewHolder(holder: NotificationViewHolder, position: Int) {
        val item = notificationList?.get(position)
        holder.binding.notificationTitle.text = item?.title
        holder.binding.notificationContent.text = item?.content
    }

    override fun getItemCount(): Int {
        return notificationList?.size?:0
    }

    class NotificationViewHolder(val binding: NotificationItemBinding):RecyclerView.ViewHolder(binding.root)
}