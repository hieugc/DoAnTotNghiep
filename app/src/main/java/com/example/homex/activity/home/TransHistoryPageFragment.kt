package com.example.homex.activity.home

import android.os.Bundle
import androidx.core.os.bundleOf
import androidx.fragment.app.viewModels
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.R
import com.example.homex.adapter.TransHistoryAdapter
import com.example.homex.app.REQUEST_STATUS
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentTransHistoryPageBinding
import com.example.homex.extension.RequestStatus
import com.example.homex.viewmodel.RequestViewModel
import com.homex.core.model.response.RequestResponse

class TransHistoryPageFragment : BaseFragment<FragmentTransHistoryPageBinding>(),
    TransHistoryAdapter.EventListener {
    override val layoutId: Int = R.layout.fragment_trans_history_page

    private lateinit var adapter: TransHistoryAdapter
    private var requestType: Int = RequestStatus.WAITING.ordinal
    private val viewModel: RequestViewModel by viewModels({ requireParentFragment() })

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
        adapter = TransHistoryAdapter(this, requireContext())
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

        viewModel.requestResponseListLiveDate.observe(viewLifecycleOwner) {
            if (it != null) {
                val listRequest = ArrayList<RequestResponse>()
                for (request in it) {
                    if (request.request?.status == requestType) {
                        listRequest.add(request)
                    }
                }
                adapter.setRequestList(listRequest)
            }
        }
    }

    override fun onBtnRateClick(request: RequestResponse) {
        val bundle = bundleOf("request" to request)
        findNavController().navigate(
            R.id.action_transHistoryFragment_to_rateBottomSheetFragment,
            bundle
        )
    }

    override fun OnItemTransClicked() {
        findNavController().navigate(R.id.action_transHistoryFragment_to_requestDetailFragment)
    }
}