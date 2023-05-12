package com.example.homex.activity.home.profile

import android.os.Bundle
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.RequestItemAdapter
import com.example.homex.app.REQUEST_STATUS
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentTransHistoryPageBinding
import com.example.homex.extension.RequestStatus
import com.example.homex.viewmodel.RequestViewModel
import com.homex.core.model.response.RequestResponse
import org.koin.androidx.viewmodel.ext.android.sharedViewModel

class TransHistoryPageFragment : BaseFragment<FragmentTransHistoryPageBinding>(){
    override val layoutId: Int = R.layout.fragment_trans_history_page

    private lateinit var adapter: RequestItemAdapter
    private var requestType: Int = RequestStatus.WAITING.ordinal
    private val viewModel: RequestViewModel by sharedViewModel()
    private val requestList = arrayListOf<RequestResponse>()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
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
    }

    override fun setView() {
        adapter = RequestItemAdapter(
            requestList,
            onClick = {
                val action = TransHistoryFragmentDirections.actionTransHistoryFragmentToRequestDetailFragment(it)
                findNavController().navigate(action)
            },
            btnClick = {
                val action = it.request?.id?.let { it1 ->
                    TransHistoryFragmentDirections.actionTransHistoryFragmentToRequestDetailFragment(
                        it1
                    )
                }
                if (action != null) {
                    findNavController().navigate(action)
                }
            }
        )
        binding.rvTransHis.adapter = adapter
        val layoutManager =
            LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.rvTransHis.layoutManager = layoutManager
    }

    override fun setEvent() {

    }

    override fun setViewModel() {
        viewModel.messageLiveData.observe(viewLifecycleOwner) {
            viewModel.getRequestHistory()
        }

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
                adapter.notifyDataSetChanged()
            }
        }
    }
}