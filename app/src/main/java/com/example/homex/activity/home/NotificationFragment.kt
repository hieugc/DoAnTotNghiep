package com.example.homex.activity.home

import android.os.Bundle
import android.view.View
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.R
import com.example.homex.adapter.NotificationAdapter
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentNotificationBinding
import com.homex.core.model.Notification


class NotificationFragment : BaseFragment<FragmentNotificationBinding>() {
    override val layoutId: Int = R.layout.fragment_notification

    private lateinit var adapter: NotificationAdapter

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = true,
            showMenu = false,
            showMessage = true,
            showTitleApp = Pair(false, ""),
            showBottomNav = true,
            showBoxChatLayout = Pair(false, null),
        )
    }

    override fun setView() {
        adapter = NotificationAdapter(
            arrayListOf(
                Notification("1", "Hello 1", "Chào các bạn lô lô lô"),
                Notification("1","Hello 1", "Lê Minh Nhật đã yêu cầu trao đổi Nhà của Hiếu của bạn với Biệt thự ven biển của Lê Minh Nhật thông qua phương thức trao đổi nhà"),
                Notification("1","Hello 1", "Chào các bạn lô lô lô"),
            )
        )

        binding.notificationRecView.adapter = adapter
        val layoutManager = LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.notificationRecView.layoutManager = layoutManager

    }

    override fun setEvent() {
        binding.readAllBtn.setOnClickListener {
            (activity as HomeActivity).showReadAllNotificationDialog()
        }
    }

}