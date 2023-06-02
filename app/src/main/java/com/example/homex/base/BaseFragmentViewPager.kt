package com.example.homex.base

import android.os.Bundle
import android.view.View
import androidx.databinding.ViewDataBinding
import com.example.homex.adapter.RequestItemAdapter
import com.example.homex.viewmodel.RequestViewModel
import com.homex.core.model.response.RequestResponse
import org.koin.androidx.viewmodel.ext.android.viewModel

abstract class BaseFragmentViewPager<ViewBinding: ViewDataBinding>: BaseFragment<ViewBinding>() {
    abstract val requestType: Int
    val viewModel: RequestViewModel by viewModel()
    val requestList = arrayListOf<RequestResponse>()
    lateinit var adapter: RequestItemAdapter

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        viewModel.getPendingRequest()
    }
}