package com.example.homex.adapter

import android.content.Context
import android.graphics.Color
import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.core.content.ContextCompat
import androidx.recyclerview.widget.RecyclerView
import com.example.homex.R
import com.example.homex.databinding.ItemTransHistoryBinding
import com.example.homex.extension.RequestStatus
import com.example.homex.extension.RequestType
import com.example.homex.extension.formatIso8601ToFormat
import com.homex.core.model.response.RequestResponse

class TransHistoryAdapter(
    private val listener: EventListener,
    private val mContext: Context
) : RecyclerView.Adapter<TransHistoryAdapter.TransHistoryViewHolder>() {

    private val requestList: ArrayList<RequestResponse> = ArrayList()

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): TransHistoryViewHolder {
        return TransHistoryViewHolder(
            ItemTransHistoryBinding.bind(
                LayoutInflater.from(parent.context).inflate(
                    R.layout.item_trans_history, parent, false
                )
            )
        )
    }

    public fun setRequestList(requestList: ArrayList<RequestResponse>) {
        this.requestList.clear()
        this.requestList.addAll(requestList)
        notifyDataSetChanged()
    }

    override fun onBindViewHolder(holder: TransHistoryViewHolder, position: Int) {
        val item = requestList?.get(position)
        holder.binding.tvUserName.text =
            (item?.house?.user?.lastName + " " + item?.house?.user?.firstName)
        holder.binding.tvTitle.text = item?.house?.name
        when (item?.request?.status) {
            RequestStatus.WAITING.ordinal -> {
                holder.binding.tvStatus.text = mContext.getString(R.string.status_waiting)
                holder.binding.tvStatus.setTextColor(Color.YELLOW)
            }
            RequestStatus.ACCEPTED.ordinal -> {
                holder.binding.tvStatus.text = mContext.getString(R.string.status_accepted)
                holder.binding.tvStatus.setTextColor(Color.BLUE)
            }
            RequestStatus.REJECTED.ordinal -> {
                holder.binding.tvStatus.text = mContext.getString(R.string.status_rejected)
                holder.binding.tvStatus.setTextColor(Color.RED)
            }
            RequestStatus.CHECK_IN.ordinal -> {
                holder.binding.tvStatus.text = mContext.getString(R.string.status_checkin)
                holder.binding.tvStatus.setTextColor(ContextCompat.getColor(holder.itemView.context, R.color.primary))
            }
            RequestStatus.REVIEWING.ordinal -> {
                holder.binding.tvStatus.text = mContext.getString(R.string.status_reviewing)
                holder.binding.tvStatus.setTextColor(Color.MAGENTA)
            }
            RequestStatus.DONE.ordinal -> {
                holder.binding.tvStatus.text = mContext.getString(R.string.status_done)
                holder.binding.tvStatus.setTextColor(Color.GREEN)
            }
        }
        if (item?.request?.type == RequestType.BY_POINT.ordinal) {
            holder.binding.tvType.text = mContext.getString(
                R.string.request_type, mContext.getString(R.string.request_type_point)
            )
        } else {
            holder.binding.tvType.text = mContext.getString(
                R.string.request_type, mContext.getString(R.string.request_type_house)
            )
        }

        holder.binding.tvFromDate.text =
            item?.request?.startDate?.formatIso8601ToFormat("dd/MM/yyyy")
        holder.binding.tvToDate.text = item?.request?.endDate?.formatIso8601ToFormat("dd/MM/yyyy")

        holder.itemView.setOnClickListener {
            listener.OnItemTransClicked()
        }
        holder.binding.btnRate.setOnClickListener {
            listener.onBtnRateClick()
        }
    }

    override fun getItemCount(): Int {
        return requestList.size ?: 0
    }

    class TransHistoryViewHolder(val binding: ItemTransHistoryBinding) :
        RecyclerView.ViewHolder(binding.root)

    interface EventListener {
        fun onBtnRateClick()
        fun OnItemTransClicked()
    }
}