package com.example.homex

import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.SearchHomeAdapter
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentPickYourHomeBinding

class PickYourHomeFragment : BaseFragment<FragmentPickYourHomeBinding>() {
    override val layoutId: Int = R.layout.fragment_pick_your_home
    private lateinit var adapter: SearchHomeAdapter

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showBottomNav = false,
            showMessage = false,
            showMenu = false,
            showTitleApp = Pair(true, "Chọn nhà"),
            showBoxChatLayout = Pair(false, null),
        )
    }

    override fun setView() {
        adapter = SearchHomeAdapter(
            arrayListOf(
                "Nhà của Hiếu",
                "Nhà của Phạm",
                "Nhà của Nhật"
            )
        )
        binding.pickYourHomeRecView.adapter = adapter
        val layoutManager = LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.pickYourHomeRecView.layoutManager = layoutManager
    }
}