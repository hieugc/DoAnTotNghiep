package com.example.homex.activity.home

import android.os.Bundle
import android.view.View
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.R
import com.example.homex.adapter.NotificationAdapter
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentNotificationBinding
import com.example.homex.viewmodel.NotificationViewModel
import com.homex.core.model.Notification
import com.homex.core.param.notification.UpdateSeenNotificationParam
import org.koin.androidx.viewmodel.ext.android.sharedViewModel


class NotificationFragment : BaseFragment<FragmentNotificationBinding>() {
    override val layoutId: Int = R.layout.fragment_notification

    private val viewModel: NotificationViewModel by sharedViewModel()
    private val notificationList = arrayListOf<Notification>()
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

        viewModel.getNotifications(1, 1000)
    }

    override fun setView() {
        adapter = NotificationAdapter(
            notificationList,
            onClick = {
                viewModel.updateSeenNotification(UpdateSeenNotificationParam("1"))
                val action =
                    NotificationFragmentDirections.actionNotificationFragmentToRequestDetailFragment(
                        id = it
                    )
                findNavController().navigate(action)
            }
        )
        binding.notificationRecView.adapter = adapter
        val layoutManager =
            LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.notificationRecView.layoutManager = layoutManager
    }

    override fun setEvent() {
        binding.readAllBtn.setOnClickListener {
            (activity as HomeActivity).showReadAllNotificationDialog()
        }
    }

    override fun setViewModel() {
        viewModel.notificationListLiveDate.observe(viewLifecycleOwner) {
            if (it != null) {
                val listRequest = it.model
                if (listRequest != null) {
                    adapter.setList(listRequest)
                }
            }
        }

        viewModel.notificationLiveDate.observe(this) {
            if (it != null) {
                adapter.add(it)
                adapter.notifyItemInserted(0)
                adapter.notifyItemRangeChanged(0, 1)
                viewModel.notificationLiveDate.postValue(null)
            }
        }
    }

}