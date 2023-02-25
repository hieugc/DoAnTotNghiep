package com.example.homex.activity.home

import android.os.Bundle
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.R
import com.example.homex.adapter.TransHistoryAdapter
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentTransHistoryBinding
import com.homex.core.model.TransHistory

class TransHistoryFragment : BaseFragment<FragmentTransHistoryBinding>(),
    TransHistoryAdapter.EventListener {
    override val layoutId: Int = R.layout.fragment_trans_history

    private lateinit var adapter: TransHistoryAdapter

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showMenu = false,
            showMessage = false,
            showTitleApp = Pair(true, getString(R.string.swap_history)),
            showBottomNav = false,
            showBoxChatLayout = Pair(false, ""),
        )
    }

    override fun setView() {
        adapter = TransHistoryAdapter(
            arrayListOf(
                TransHistory("Nhà của Hiếu"),
                TransHistory("Nhà của Nhật"),
                TransHistory("Nhà của Thanh"),
                TransHistory("Nhà của Nam"),
                TransHistory("Nhà của Vũ"),
                TransHistory("Nhà của Tiến"),
                TransHistory("Nhà của Alo")
            ),
            this
        )
        binding.rvTransHis.adapter = adapter
        val layoutManager =
            LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.rvTransHis.layoutManager = layoutManager
    }

    override fun onBtnRateClick() {
        findNavController().navigate(R.id.action_transHistoryFragment_to_rateBottomSheetFragment)
    }

    override fun OnItemTransClicked() {
        findNavController().navigate(R.id.action_transHistoryFragment_to_requestDetailFragment)
    }
}