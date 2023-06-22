package com.example.homex.activity.home.pending.viewpager

import android.view.View
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.R
import com.example.homex.activity.home.pending.RequestFragmentDirections
import com.example.homex.adapter.RequestItemAdapter
import com.example.homex.base.BaseActivity
import com.example.homex.base.BaseFragmentViewPager
import com.example.homex.databinding.FragmentPendingRequestBinding
import com.example.homex.extension.RequestStatus
import com.example.homex.extension.gone
import com.example.homex.extension.visible
import com.google.android.material.dialog.MaterialAlertDialogBuilder
import com.homex.core.model.response.RequestResponse
import com.homex.core.param.request.UpdateStatusParam
import com.homex.core.util.AppEvent

class PendingRequestAcceptFragment: BaseFragmentViewPager<FragmentPendingRequestBinding>() {
    override val layoutId: Int
        get() = R.layout.fragment_pending_request
    override val requestType: Int
        get() = RequestStatus.ACCEPTED.ordinal

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
                MaterialAlertDialogBuilder(requireContext())
                    .setTitle(getString(R.string.check_in))
                    .setMessage(getString(R.string.check_in_request_message))
                    .setNegativeButton(resources.getString(R.string.cancel)) { _, _ ->
                        // Respond to negative button press
                    }
                    .setPositiveButton(resources.getString(R.string.confirm)) { dialog, _ ->
                        // Respond to positive button press
                        dialog.dismiss()
                        val id = it.request?.id
                        if (id != null){
                            val param = UpdateStatusParam(
                                id = id,
                                status = RequestStatus.CHECK_IN.ordinal
                            )
                            viewModel.updateStatus(param)
                        }
                    }
                    .show()
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

        viewModel.messageLiveData.observe(this){
            AppEvent.showPopUp()
            binding.noRequestLayout.gone()
            requestList.clear()
            binding.requestRecView.visibility = View.INVISIBLE
            viewModel.getPendingRequest()
            (activity as BaseActivity).displayMessage(getString(R.string.checkin_request_success))
        }
    }
}