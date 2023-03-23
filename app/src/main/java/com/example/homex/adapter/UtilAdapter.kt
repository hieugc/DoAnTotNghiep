package com.example.homex.adapter

import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.core.content.ContextCompat
import androidx.recyclerview.widget.RecyclerView
import com.example.homex.R
import com.example.homex.databinding.ItemRuleUtilBinding
import com.example.homex.extension.Rules
import com.example.homex.extension.Utilities

class UtilAdapter(var itemList: List<Int>?, val rule: Boolean = false, var showAll: Boolean = false): RecyclerView.Adapter<UtilAdapter.UtilViewHolder>() {
    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): UtilViewHolder {
        return UtilViewHolder(
            ItemRuleUtilBinding.bind(LayoutInflater.from(parent.context).inflate(
                R.layout.item_rule_util, parent, false
            ))
        )
    }

    override fun onBindViewHolder(holder: UtilViewHolder, position: Int) {
        val item = itemList?.get(position)
        if(holder.itemViewType == 1){
            when(item){
                Rules.NO_SMOKING.ordinal + 1 -> {
                    holder.binding.root.text = holder.itemView.resources.getString(R.string.no_smoking)
                    holder.binding.root.setCompoundDrawablesWithIntrinsicBounds(
                        ContextCompat.getDrawable(holder.itemView.context, R.drawable.ic_no_smoking),
                        null,
                        null,
                        null
                    )
                    holder.binding.root.requestLayout()
                }
                Rules.NO_PET.ordinal + 1 -> {
                    holder.binding.root.text = holder.itemView.resources.getString(R.string.no_pet)
                    holder.binding.root.setCompoundDrawablesWithIntrinsicBounds(
                        ContextCompat.getDrawable(holder.itemView.context, R.drawable.ic_no_pet),
                        null,
                        null,
                        null
                    )
                    holder.binding.root.requestLayout()
                }
            }
        }else{
            when(item){
                Utilities.WIFI.ordinal + 1 -> {
                    holder.binding.root.text = holder.itemView.resources.getString(R.string.wifi)
                    holder.binding.root.setCompoundDrawablesWithIntrinsicBounds(
                        ContextCompat.getDrawable(holder.itemView.context, R.drawable.ic_wifi),
                        null,
                        null,
                        null
                    )
                    holder.binding.root.requestLayout()
                }
                Utilities.COMPUTER.ordinal + 1 -> {
                    holder.binding.root.text = holder.itemView.resources.getString(R.string.computer)
                    holder.binding.root.setCompoundDrawablesWithIntrinsicBounds(
                        ContextCompat.getDrawable(holder.itemView.context, R.drawable.ic_computer),
                        null,
                        null,
                        null
                    )
                    holder.binding.root.requestLayout()
                }
                Utilities.TV.ordinal + 1 -> {
                    holder.binding.root.text = holder.itemView.resources.getString(R.string.smart_tv)
                    holder.binding.root.setCompoundDrawablesWithIntrinsicBounds(
                        ContextCompat.getDrawable(holder.itemView.context, R.drawable.ic_tv),
                        null,
                        null,
                        null
                    )
                    holder.binding.root.requestLayout()
                }
                Utilities.BATHTUB.ordinal + 1 -> {
                    holder.binding.root.text = holder.itemView.resources.getString(R.string.bath_tub)
                    holder.binding.root.setCompoundDrawablesWithIntrinsicBounds(
                        ContextCompat.getDrawable(holder.itemView.context, R.drawable.ic_bath),
                        null,
                        null,
                        null
                    )
                    holder.binding.root.requestLayout()
                }
                Utilities.PARKING_LOT.ordinal + 1 -> {
                    holder.binding.root.text = holder.itemView.resources.getString(R.string.parking_lot)
                    holder.binding.root.setCompoundDrawablesWithIntrinsicBounds(
                        ContextCompat.getDrawable(holder.itemView.context, R.drawable.ic_parking_lot),
                        null,
                        null,
                        null
                    )
                    holder.binding.root.requestLayout()
                }
                Utilities.AIR_CONDITIONER.ordinal + 1 -> {
                    holder.binding.root.text = holder.itemView.resources.getString(R.string.air_conditioner)
                    holder.binding.root.setCompoundDrawablesWithIntrinsicBounds(
                        ContextCompat.getDrawable(holder.itemView.context, R.drawable.ic_air_conditioning),
                        null,
                        null,
                        null
                    )
                    holder.binding.root.requestLayout()
                }
                Utilities.WASHING_MACHINE.ordinal + 1 -> {
                    holder.binding.root.text = holder.itemView.resources.getString(R.string.washing_machine)
                    holder.binding.root.setCompoundDrawablesWithIntrinsicBounds(
                        ContextCompat.getDrawable(holder.itemView.context, R.drawable.ic_wash_machine),
                        null,
                        null,
                        null
                    )
                    holder.binding.root.requestLayout()
                }
                Utilities.FRIDGE.ordinal + 1 -> {
                    holder.binding.root.text = holder.itemView.resources.getString(R.string.fridge)
                    holder.binding.root.setCompoundDrawablesWithIntrinsicBounds(
                        ContextCompat.getDrawable(holder.itemView.context, R.drawable.ic_fridge),
                        null,
                        null,
                        null
                    )
                    holder.binding.root.requestLayout()
                }
                Utilities.POOL.ordinal + 1 -> {
                    holder.binding.root.text = holder.itemView.resources.getString(R.string.pool)
                    holder.binding.root.setCompoundDrawablesWithIntrinsicBounds(
                        ContextCompat.getDrawable(holder.itemView.context, R.drawable.ic_pool),
                        null,
                        null,
                        null
                    )
                    holder.binding.root.requestLayout()
                }
            }
        }
    }

    override fun getItemCount(): Int {
        if (itemList == null)
            return 0
        else{
            if (!showAll){
                if (itemList!!.size < 4)
                    return itemList!!.size
                return 4
            }
            return itemList!!.size
        }
    }

    override fun getItemViewType(position: Int): Int {
        if (rule)
            return 1
        return 0
    }

    class UtilViewHolder(val binding: ItemRuleUtilBinding): RecyclerView.ViewHolder(binding.root)
}