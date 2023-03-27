package com.example.homex.activity.home.message

import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.util.Log
import android.view.View
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
import com.homex.core.util.PrefUtil
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.sharedViewModel


class MessageFragment : BaseFragment<FragmentMessageBinding>() {
    override val layoutId: Int = R.layout.fragment_message
    private val viewModel: ChatViewModel by sharedViewModel()
    private val prefUtil: PrefUtil by inject()
    private val boxChatList = arrayListOf<MessageRoom>()
    private lateinit var adapter: MessageBoxAdapter
    private var page = 0
    private var isShimmer = true

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
        }
        viewModel.getChatRoom(page)

        arguments?.getInt(ID)?.let {
            if (it == 0)
                return@let
            Handler(Looper.getMainLooper()).post {
                findNavController().navigate(MessageFragmentDirections.actionMessageFragmentToMessageBoxFragment(it))
                arguments?.clear()
            }
        }
    }

    override fun setView() {
        adapter = MessageBoxAdapter(
            boxChatList,
            onClick = { room->
                room.idRoom?.let {
                    findNavController().navigate(MessageFragmentDirections.actionMessageFragmentToMessageBoxFragment(it))
                }
            },
            prefUtil.profile?.userAccess
        )
        binding.messageBoxRecView.adapter = adapter
        val layoutManager = LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.messageBoxRecView.layoutManager = layoutManager
    }

    override fun setViewModel() {
        viewModel.chatRoom.observe(this){
            if (it != null){
                if(page == 0){
                    val size = boxChatList.size
                    if (size > 0){
                        boxChatList.clear()
                        adapter.notifyItemRangeRemoved(0, size)
                    }
                }
                val rooms = it.rooms
                val pos = boxChatList.size
                if(rooms != null){
                    boxChatList.addAll(rooms)
                    binding.messageShimmer.stopShimmer()
                    binding.messageShimmer.gone()
                    isShimmer = false
                    adapter.notifyItemRangeInserted(pos, rooms.size)
                }else{
                    binding.messageShimmer.stopShimmer()
                    binding.messageShimmer.gone()
                    isShimmer = false
                }
            }else{
                binding.messageShimmer.stopShimmer()
                binding.messageShimmer.gone()
                isShimmer = false
            }
        }

        viewModel.newMessage.observe(this){
            if (it != null){
                Log.e("newBox", "${it.messages}")
                for ((index, chat) in boxChatList.withIndex()){
                    if (chat.idRoom == it.idRoom){
                        boxChatList.removeAt(index)
                        adapter.notifyItemRemoved(index)
                        break
                    }
                }
                boxChatList.add(0, it)
                adapter.notifyItemInserted(0)
                viewModel.newMessage.postValue(null)
            }
        }
    }
}