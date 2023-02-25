package com.example.homex.activity.home

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import com.example.homex.R
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentRequestDetailBinding

class RequestDetailFragment : BaseFragment<FragmentRequestDetailBinding>() {
    override val layoutId: Int = R.layout.fragment_request_detail
    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        // Inflate the layout for this fragment
        return inflater.inflate(R.layout.fragment_request_detail, container, false)
    }

}