package com.example.homex

import android.os.Bundle
import android.util.Log
import android.view.View
import androidx.core.content.ContextCompat
import androidx.navigation.fragment.findNavController
import androidx.navigation.fragment.navArgs
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.MessageAdapter
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentMessageBoxBinding
import com.example.homex.extension.formatIso8601ToFormat
import com.example.homex.extension.gone
import com.example.homex.extension.visible
import com.example.homex.viewmodel.ChatViewModel
import com.homex.core.model.Message
import com.homex.core.model.OldMessage
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


class MessageBoxFragment : BaseFragment<FragmentMessageBoxBinding>() {
    override val layoutId: Int = R.layout.fragment_message_box
    private val args: MessageBoxFragmentArgs by navArgs()
    private val viewModel: ChatViewModel by sharedViewModel()
    private lateinit var adapter: MessageAdapter
    private val messageList = arrayListOf<Message>()
    private val userMessages = arrayListOf<UserMessage>()
    private lateinit var body: RequestBody
    private var page = 0
    private val limit = 20
    private val prefUtil : PrefUtil by inject()

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showBottomNav = false,
            showTitleApp = Pair(false, ""),
            showMessage = false,
            showMenu = true,
            showBoxChatLayout = Pair(false, null),
        )
        val param = Pagination(page++, limit)
        if (args.id != 0){
            viewModel.getMessagesInChatRoom(param = GetMessagesParam(idRoom = args.id, param))
            val mediaType = "application/json".toMediaType()
            body = "\"${args.id}\"".toRequestBody(mediaType)
            viewModel.seenAll(body)
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
//
//        val dayOfWeekList =
//            listOf(
//                "T2", "T3", "T4", "T5", "T6", "T7", "CN"
//            )
//
//        val arrayAdapter = ArrayAdapter(requireContext(), R.layout.date_cell_item, dayOfWeekList)
//
//        binding.gridLayout.adapter = arrayAdapter
//
//        val calendar = Calendar.getInstance()
//        Log.e("year", "${calendar.get(Calendar.YEAR)}")
//        Log.e("month", "${calendar.get(Calendar.MONTH)}")
//        Log.e("dayOfMonth", "${calendar.get(Calendar.DAY_OF_MONTH)}")
//        Log.e("dayOfWeek", "${calendar.get(Calendar.DAY_OF_WEEK)}")
//        Log.e("maxDay", "${calendar.getActualMaximum(Calendar.DATE)}")
//        calendar.add(Calendar.MONTH, 1)
//
//        val arrayList = arrayListOf<String>()
//        val daysInMonth = calendar.getActualMaximum(Calendar.DATE)
//        calendar.set(Calendar.DAY_OF_MONTH, 1)
//        val dayOfWeek = calendar.get(Calendar.DAY_OF_WEEK) - 2
//        Log.e("dayOfWeek", "$dayOfWeek")
//        Log.e("daysInMonth", "$daysInMonth")
//
//
//        for(i in 1..42){
//            if(i <= dayOfWeek || i > daysInMonth + dayOfWeek){
//                arrayList.add("")
//            }else{
//                arrayList.add((i - dayOfWeek).toString())
//            }
//        }
//
//        monthAdapter = MyAdapter(
//            arrayList
//        )
//        binding.messageRecView.adapter = monthAdapter
//        val layoutManager = GridLayoutManager(requireContext(), 7)
//        binding.messageRecView.layoutManager = layoutManager

    }

    override fun setEvent() {
        binding.addBtn.setOnClickListener {
            if(binding.actionLayout.visibility == View.GONE)
            {
                binding.actionLayout.visible()
                binding.addBtn.setImageDrawable(ContextCompat.getDrawable(requireContext(), R.drawable.ic_close_circle))
            }
            else if(binding.actionLayout.visibility == View.VISIBLE)
            {
                binding.actionLayout.gone()
                binding.addBtn.setImageDrawable(ContextCompat.getDrawable(requireContext(), R.drawable.ic_pluscircle))
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
            Log.e("layout", "hello")
        }
    }

    override fun setViewModel() {
        viewModel.messages.observe(this){
            if(it != null){
                Log.e("messagesList", "${it.messages}")
                userMessages.clear()
                if(page == 1){
                    messageList.clear()
                }
                val messages = it.messages
                if (messages != null){
                    val tmpList = arrayListOf<Message>()
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
                    messageList.addAll(tmpList)
                }
                val users = it.userMessages
                if(users != null){
                    userMessages.addAll(users)
                    if (users.size > 0){
                        (activity as HomeActivity).setPropertiesScreen(
                            showLogo = false,
                            showBottomNav = false,
                            showTitleApp = Pair(false, ""),
                            showMessage = false,
                            showMenu = true,
                            showBoxChatLayout = Pair(true, it.userMessages?.get(0)),
                        )
                    }
                }
                adapter.notifyDataSetChanged()
            }
        }

        viewModel.seenAll.observe(this){
            if(it != null){
                Log.e("seenAll", "hello")
            }
        }

        viewModel.newMessage.observe(this){
            if (it != null){
                Log.e("newMessage", "${it.messages}")
                val messages = it.messages
                if (messages != null){
                    messageList.addAll(0, messages)
                    adapter.notifyDataSetChanged()
                    viewModel.seenAll(body)
                }
                viewModel.newMessage.postValue(null)
            }
        }

        viewModel.sendMessage.observe(this){
            if (it != null){
                Log.i("sendMessage", "Success")
            }
        }
    }

}