package com.example.homex.activity.home.history

import android.os.Bundle
import android.view.View
import androidx.core.os.bundleOf
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.RequestItemAdapter
import com.example.homex.app.CONTACT_USER
import com.example.homex.app.ID
import com.example.homex.app.REQUEST_STATUS
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentTransHistoryPageBinding
import com.example.homex.extension.RequestStatus
import com.example.homex.extension.gone
import com.example.homex.extension.visible
import com.example.homex.viewmodel.ChatViewModel
import com.example.homex.viewmodel.RequestViewModel
import com.google.android.material.dialog.MaterialAlertDialogBuilder
import com.homex.core.model.response.RequestResponse
import com.homex.core.param.chat.ContactUserParam
import com.homex.core.param.request.UpdateStatusParam
import com.homex.core.util.AppEvent
import com.homex.core.util.PrefUtil
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class TransHistoryPageFragment : BaseFragment<FragmentTransHistoryPageBinding>(){
    override val layoutId: Int = R.layout.fragment_trans_history_page

    private lateinit var adapter: RequestItemAdapter
    private var requestType: Int = RequestStatus.WAITING.ordinal
    private val viewModel: RequestViewModel by viewModel()
    private val requestList = arrayListOf<RequestResponse>()
    private val prefUtil: PrefUtil by inject()
    private val chatViewModel: ChatViewModel by viewModel()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        viewModel.getRequestHistory()
        chatViewModel.connectToUser.observe(this){ messageRoom->
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

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showMenu = false,
            showMessage = false,
            showTitleApp = Pair(true, getString(R.string.swap_history)),
            showBottomNav = false,
            showBoxChatLayout = Pair(false, null),
        )
        arguments?.takeIf { it.containsKey(REQUEST_STATUS) }?.apply {
            requestType = getInt(REQUEST_STATUS)
        }
        initSwipeLayout()
    }

    private fun initSwipeLayout(){
        binding.swipeRefreshLayout.setOnRefreshListener {
            AppEvent.showPopUp()
            binding.noRequestLayout.gone()
            requestList.clear()
            binding.rvTransHis.visibility = View.INVISIBLE
            viewModel.getRequestHistory()
            binding.swipeRefreshLayout.isRefreshing = false
        }
    }
    override fun setView() {
        adapter = RequestItemAdapter(
            requestList,
            onClick = {
                val action =
                    TransHistoryFragmentDirections.actionTransHistoryFragmentToRequestDetailFragment(
                        it
                    )
                findNavController().navigate(action)
            },
            btnClick = {
                when (requestType) {
                    RequestStatus.ACCEPTED.ordinal -> {
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
                    RequestStatus.WAITING.ordinal -> {
                        val action = it.request?.id?.let { it1 ->
                            TransHistoryFragmentDirections.actionTransHistoryFragmentToRequestDetailFragment(
                                it1
                            )
                        }
                        if (action != null) {
                            findNavController().navigate(action)
                        }
                    }
                    RequestStatus.CHECK_IN.ordinal -> {
                        MaterialAlertDialogBuilder(requireContext())
                            .setTitle(getString(R.string.check_out))
                            .setMessage(getString(R.string.check_out_request_message))
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
                                        status = RequestStatus.REVIEWING.ordinal
                                    )
                                    viewModel.updateStatus(param)
                                }
                            }
                            .show()
                    }
                    RequestStatus.REJECTED.ordinal -> {
                        val userAccess = it.house?.user?.userAccess
                        prefUtil.connectionId?.let { connectionId->
                            userAccess?.let {
                                chatViewModel.contactToUser(param = ContactUserParam(
                                    connectionId = connectionId,
                                    userAccess = userAccess
                                )
                                )
                            }
                        }
                    }

                    RequestStatus.REVIEWING.ordinal -> {
                        val action =
                            TransHistoryFragmentDirections.actionTransHistoryFragmentToRateBottomSheetFragment(
                                it
                            )
                        findNavController().navigate(action)
                    }

                    RequestStatus.DONE.ordinal -> {
                        val id = it.request?.id ?: return@RequestItemAdapter
                        val action =
                            TransHistoryFragmentDirections.actionTransHistoryFragmentToRequestDetailFragment(
                                id
                            )
                        findNavController().navigate(action)
                    }
                }
            }
        )
        binding.rvTransHis.adapter = adapter
        val layoutManager =
            LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.rvTransHis.layoutManager = layoutManager
    }


    override fun setViewModel() {
        viewModel.requestSentResponseListLiveDate.observe(viewLifecycleOwner) {
            if (it != null) {
                requestList.clear()
                val listRequest = ArrayList<RequestResponse>()
                for (request in it) {
                    if (request.request?.status == requestType) {
                        listRequest.add(request)
                    }
                }
                requestList.addAll(listRequest)
                if (requestList.isEmpty()){
                    binding.noRequestLayout.visible()
                }else{
                    binding.rvTransHis.visible()
                    binding.noRequestLayout.gone()
                }
                adapter.notifyDataSetChanged()
            }else{
                binding.noRequestLayout.visible()
                binding.rvTransHis.gone()
            }
            AppEvent.closePopup()
        }
    }
}