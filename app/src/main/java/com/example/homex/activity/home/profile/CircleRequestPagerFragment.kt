package com.example.homex.activity.home.profile

import android.os.Bundle
import android.view.View
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.R
import com.example.homex.adapter.CircleRequestAdapter
import com.example.homex.app.REQUEST_STATUS
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentCircleRequestPagerBinding
import com.example.homex.extension.StatusWaitingRequest
import com.example.homex.extension.gone
import com.example.homex.extension.visible
import com.example.homex.viewmodel.RequestViewModel
import com.homex.core.model.response.CircleRequest
import com.homex.core.util.AppEvent
import org.koin.androidx.viewmodel.ext.android.viewModel

class CircleRequestPagerFragment : BaseFragment<FragmentCircleRequestPagerBinding>() {
    override val layoutId: Int = R.layout.fragment_circle_request_pager
    private lateinit var adapter: CircleRequestAdapter
    private var requestType: Int = StatusWaitingRequest.INIT.ordinal
    private val viewModel: RequestViewModel by viewModel()
    private val requestList = arrayListOf<CircleRequest>()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        arguments?.takeIf { it.containsKey(REQUEST_STATUS) }?.apply {
            requestType = getInt(REQUEST_STATUS)
        }
        viewModel.getCircleRequest()
    }

    override fun setEvent() {
        initSwipeLayout()
    }

    private fun initSwipeLayout(){
        binding.swipeRefreshLayout.setOnRefreshListener {
            AppEvent.showPopUp()
            binding.noRequestLayout.gone()
            requestList.clear()
            binding.rvCircleRequest.visibility = View.INVISIBLE
            viewModel.getCircleRequest()
            binding.swipeRefreshLayout.isRefreshing = false
        }
    }

    override fun setView() {
        adapter = CircleRequestAdapter(
            requestList,
            onClick = {
                val action =
                    CircleRequestFragmentDirections.actionCircleRequestFragmentToCircleRequestDetailFragment(
                        it
                    )
                findNavController().navigate(action)
            },
            btnClick = {
                val action =
                    CircleRequestFragmentDirections.actionCircleRequestFragmentToCircleRequestDetailFragment(
                        it
                    )
                findNavController().navigate(action)
            }
        )
        binding.rvCircleRequest.adapter = adapter
        val layoutManager =
            LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.rvCircleRequest.layoutManager = layoutManager
    }

    override fun setViewModel() {
        viewModel.circleRequestResponseListLiveDate.observe(viewLifecycleOwner) {
            if (it != null) {
                requestList.clear()
                val listRequest = ArrayList<CircleRequest>()
                for (request in it) {
                    if (request.status == requestType) {
                        listRequest.add(request)
                    }
                }
                if (listRequest.isNotEmpty()) {
                    binding.rvCircleRequest.visible()
                    requestList.addAll(listRequest)
                    adapter.notifyDataSetChanged()
                } else {
                    binding.rvCircleRequest.gone()
                    binding.noRequestLayout.visible()
                }
            }
            AppEvent.closePopup()
        }
    }
}