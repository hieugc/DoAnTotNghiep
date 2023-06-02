package com.example.homex.adapter

import android.view.LayoutInflater
import android.view.ViewGroup
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
            if (item != null){
                val rule = Rules.values()[item - 1]
                holder.binding.root.text = rule.getString(holder.itemView.context)
                holder.binding.root.setCompoundDrawablesWithIntrinsicBounds(
                    rule.getDrawable(holder.itemView.context),
                    null,
                    null,
                    null
                )
                holder.binding.root.requestLayout()
            }
        }else{
            if (item != null){
                val util = Utilities.values()[item - 1]
                holder.binding.root.text = util.getString(holder.itemView.context)
                holder.binding.root.setCompoundDrawablesWithIntrinsicBounds(
                    util.getDrawable(holder.itemView.context),
                    null,
                    null,
                    null
                )
                holder.binding.root.requestLayout()
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