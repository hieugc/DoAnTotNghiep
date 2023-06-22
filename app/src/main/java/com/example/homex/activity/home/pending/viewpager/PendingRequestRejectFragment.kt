package com.example.homex.activity.home.pending.viewpager

import android.os.Bundle
import android.view.View
import androidx.core.os.bundleOf
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.R
import com.example.homex.activity.home.pending.RequestFragmentDirections
import com.example.homex.adapter.RequestItemAdapter
import com.example.homex.app.CONTACT_USER
import com.example.homex.app.ID
import com.example.homex.base.BaseFragmentViewPager
import com.example.homex.databinding.FragmentPendingRequestBinding
import com.example.homex.extension.RequestStatus
import com.example.homex.extension.gone
import com.example.homex.extension.visible
import com.example.homex.viewmodel.ChatViewModel
import com.homex.core.model.response.RequestResponse
import com.homex.core.param.chat.ContactUserParam
import com.homex.core.util.AppEvent
import com.homex.core.util.PrefUtil
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class PendingRequestRejectFragment: BaseFragmentViewPager<FragmentPendingRequestBinding>() {
    override val layoutId: Int
        get() = R.layout.fragment_pending_request
    override val requestType: Int
        get() = RequestStatus.REJECTED.ordinal
    private val chatViewModel: ChatViewModel by viewModel()
    private val prefUtil : PrefUtil by inject()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        chatViewModel.connectToUser.observe(this){ messageRoom ->
            if (messageRoom != null){
                messageRoom.idRoom?.let {
                    findNavController().navigate(
                        R.id.action_global_messageFragment, bundleOf(
                            ID to it,
                            CONTACT_USER to true
                        )
                    )
                }
            }
        }
    }


    override fun setEvent() {
        initSwipeLayout()
    }

    private fun initSwipeLayout(){
        binding.swipeRefreshLayout.setOnRefreshListener {
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
            btnClick = { request ->
                val userAccess = request.request?.user?.userAccess
                prefUtil.connectionId?.let { connectionId->
                    userAccess?.let {
                        chatViewModel.contactToUser(param = ContactUserParam(
                            connectionId = connectionId,
                            userAccess = userAccess
                        ))
                    }
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