package com.example.homex

import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.MyHomeAdapter
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentMyHomeBinding


class MyHomeFragment : BaseFragment<FragmentMyHomeBinding>() {
    override val layoutId: Int = R.layout.fragment_my_home
    private lateinit var adapter: MyHomeAdapter

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showBottomNav = false,
            showTitleApp = Pair(true, "Nhà của bạn"),
            showMenu = false,
            showMessage = false,
            showBoxChatLayout = Pair(false, "")
        )
    }

    override fun setView() {
        adapter = MyHomeAdapter(
            arrayListOf(
                "Nhà của Hiếu", "Nhà của tui", "Nhà Nhà Nhà", "Hello mudafakar"
            ),
            onClick = {
                findNavController().navigate(R.id.action_global_myHomeDetailFragment)
            }
        )
        binding.mainHomeRecView.adapter = adapter
        val layoutManager = LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.mainHomeRecView.layoutManager = layoutManager
    }

    override fun setEvent() {
        binding.addHomeFAB.setOnClickListener {
            findNavController().navigate(R.id.action_myHomeFragment_to_addHomeFragment)
        }
    }
}