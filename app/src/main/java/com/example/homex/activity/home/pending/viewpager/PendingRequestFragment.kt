package com.example.homex.activity.home.pending.viewpager

import android.view.View
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.R
import com.example.homex.activity.home.pending.RequestFragmentDirections
import com.example.homex.adapter.RequestItemAdapter
import com.example.homex.base.BaseFragmentViewPager
import com.example.homex.databinding.FragmentPendingRequestBinding
import com.example.homex.extension.RequestStatus
import com.example.homex.extension.gone
import com.example.homex.extension.visible
import com.homex.core.model.response.RequestResponse
import com.homex.core.util.AppEvent


class PendingRequestFragment : BaseFragmentViewPager<FragmentPendingRequestBinding>() {
    override val layoutId: Int
        get() = R.layout.fragment_pending_request
    override val requestType: Int
        get() = RequestStatus.WAITING.ordinal

    override fun setEvent() {
        initSwipeLayout()
    }

    private fun initSwipeLayout(){
        binding.swipeRefreshLayout.setOnRefreshListener {
            AppEvent.showPopUp()
            binding.noRequestLayout.gone()
            requestList.clear()
            adapter.notifyDataSetChanged()
            binding.requestRecView.visibility = View.INVISIBLE
            viewModel.getPendingRequest()
            binding.swipeRefreshLayout.isRefreshing = false
        }
    }

    override fun setView() {
        adapter = RequestItemAdapter(
            requestList,
            onClick = {
                val action = RequestFragmentDirections.actionRequestFragmentToPendingRequestDetailFragment(it)
                findNavController().navigate(action)
            },
            btnClick = {
                val action = it.request?.id?.let { it1 ->
                    RequestFragmentDirections.actionRequestFragmentToPendingRequestDetailFragment(
                        it1
                    )
                }
                if (action != null) {
                    findNavController().navigate(action)
                }
            }
        )
        binding.requestRecView.adapter = adapter
        val layoutManager = LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.requestRecView.layoutManager = layoutManager
    }

    override fun setViewModel() {
        viewModel.requestResponseListLiveDate.observe(this){
            if (it != null){
                requestList.clear()
                val tmpList = arrayListOf<RequestResponse>()
                for (tmp in it){
                    if (tmp.request?.status == requestType)
                        tmpList.add(tmp)
                }
                requestList.addAll(tmpList)
                adapter.notifyDataSetChanged()
                if (requestList.isEmpty()){
                    binding.noRequestLayout.visible()
                }else{
                    binding.requestRecView.visible()
                    binding.noRequestLayout.gone()
                }
            }else{
                binding.noRequestLayout.visible()
                binding.requestRecView.gone()
            }
            AppEvent.closePopup()
        }
    }
}