package com.example.homex.activity.home.message

import android.content.Context
import android.os.Bundle
import android.os.IBinder
import android.util.Log
import android.view.View
import android.view.inputmethod.InputMethodManager
import androidx.core.content.ContextCompat
import androidx.navigation.fragment.findNavController
import androidx.navigation.fragment.navArgs
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.MessageAdapter
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentMessageBoxBinding
import com.example.homex.extension.formatIso8601ToFormat
import com.example.homex.extension.gone
import com.example.homex.extension.visible
import com.example.homex.viewmodel.ChatViewModel
import com.homex.core.model.Message
import com.homex.core.model.UserMessage
import com.homex.core.param.chat.GetMessagesParam
import com.homex.core.param.chat.Pagination
import com.homex.core.param.chat.SendMessageParam
import com.homex.core.util.PrefUtil
import okhttp3.MediaType.Companion.toMediaType
import okhttp3.RequestBody
import okhttp3.RequestBody.Companion.toRequestBody
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.sharedViewModel
import org.koin.androidx.viewmodel.ext.android.viewModel


class MessageBoxFragment : BaseFragment<FragmentMessageBoxBinding>() {
    override val layoutId: Int = R.layout.fragment_message_box
    private val args: MessageBoxFragmentArgs by navArgs()
    private val viewModel: ChatViewModel by viewModel()
    private lateinit var adapter: MessageAdapter
    private val messageList = arrayListOf<Message>()
    private val userMessages = arrayListOf<UserMessage>()
    private lateinit var body: RequestBody
    private var page = 0
    private val limit = 20
    private val prefUtil : PrefUtil by inject()
    private var isShimmer = true
    private var userChat: UserMessage? = null
    private val sharedViewModel: ChatViewModel by sharedViewModel()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        val param = Pagination(page++, limit)
        if (args.id != 0){
            viewModel.getMessagesInChatRoom(param = GetMessagesParam(idRoom = args.id, param))
            val mediaType = "application/json".toMediaType()
            body = "\"${args.id}\"".toRequestBody(mediaType)
            viewModel.seenAll(body)
        }
        viewModel.messages.observe(this){
            if(it != null){
                val users = it.userMessages
                if(users != null){
                    userMessages.addAll(users)
                    if (users.size > 0){
                        userChat = users.first()
                        (activity as HomeActivity).setPropertiesScreen(
                            showLogo = false,
                            showBottomNav = false,
                            showTitleApp = Pair(false, ""),
                            showMessage = false,
                            showMenu = true,
                            showBoxChatLayout = Pair(true, userChat),
                        )
                    }
                }
                if(page == 1){
                    val size = messageList.size
                    messageList.clear()
                    adapter.notifyItemRangeRemoved(0, size)
                }
                val messages = it.messages
                val tmpList = arrayListOf<Message>()
                if (messages != null){
                    if (messages.size > 0){
                        var date = messages[0].createdDate
                        for((index, msg) in messages.withIndex()){
                            if(date?.formatIso8601ToFormat("dd/MM/yyyy") != msg.createdDate?.formatIso8601ToFormat("dd/MM/yyyy")){
                                tmpList.add(
                                    Message(
                                        createdDate = date,
                                        isDateItem = true
                                    )
                                )
                                date = msg.createdDate
                            }
                            tmpList.add(msg)
                            if(index == messages.size - 1){
                                tmpList.add(
                                    Message(
                                        createdDate = date,
                                        isDateItem = true
                                    )
                                )
                            }
                        }
                    }
                    val pos = messageList.size
                    messageList.addAll(tmpList)
                    adapter.notifyItemRangeInserted(pos, tmpList.size)
                    if (messageList.isEmpty()){
                        binding.messageShimmer.stopShimmer()
                        binding.messageShimmer.gone()
                        isShimmer = false
                    }else{
                        if (isShimmer){
                            binding.messageShimmer.stopShimmer()
                            binding.messageShimmer.gone()
                            isShimmer = false
                        }
                        binding.messageRecView.visible()
                    }
                }else{
                    binding.messageShimmer.stopShimmer()
                    binding.messageShimmer.gone()
                    isShimmer = false
                    binding.messageRecView.gone()
                }
            }else{
                binding.messageShimmer.stopShimmer()
                binding.messageShimmer.gone()
                isShimmer = false
                binding.messageRecView.gone()
            }
        }

