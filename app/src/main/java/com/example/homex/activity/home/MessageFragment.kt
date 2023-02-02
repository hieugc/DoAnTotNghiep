package com.example.homex.activity.home

import android.os.Bundle
import android.view.View
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.R
import com.example.homex.adapter.MessageBoxAdapter
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentMessageBinding
import com.homex.core.model.MessageBox


class MessageFragment : BaseFragment<FragmentMessageBinding>() {
    override val layoutId: Int = R.layout.fragment_message

    private lateinit var adapter: MessageBoxAdapter

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showMenu = false,
            showMessage = false,
            showTitleApp = Pair(true , "Trò chuyện"),
            showBottomNav = false,
            showBoxChatLayout = Pair(false, ""),
        )
    }

    override fun setView() {
        adapter = MessageBoxAdapter(
            arrayListOf(
                MessageBox(id = "1", user="Lộc Minh Hiếu", preview = "Hello my friend"),
                MessageBox(id = "1", user="Lộc Minh Hiếu", preview = "Hello my friend Hello my friend Hello my friend"),
                MessageBox(id = "1", user="Lộc Minh Hiếu Lộc Minh Hiếu", preview = "Hello my friend Hello my friend"),
                MessageBox(id = "1", user="Lộc Minh Hiếu Lộc Minh Hiếu Lộc Minh Hiếu", preview = "Hello my friend Hello my friend Hello my friend Hello my friend"),
            ),
            onClick = {
                findNavController().navigate(R.id.action_messageFragment_to_messageBoxFragment)
            }
        )
        binding.messageBoxRecView.adapter = adapter
        val layoutManager = LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.messageBoxRecView.layoutManager = layoutManager
    }
}