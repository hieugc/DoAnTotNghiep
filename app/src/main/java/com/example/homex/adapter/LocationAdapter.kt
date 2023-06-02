package com.example.homex.adapter

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ArrayAdapter
import android.widget.TextView
import com.example.homex.R
import com.homex.core.model.BingLocation

class LocationAdapter(context: Context, private val resource: Int, private val objects: List<BingLocation>): ArrayAdapter<BingLocation>(context, resource, objects) {
    override fun getView(position: Int, convertView: View?, parent: ViewGroup): View {
        var v = convertView
        if (v == null){
            v = LayoutInflater.from(parent.context).inflate(resource,  parent, false)
        }
        val item = objects[position]
        val tvLocationName = v?.findViewById<TextView>(R.id.textView2)
        tvLocationName?.text = item.name

        return v?:super.getDropDownView(position, null, parent)
    }


    override fun getDropDownView(position: Int, convertView: View?, parent: ViewGroup): View {
        var v = convertView
        if (v == null){
            v = LayoutInflater.from(parent.context).inflate(resource,  parent, false)
        }
        val item = objects[position]
        val tvLocationName = v?.findViewById<TextView>(R.id.textView2)
        tvLocationName?.text = item.name

        return v?:super.getDropDownView(position, null, parent)
    }
}