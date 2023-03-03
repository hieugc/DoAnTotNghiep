package com.example.homex

import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.RequestItemAdapter
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentRequestBinding


class RequestFragment : BaseFragment<FragmentRequestBinding>() {
    override val layoutId: Int = R.layout.fragment_request

    private lateinit var adapter: RequestItemAdapter

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showBoxChatLayout = Pair(false, ""),
            showMenu = false,
            showMessage = false,
            showBottomNav = false,
            showSearchLayout = false,
            showTitleApp = Pair(true, "Yêu cầu trao đổi")
        )
    }

    override fun setView() {
        adapter = RequestItemAdapter(
            listOf(
                "Nhà của Hiếu",
                "Nhà của Phạm",
                "Nhà của Nhật"
            )
        ){
            findNavController().navigate(R.id.action_requestFragment_to_pendingRequestDetailFragment)
        }
        binding.requestRecView.adapter = adapter
        val layoutManager = LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.requestRecView.layoutManager = layoutManager
    }
}