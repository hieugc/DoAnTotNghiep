package com.example.homex.activity.home

import android.os.Bundle
import android.view.View
import androidx.core.view.isGone
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.R
import com.example.homex.activity.auth.AuthActivity
import com.example.homex.adapter.NotificationAdapter
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentNotificationBinding
import com.example.homex.extension.NotificationType
import com.example.homex.extension.gone
import com.example.homex.extension.visible
import com.example.homex.viewmodel.NotificationViewModel
import com.example.homex.viewmodel.RequestViewModel
import com.homex.core.model.Notification
import com.homex.core.util.AppEvent
import com.homex.core.util.PrefUtil
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.sharedViewModel
import org.koin.androidx.viewmodel.ext.android.viewModel


class NotificationFragment : BaseFragment<FragmentNotificationBinding>() {
    override val layoutId: Int = R.layout.fragment_notification

    private val viewModel: NotificationViewModel by sharedViewModel()
    private val requestViewModel: RequestViewModel by viewModel()
    private val notificationList = arrayListOf<Notification>()
    private lateinit var adapter: NotificationAdapter
    private val prefUtil: PrefUtil by inject()
    private var isShimmer = true

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        viewModel.notificationListLiveDate.observe(this) {
            if (it != null) {
                notificationList.clear()
                val listNotification = it.model
                if(listNotification != null){
                    notificationList.addAll(listNotification)
                    adapter.notifyDataSetChanged()
                    if (notificationList.isEmpty()){
                        binding.shimmer.stopShimmer()
                        binding.shimmer.gone()
                        isShimmer = false
                        binding.noRequestLayout.visible()
                        binding.readAllBtn.gone()
                    }else{
                        if (isShimmer){
                            binding.shimmer.stopShimmer()
                            binding.shimmer.gone()
                            isShimmer = false
                        }
                        binding.notificationRecView.visible()
                        binding.noRequestLayout.gone()
                        binding.readAllBtn.visible()
                    }
                }else{
                    binding.shimmer.stopShimmer()
                    binding.shimmer.gone()
                    isShimmer = false
                    binding.notificationRecView.gone()
                    binding.noRequestLayout.visible()
                    binding.readAllBtn.gone()
                }
            }else{
                binding.shimmer.stopShimmer()
                binding.shimmer.gone()
                isShimmer = false
                binding.notificationRecView.gone()
                binding.noRequestLayout.visible()
                binding.readAllBtn.gone()
            }
            if (binding.shimmer.isGone)
                AppEvent.closePopup()
        }

        viewModel.notificationLiveData.observe(this) {
            if (it != null) {
                adapter.add(it)
                viewModel.notificationLiveData.postValue(null)
            }
        }

        requestViewModel.requestResponseLiveData.observe(this){
            val id = it?.request?.id
            if (id != null){
                if (it.request?.isOwner == true){
                    val action =
                        NotificationFragmentDirections.actionNotificationFragmentToRequestDetailFragment(
                            id = id
                        )
                    findNavController().navigate(action)
                }else if (it.request?.isOwner == false){
                    val action =
                        NotificationFragmentDirections.actionNotificationFragmentToPendingRequestDetailFragment(
                            id = id
                        )
                    findNavController().navigate(action)
                }
            }
            requestViewModel.requestResponseLiveData.postValue(null)
        }

    }
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
            binding.shimmer.gone()
            if (isShimmer){
                binding.shimmer.startShimmer()
                binding.shimmer.visible()
                binding.notificationRecView.visibility = View.INVISIBLE
            }
        }else{
            binding.goToAuthBtn.visible()
            binding.noHomeTxt.visible()
            binding.shimmer.gone()
        }
        initSwipeLayout()
    }

    private fun initSwipeLayout(){
        binding.swipeRefreshLayout.setOnRefreshListener {
            if (!isShimmer){
                AppEvent.showPopUp()
                isShimmer = true
                binding.shimmer.startShimmer()
                binding.shimmer.visible()
                notificationList.clear()
                adapter.notifyDataSetChanged()
                binding.noHomeTxt.gone()
                binding.goToAuthBtn.gone()
                binding.notificationRecView.visibility = View.INVISIBLE
                if (prefUtil.token != null && prefUtil.profile != null){
                    viewModel.getNotifications(1, 1000)
                }else{
                    binding.goToAuthBtn.visible()
                    binding.noHomeTxt.visible()
                    binding.shimmer.gone()
                }
                binding.swipeRefreshLayout.isRefreshing = false
            } else {
                binding.swipeRefreshLayout.isRefreshing = false
            }
        }
    }

    override fun setView() {
        adapter = NotificationAdapter(
            notificationList,
            onClick = {
                idType, id, type ->
                if (id != null) {
                    viewModel.updateSeenNotification(id)
                }
                if (idType != null) {
                    when(type){
                        NotificationType.REQUEST.ordinal -> {
                            requestViewModel.getRequestDetail(idType)
                        }
                        NotificationType.COMMENT.ordinal -> {

                        }
                        NotificationType.RESPONSE.ordinal -> {}
                        NotificationType.ADMIN_REPORT.ordinal -> {}
                        NotificationType.MESSAGE.ordinal -> {}
                        NotificationType.CIRCLE_SWAP.ordinal -> {}
                        NotificationType.PAYMENT.ordinal -> {

                        }
                    }
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

        viewModel.notificationLiveMessage.observe(this) {
            viewModel.getNotifications(1,1000)
        }
    }

}