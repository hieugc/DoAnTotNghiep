package com.example.homex.activity.home.request

import android.os.Bundle
import android.util.Log
import android.view.View
import androidx.core.os.bundleOf
import androidx.fragment.app.viewModels
import androidx.navigation.fragment.findNavController
import androidx.navigation.fragment.navArgs
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.app.USER_ACCESS
import com.example.homex.base.BaseActivity
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentCreateRequestBinding
import com.example.homex.extension.betweenDays
import com.example.homex.extension.longToFormat
import com.example.homex.viewmodel.RequestViewModel
import com.homex.core.model.CalendarDate
import com.homex.core.model.DateRange
import com.homex.core.model.Home
import com.homex.core.param.request.CreateRequestParam
import com.homex.core.util.AppEvent
import com.homex.core.util.PrefUtil
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel
import java.util.*


class CreateRequestFragment : BaseFragment<FragmentCreateRequestBinding>() {
    override val layoutId: Int = R.layout.fragment_create_request
    private val args: CreateRequestFragmentArgs by navArgs()
    private val viewModel: CreateRequestViewModel by viewModels()
    private val requestViewModel: RequestViewModel by viewModel()
    private val prefUtil: PrefUtil by inject()

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showTitleApp = Pair(true, "Tạo yêu cầu"),
            showMenu = false,
            showMessage = false,
            showBoxChatLayout = Pair(false, null),
            showBottomNav = false,
            showLogo = false
        )

        binding.viewModel = viewModel


        findNavController().currentBackStackEntry?.savedStateHandle?.getLiveData<Pair<CalendarDate?, CalendarDate?>>("DATE")?.observe(viewLifecycleOwner){
            dates->
            viewModel.startDate.postValue(dates.first)
            viewModel.endDate.postValue(dates.second)
        }

        findNavController().currentBackStackEntry?.savedStateHandle?.getLiveData<Home>("SWAP_HOUSE")?.observe(viewLifecycleOwner){
            viewModel.houseSwap.postValue(it)
            viewModel.myInValidRangeDates.postValue(it.inValidRangeDates)
        }

        findNavController().currentBackStackEntry?.savedStateHandle?.getLiveData<Home>("TARGET_HOUSE")?.observe(viewLifecycleOwner){
            viewModel.house.postValue(it)
            viewModel.inValidRangeDates.postValue(it.inValidRangeDates)
        }


        if(args.targetHome != null){
            viewModel.house.postValue(args.targetHome)
            viewModel.inValidRangeDates.postValue(args.targetHome?.inValidRangeDates)
        }
    }

    override fun setView() {
        if (prefUtil.profile != null){
            binding.yourPointTV.text = getString(R.string.point_you_have, prefUtil.profile?.point)
        }
        viewModel.type.postValue(args.type)
        if(args.startDate != null && args.endDate != null){
            viewModel.startDate.postValue(args.startDate)
            viewModel.endDate.postValue(args.endDate)
        }
        else {
            val cal = Calendar.getInstance()
            val first = CalendarDate(cal.time, cal.get(Calendar.DAY_OF_MONTH).toString())
            Log.e("first", cal.get(Calendar.DAY_OF_MONTH).toString())
            cal.add(Calendar.DATE, 7)
            val second = CalendarDate(cal.time, cal.get(Calendar.DAY_OF_MONTH).toString())
            Log.e("second", cal.get(Calendar.DAY_OF_MONTH).toString())
            viewModel.startDate.postValue(first)
            viewModel.endDate.postValue(second)
        }
    }



    override fun setEvent() {
        binding.homeRB.setOnCheckedChangeListener { _, b ->
            if (b){
                viewModel.type.postValue(2)
            }
        }
        binding.homeRBLayout.setOnClickListener {
            binding.homeRB.isChecked = true
        }

        binding.pointRB.setOnCheckedChangeListener { _, b ->
            if (b){
                viewModel.type.postValue(1)
            }
        }
        binding.pointRBLayout.setOnClickListener {
            binding.pointRB.isChecked = true
        }

        binding.changeDateBtn.setOnClickListener {
            val ranges = viewModel.inValidRangeDates.value?: arrayListOf()
            val myRange = viewModel.myInValidRangeDates.value?: arrayListOf()
            val invalid = arrayListOf<DateRange>()
            invalid.addAll(ranges)
            invalid.addAll(myRange)
            val action = CreateRequestFragmentDirections.actionCreateRequestFragmentToBottomSheetChangeDateFragment(viewModel.startDate.value, viewModel.endDate.value, inValidRangeDates = invalid.toTypedArray())
            findNavController().navigate(action)
        }

        binding.addYourHomeBtn.setOnClickListener {
            findNavController().navigate(R.id.action_createRequestFragment_to_pickYourHomeFragment)
        }

        binding.swapHome.root.setOnClickListener {
            findNavController().navigate(R.id.action_createRequestFragment_to_pickYourHomeFragment)
        }

        binding.addTargetHomeBtn.setOnClickListener {
            findNavController().navigate(
                R.id.action_createRequestFragment_to_pickHomeFragment, bundleOf(
                USER_ACCESS to args.userAccess
            ))
        }

        binding.targetHome.root.setOnClickListener {
            findNavController().navigate(
                R.id.action_createRequestFragment_to_pickHomeFragment, bundleOf(
                    USER_ACCESS to args.userAccess
                ))
        }

        binding.createRequestBtn.setOnClickListener {
            if (viewModel.type.value == 2){
                if (viewModel.house.value == null || viewModel.houseSwap.value == null){
                    AppEvent.showPopUpError("Hãy chọn các căn nhà để tạo yêu cầu")
                    return@setOnClickListener
                }
                val house = viewModel.house.value
                val idHouse = house?.id
                val swapHouse = viewModel.houseSwap.value
                val idSwapHouse = swapHouse?.id
                val price = 0
                val startDate = viewModel.startDate.value?.time?.time?.longToFormat("yyyy-MM-dd").toString()
                val endDate = viewModel.endDate.value?.time?.time?.longToFormat("yyyy-MM-dd").toString()
                if (idHouse != null && idSwapHouse != null){
                    val param = CreateRequestParam(
                        idHouse = idHouse,
                        type = 2,
                        price = price,
                        idSwapHouse = idSwapHouse,
                        startDate = startDate,
                        endDate = endDate
                        )
                    requestViewModel.createNewRequest(param)
                    return@setOnClickListener
                }
                AppEvent.showPopUpError("Hệ thống không tạo được yêu cầu\nVui lòng thử lại sau.")
            }else{
                if (viewModel.house.value == null){
                    AppEvent.showPopUpError("Hãy chọn căn nhà muốn trao đổi để tạo yêu cầu")
                    return@setOnClickListener
                }
                val house = viewModel.house.value
                val idHouse = house?.id
                val startDate = viewModel.startDate.value?.time?.time?.longToFormat("yyyy-MM-dd").toString()
                val endDate = viewModel.endDate.value?.time?.time?.longToFormat("yyyy-MM-dd").toString()
                val days = startDate.betweenDays(endDate)
                var price = 0
                val housePrice = house?.price
                if (days != null && housePrice != null){
                    price = days * housePrice
                }
                if (idHouse != null && price <= 0){
                    val param = CreateRequestParam(
                        idHouse = idHouse,
                        type = 1,
                        price = price,
                        idSwapHouse = null,
                        startDate = viewModel.startDate.value?.time?.time?.longToFormat("yyyy-MM-dd").toString(),
                        endDate = viewModel.endDate.value?.time?.time?.longToFormat("yyyy-MM-dd").toString()
                    )
                    requestViewModel.createNewRequest(param)
                    return@setOnClickListener
                }
                AppEvent.showPopUpError("Hệ thống không tạo được yêu cầu\nVui lòng thử lại sau.")
            }
        }
    }

    override fun setViewModel() {
        requestViewModel.messageLiveData.observe(viewLifecycleOwner){
            if (it != null){
                (activity as BaseActivity).displayMessage(getString(R.string.create_request_success))
                findNavController().popBackStack()
            }
            AppEvent.closePopup()
        }
    }
}