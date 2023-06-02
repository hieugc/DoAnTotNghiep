package com.example.homex.activity.home.message

import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.view.View
import androidx.core.view.isGone
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.MessageBoxAdapter
import com.example.homex.app.ID
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentMessageBinding
import com.example.homex.extension.gone
import com.example.homex.extension.visible
import com.example.homex.viewmodel.ChatViewModel
import com.homex.core.model.MessageRoom
import com.homex.core.util.AppEvent
import com.homex.core.util.PrefUtil
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.sharedViewModel
import org.koin.androidx.viewmodel.ext.android.viewModel


class MessageFragment : BaseFragment<FragmentMessageBinding>() {
    override val layoutId: Int = R.layout.fragment_message
    private val viewModel: ChatViewModel by viewModel()
    private val prefUtil: PrefUtil by inject()
    private val boxChatList = arrayListOf<MessageRoom>()
    private lateinit var adapter: MessageBoxAdapter
    private var page = 0
    private var isShimmer = true
    private val sharedViewModel: ChatViewModel by sharedViewModel()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        viewModel.getChatRoom(page++)
        arguments?.getInt(ID)?.let {
            if (it == 0)
                return@let
            Handler(Looper.getMainLooper()).post {
                findNavController().navigate(MessageFragmentDirections.actionMessageFragmentToMessageBoxFragment(it))
                arguments?.clear()
            }
        }
        viewModel.chatRoom.observe(this){
            if (it != null){
                val rooms = it.rooms
                if(rooms != null){
                    boxChatList.addAll(rooms)
                    adapter.notifyDataSetChanged()
                    if (boxChatList.isEmpty()){
                        binding.messageShimmer.stopShimmer()
                        binding.messageShimmer.gone()
                        isShimmer = false
                    }else{
                        if (isShimmer){
                            binding.messageShimmer.stopShimmer()
                            binding.messageShimmer.gone()
                            isShimmer = false
                        }
                        binding.messageBoxRecView.visible()
                    }
                }else{
                    binding.messageShimmer.stopShimmer()
                    binding.messageShimmer.gone()
                    isShimmer = false
                    binding.messageBoxRecView.gone()
                }
            }else{
                binding.messageShimmer.stopShimmer()
                binding.messageShimmer.gone()
                isShimmer = false
                binding.messageBoxRecView.gone()
            }
            if (binding.messageShimmer.isGone)
                AppEvent.closePopup()
        }
        sharedViewModel.newMessage.observe(this){
            if (it != null){
                var c : MessageRoom? = null
                for ((index, chat) in boxChatList.withIndex()){
                    if (chat.idRoom == it.idRoom){
                        boxChatList.removeAt(index)
                        c = chat
                        adapter.notifyItemRemoved(index)
                        break
                    }
                }
                if (c != null){
                    if (!it.messages.isNullOrEmpty()){
                        c.messages?.addAll(0, it.messages!!)
                    }
                    if (c.messages?.isNotEmpty() == true){
                        c.messages?.first()?.isSeen = c.messages?.first()?.idSend == prefUtil.profile?.userAccess
                    }
                    boxChatList.add(0, c)
                    adapter.notifyItemInserted(0)
                }
                sharedViewModel.newMessage.postValue(null)
            }
        }
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showMenu = false,
            showMessage = false,
            showTitleApp = Pair(true , "Trò chuyện"),
            showBottomNav = false,
            showBoxChatLayout = Pair(false, null),
        )
        binding.messageShimmer.gone()
        if (isShimmer){
            binding.messageShimmer.startShimmer()
            binding.messageShimmer.visible()
            binding.messageBoxRecView.visibility = View.INVISIBLE
        }
        initSwipeLayout()
    }

    private fun initSwipeLayout(){
        binding.swipeRefreshLayout.setOnRefreshListener {
            if (!isShimmer){
                AppEvent.showPopUp()
                isShimmer = true
                binding.messageShimmer.startShimmer()
                binding.messageShimmer.visible()
                boxChatList.clear()
                binding.messageBoxRecView.visibility = View.INVISIBLE
                page = 0
                viewModel.getChatRoom(page++)
                binding.swipeRefreshLayout.isRefreshing = false
            } else {
                binding.swipeRefreshLayout.isRefreshing = false
            }
        }
    }

    override fun setView() {
        adapter = MessageBoxAdapter(
            boxChatList,
            onClick = { room, pos->
                room.idRoom?.let {
                    findNavController().navigate(MessageFragmentDirections.actionMessageFragmentToMessageBoxFragment(it))
                }
                if (room.messages?.isNotEmpty() == true){
                    room.messages?.first()?.isSeen = true
                    adapter.notifyItemChanged(pos)
                }
            },
            prefUtil.profile?.userAccess
        )
        binding.messageBoxRecView.adapter = adapter
        val layoutManager = LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.messageBoxRecView.layoutManager = layoutManager
    }
}