        sharedViewModel.newMessage.observe(this){
            if (it != null){
                Log.d("newMessageBox", "${it.messages}")
                val messages = it.messages
                if (messages != null){
                    val date = messages[0].createdDate
                    var item : Message? = null
                    for(msg in messageList){
                        if (msg.isDateItem == true){
                            if(date?.formatIso8601ToFormat("dd/MM/yyyy") != msg.createdDate?.formatIso8601ToFormat("dd/MM/yyyy")){
                                item = Message(
                                    createdDate = date,
                                    isDateItem = true
                                )
                            }
                            break
                        }
                    }
                    if (item != null){
                        messageList.add(0, item)
                        adapter.notifyItemInserted(0)
                    }
                    messageList.addAll(0, messages)
                    adapter.notifyItemInserted(0)
                    val pos = messageList.size - 2
                    adapter.notifyItemChanged(pos)
                    viewModel.seenAll(body)
                    binding.messageRecView.smoothScrollToPosition(0)
                }
            }
        }
    }
    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showBottomNav = false,
            showTitleApp = Pair(false, ""),
            showMessage = false,
            showMenu = true,
            showBoxChatLayout = Pair(true, userChat),
        )
        binding.messageShimmer.gone()
        if (isShimmer){
            binding.messageShimmer.startShimmer()
            binding.messageShimmer.visible()
            binding.messageRecView.visibility = View.INVISIBLE
        }
    }

    override fun setView() {
        adapter = MessageAdapter(
            messageList,
            userMessages,
            prefUtil.profile?.userAccess
        )
        binding.messageRecView.adapter = adapter
        val layoutManager = LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, true)
        binding.messageRecView.layoutManager = layoutManager
        binding.messageRecView.setHasFixedSize(true)
    }

    override fun setEvent() {
        binding.addBtn.setOnClickListener {
            if(binding.actionLayout.visibility == View.GONE)
            {
                binding.actionLayout.visible()
                binding.addBtn.setImageDrawable(ContextCompat.getDrawable(requireContext(),
                    R.drawable.ic_close_circle
                ))
            }
            else if(binding.actionLayout.visibility == View.VISIBLE)
            {
                binding.actionLayout.gone()
                binding.addBtn.setImageDrawable(ContextCompat.getDrawable(requireContext(),
                    R.drawable.ic_pluscircle
                ))
            }
        }

        binding.createRequestBtn.setOnClickListener {
            if (userMessages.isNotEmpty()){
                val user = userMessages[0]
                user.userAccess?.let {
                    val action = MessageBoxFragmentDirections.actionMessageBoxFragmentToCreateRequestFragment(it)
                    findNavController().navigate(action)
                }
            }

        }
        binding.sendBtn.setOnClickListener {
            if(binding.msgEditText.text.toString() != ""){
                val param = SendMessageParam(
                    idRoom = args.id,
                    idReply = 0,
                    message = binding.msgEditText.text.toString()
                )
                viewModel.sendMessage(param)
                binding.msgEditText.setText("")
            }
        }
        binding.msgInputLayout.setOnClickListener {
        }
    }

    override fun setViewModel() {
        viewModel.seenAll.observe(this){}
        viewModel.sendMessage.observe(this){}
    }
    fun hideKeyboard(windowToken: IBinder){
        val imm =
            activity?.getSystemService(Context.INPUT_METHOD_SERVICE) as InputMethodManager
        imm.hideSoftInputFromWindow(windowToken, 0)
    }
}