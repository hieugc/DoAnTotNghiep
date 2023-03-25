package com.example.homex.activity.home.request

import android.os.Bundle
import android.util.Log
import android.view.View
import android.widget.Toast
import androidx.core.os.bundleOf
import androidx.fragment.app.viewModels
import androidx.navigation.fragment.findNavController
import androidx.navigation.fragment.navArgs
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.app.USER_ACCESS
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentCreateRequestBinding
import com.example.homex.extension.*
import com.example.homex.viewmodel.RequestViewModel
import com.example.homex.viewmodel.YourHomeViewModel
import com.google.android.material.datepicker.CalendarConstraints
import com.google.android.material.datepicker.DateValidatorPointForward
import com.google.android.material.datepicker.MaterialDatePicker
import com.homex.core.model.CalendarDate
import com.homex.core.model.Home
import com.homex.core.param.request.CreateRequestParam
import com.homex.core.util.AppEvent
import com.homex.core.util.PrefUtil
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel
import java.util.*


class CreateRequestFragment : BaseFragment<FragmentCreateRequestBinding>() {
    companion object{
        const val MILLIS_IN_A_DAY = 1000 * 60 * 60 * 24
    }
    override val layoutId: Int = R.layout.fragment_create_request
//    private lateinit var dateRangePicker: MaterialDatePicker<androidx.core.util.Pair<Long, Long>>
//    private lateinit var builder: MaterialDatePicker.Builder<androidx.core.util.Pair<Long, Long>>
//    private lateinit var constraintBuilder: CalendarConstraints.Builder
//    private var selectedDate: androidx.core.util.Pair<Long, Long> = androidx.core.util.Pair(MaterialDatePicker.todayInUtcMilliseconds(), MaterialDatePicker.todayInUtcMilliseconds() + 7 * MILLIS_IN_A_DAY)
//    private var selection: Pair<CalendarDate?, CalendarDate?> = Pair(null, null)
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
//            val startDate = dates.first?.time?.time?.longToDate()
//            val endDate = dates.second?.time?.time?.longToDate()
//            binding.fromDateTV.text =  startDate
//            binding.toDateTV.text = endDate
//            Log.e("betweenDate",  "${startDate.betweenDays(endDate)}")
//            selection = dates
            viewModel.startDate.postValue(dates.first)
            viewModel.endDate.postValue(dates.second)
        }

        findNavController().currentBackStackEntry?.savedStateHandle?.getLiveData<Home>("SWAP_HOUSE")?.observe(viewLifecycleOwner){
            viewModel.houseSwap.postValue(it)
        }

        findNavController().currentBackStackEntry?.savedStateHandle?.getLiveData<Home>("TARGET_HOUSE")?.observe(viewLifecycleOwner){
            viewModel.house.postValue(it)
        }


        if(args.targetHome != null){
            viewModel.house.postValue(args.targetHome)
        }
    }

    override fun setView() {

        //Date Range Picker
//        val calendar = Calendar.getInstance(TimeZone.getDefault())
//        calendar.clear()
//
//        val today = MaterialDatePicker.todayInUtcMilliseconds()
//        calendar.timeInMillis = today
//
//        calendar.add(Calendar.YEAR, 5)
//        val next_year = calendar.timeInMillis
//
//        constraintBuilder = CalendarConstraints.Builder()
//        constraintBuilder.setOpenAt(selectedDate.first)
//        constraintBuilder.setStart(today)
//        constraintBuilder.setEnd(next_year)
//        constraintBuilder.setValidator(DateValidatorPointForward.now())
//
//        builder = MaterialDatePicker.Builder.dateRangePicker()
//            .setTitleText("Chọn khoảng thời gian trao đổi")
//            .setSelection(selectedDate)
//            .setCalendarConstraints(constraintBuilder.build())
//        dateRangePicker = builder.build()

        if(args.startDate != null && args.endDate != null){
            viewModel.startDate.postValue(args.startDate)
            viewModel.endDate.postValue(args.endDate)
//            selection = Pair(
//                args.startDate,
//                args.endDate
//            )
//            binding.fromDateTV.text = selection.first?.time?.time?.longToDate()
//            binding.toDateTV.text = selection.second?.time?.time?.longToDate()
        }
        else {
            val cal = Calendar.getInstance()
            val first = CalendarDate(cal.time, cal.get(Calendar.DAY_OF_MONTH).toString())
            Log.e("first", cal.get(Calendar.DAY_OF_MONTH).toString())
            cal.add(Calendar.DATE, 7)
            val second = CalendarDate(cal.time, cal.get(Calendar.DAY_OF_MONTH).toString())
            Log.e("second", cal.get(Calendar.DAY_OF_MONTH).toString())
//            selection = Pair(
//                first, second
//            )
            viewModel.startDate.postValue(first)
            viewModel.endDate.postValue(second)
//            binding.fromDateTV.text = first.time?.time?.longToDate()
//            binding.toDateTV.text = second.time?.time?.longToDate()
        }
    }



    override fun setEvent() {
        binding.homeRB.setOnCheckedChangeListener { _, b ->
            if (b){
                viewModel.type.postValue(2)
            }
        }

        binding.pointRB.setOnCheckedChangeListener { _, b ->
            if (b){
                viewModel.type.postValue(1)
            }
        }

        binding.changeDateBtn.setOnClickListener {
//            dateRangePicker.show(parentFragmentManager, "Chọn ngày trao đổi")
            val action = CreateRequestFragmentDirections.actionCreateRequestFragmentToBottomSheetChangeDateFragment(viewModel.startDate.value, viewModel.endDate.value)
            findNavController().navigate(action)
//            findNavController().navigate(R.id.action_createRequestFragment_to_bottomSheetChangeDateFragment)
        }

//        dateRangePicker.addOnPositiveButtonClickListener { sel->
//            updatePicker(sel)
//        }

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
            AppEvent.showLoading()
            if (viewModel.type.value == 2){
                if (viewModel.house.value == null || viewModel.houseSwap.value == null){
                    AppEvent.hideLoading()
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
                AppEvent.hideLoading()
                AppEvent.showPopUpError("Hệ thống không tạo được yêu cầu\nVui lòng thử lại sau.")
            }else{
                if (viewModel.house.value == null){
                    AppEvent.hideLoading()
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
                AppEvent.hideLoading()
                AppEvent.showPopUpError("Hệ thống không tạo được yêu cầu\nVui lòng thử lại sau.")
            }
        }
    }

//    private fun updatePicker(sel: androidx.core.util.Pair<Long, Long>) {
//        binding.fromDateTV.text = sel.first.longToDate()
//        binding.toDateTV.text = sel.second.longToDate()
//        selectedDate = sel
//        constraintBuilder.setOpenAt(sel.first)
//        dateRangePicker =
//            builder.setSelection(sel).setCalendarConstraints(constraintBuilder.build())
//                .build()
//        dateRangePicker.addOnPositiveButtonClickListener {
//            updatePicker(it)
//        }
//    }


    override fun setViewModel() {
        requestViewModel.messageLiveData.observe(viewLifecycleOwner){
            if (it != null){
                Toast.makeText(requireContext(), "Tạo yêu cầu thành công", Toast.LENGTH_LONG).show()
                findNavController().popBackStack()
            }
            AppEvent.hideLoading()
        }
    }
}