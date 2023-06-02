package com.example.homex.activity.home.history

import android.os.Bundle
import android.view.View
import androidx.core.os.bundleOf
import androidx.navigation.fragment.findNavController
import androidx.navigation.fragment.navArgs
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.activity.home.homepage.HomeDetailFragmentDirections
import com.example.homex.activity.home.pending.PendingRequestDetailFragmentArgs
import com.example.homex.app.CONTACT_USER
import com.example.homex.app.ID
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentRequestDetailBinding
import com.example.homex.extension.RequestStatus
import com.example.homex.viewmodel.ChatViewModel
import com.example.homex.viewmodel.RequestViewModel
import com.google.android.material.dialog.MaterialAlertDialogBuilder
import com.homex.core.param.chat.ContactUserParam
import com.homex.core.param.request.UpdateStatusParam
import com.homex.core.util.AppEvent
import com.homex.core.util.PrefUtil
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class RequestDetailFragment : BaseFragment<FragmentRequestDetailBinding>() {
    override val layoutId: Int = R.layout.fragment_request_detail
    private val viewModel: RequestViewModel by viewModel()
    private val args: PendingRequestDetailFragmentArgs by navArgs()
    private val chatViewModel: ChatViewModel by viewModel()
    private val prefUtil : PrefUtil by inject()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        chatViewModel.connectToUser.observe(this){ messageRoom->
            if (messageRoom != null){
                messageRoom.idRoom?.let {
                    findNavController().navigate(
                        R.id.action_global_messageFragment, bundleOf(
                            ID to it,
                            CONTACT_USER to true
                        )
                    )
                }
            }
        }
    }
    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo =  false,
            showTitleApp = Pair(true, "Chi tiết yêu cầu"),
            showBottomNav = false,
            showSearchLayout = false,
            showMessage = false,
            showMenu = false,
            showBoxChatLayout = Pair(false, null),
        )

        if (args.id != 0){
            viewModel.getRequestDetail(args.id)
        }
    }

    override fun setEvent() {
        binding.acceptBtn.setOnClickListener {
            when(binding.acceptBtn.text){
                getString(R.string.check_in)->{
                    MaterialAlertDialogBuilder(requireContext())
                        .setTitle(getString(R.string.check_in))
                        .setMessage(getString(R.string.check_in_request_message))
                        .setNegativeButton(resources.getString(R.string.cancel)) { _, _ ->
                            // Respond to negative button press
                        }
                        .setPositiveButton(resources.getString(R.string.confirm)) { dialog, _ ->
                            // Respond to positive button press
                            dialog.dismiss()
                            if (args.id != 0){
                                val param = UpdateStatusParam(
                                    id = args.id,
                                    status = RequestStatus.CHECK_IN.ordinal
                                )
                                viewModel.updateStatus(param)
                            }
                        }
                        .show()
                }

                getString(R.string.check_out)->{
                    MaterialAlertDialogBuilder(requireContext())
                        .setTitle(getString(R.string.check_out))
                        .setMessage(getString(R.string.check_out_request_message))
                        .setNegativeButton(resources.getString(R.string.cancel)) { _, _ ->
                            // Respond to negative button press
                        }
                        .setPositiveButton(resources.getString(R.string.confirm)) { dialog, _ ->
                            // Respond to positive button press
                            dialog.dismiss()
                            if (args.id != 0){
                                val param = UpdateStatusParam(
                                    id = args.id,
                                    status = RequestStatus.REVIEWING.ordinal
                                )
                                viewModel.updateStatus(param)
                            }
                        }
                        .show()
                }

                getString(R.string.rate)->{
                    val action =
                        RequestDetailFragmentDirections.actionRequestDetailFragmentToRateBottomSheetFragment(
                            binding.request
                        )
                    findNavController().navigate(action)
                }
                getString(R.string.view_rating)->{
                    val action =
                        RequestDetailFragmentDirections.actionRequestDetailFragmentToRateBottomSheetFragment(
                            binding.request
                        )
                    findNavController().navigate(action)
                }
            }
        }

        binding.contactBtn.setOnClickListener {
            prefUtil.connectionId?.let { it1->
                binding.request?.house?.user?.userAccess?.let { it2->
                    chatViewModel.contactToUser(
                        ContactUserParam(
                            connectionId =  it1,
                            userAccess = it2
                        )
                    )
                }
            }
        }

        binding.targetHomeLayout.setOnClickListener {
            val id = binding.request?.house?.id?:return@setOnClickListener
            val action = HomeDetailFragmentDirections.actionGlobalHomeDetailFragment(id)
            findNavController().navigate(action)
        }

        binding.myHomeLayout.setOnClickListener {
            val id = binding.request?.swapHouse?.id?:return@setOnClickListener
            val action = HomeDetailFragmentDirections.actionGlobalHomeDetailFragment(id)
            findNavController().navigate(action)
        }
    }

    override fun setViewModel() {
        viewModel.requestResponseLiveData.observe(this){
            if (it != null){
                binding.request = it
            }
            AppEvent.closePopup()
        }
    }

}