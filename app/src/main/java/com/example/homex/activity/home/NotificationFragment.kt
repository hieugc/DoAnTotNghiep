package com.example.homex.activity.home

import android.os.Bundle
import android.view.View
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.R
import com.example.homex.activity.auth.AuthActivity
import com.example.homex.adapter.NotificationAdapter
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentNotificationBinding
import com.example.homex.extension.visible
import com.example.homex.viewmodel.NotificationViewModel
import com.homex.core.model.Notification
import com.homex.core.util.PrefUtil
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.sharedViewModel


class NotificationFragment : BaseFragment<FragmentNotificationBinding>() {
    override val layoutId: Int = R.layout.fragment_notification

    private val viewModel: NotificationViewModel by sharedViewModel()
    private val notificationList = arrayListOf<Notification>()
    private lateinit var adapter: NotificationAdapter
    private val prefUtil: PrefUtil by inject()

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

        if (prefUtil.token != null && prefUtil.profile != null){
            viewModel.getNotifications(1, 1000)
        }else{
            binding.goToAuthBtn.visible()
            binding.noHomeTxt.visible()
        }
    }

    override fun setView() {
        adapter = NotificationAdapter(
            notificationList,
            onClick = {
                idType, id ->
                if (id != null) {
                    viewModel.updateSeenNotification(id)
                }
                if (idType != null) {
                    val action =
                        NotificationFragmentDirections.actionNotificationFragmentToRequestDetailFragment(
                            id = idType
                        )
                    findNavController().navigate(action)
                }
            }
        )
        binding.notificationRecView.adapter = adapter
        val layoutManager =
            LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.notificationRecView.layoutManager = layoutManager
        binding.notificationRecView.setHasFixedSize(true)
    }

    override fun setEvent() {
        binding.readAllBtn.setOnClickListener {
            (activity as HomeActivity).showReadAllNotificationDialog()
        }
        binding.goToAuthBtn.setOnClickListener {
            startActivity(AuthActivity.open(requireContext()))
        }
    }

    override fun setViewModel() {
        viewModel.notificationListLiveDate.observe(viewLifecycleOwner) {
            if (it != null) {
                val listNotification = it.model
                if (listNotification != null) {
                    adapter.setList(listNotification)
                }
            }
        }

        viewModel.notificationLiveData.observe(this) {
            if (it != null) {
                adapter.add(it)
                adapter.notifyItemInserted(0)
                adapter.notifyItemRangeChanged(0, 1)
                viewModel.notificationLiveData.postValue(null)
            }
        }

        viewModel.notificationLiveMessage.observe(this) {
            viewModel.getNotifications(1,1000)
        }
    }

}