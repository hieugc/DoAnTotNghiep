package com.example.homex.activity.home

import android.os.Bundle
import android.view.View
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.R
import com.example.homex.adapter.RequestItemAdapter
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentRequestBinding
import com.example.homex.extension.gone
import com.example.homex.extension.visible
import com.example.homex.viewmodel.RequestViewModel
import com.homex.core.model.response.RequestResponse
import com.homex.core.util.AppEvent
import org.koin.androidx.viewmodel.ext.android.viewModel


class RequestFragment : BaseFragment<FragmentRequestBinding>() {
    override val layoutId: Int = R.layout.fragment_request
    private val viewModel: RequestViewModel by viewModel()
    private val requestList = arrayListOf<RequestResponse>()
    private lateinit var adapter: RequestItemAdapter

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showBoxChatLayout = Pair(false, null),
            showMenu = false,
            showMessage = false,
            showBottomNav = false,
            showSearchLayout = false,
            showTitleApp = Pair(true, "Yêu cầu trao đổi")
        )
        viewModel.getPendingRequest()
        AppEvent.showLoading()
    }

    override fun setView() {
        adapter = RequestItemAdapter(
            requestList
        ){
            val action = RequestFragmentDirections.actionRequestFragmentToPendingRequestDetailFragment(it)
            findNavController().navigate(action)
        }
        binding.requestRecView.adapter = adapter
        val layoutManager = LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.requestRecView.layoutManager = layoutManager
    }

    override fun setViewModel() {
        viewModel.requestResponseListLiveDate.observe(this){
            if (it != null){
                requestList.clear()
                requestList.addAll(it)
                adapter.notifyDataSetChanged()
                if (requestList.isEmpty()){
                    binding.noHomeTxt.visible()
                }else{
                    binding.noHomeTxt.gone()
                }
            }
            AppEvent.hideLoading()
        }
    }
}