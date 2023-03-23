package com.example.homex

import android.net.Uri
import android.os.Bundle
import android.util.Log
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.navigation.fragment.findNavController
import androidx.navigation.fragment.navArgs
import androidx.recyclerview.widget.GridLayoutManager
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.LinearSnapHelper
import androidx.viewpager2.widget.ViewPager2
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.HomeRatingAdapter
import com.example.homex.adapter.ImageSlideAdapter
import com.example.homex.adapter.SimilarHomeAdapter
import com.example.homex.adapter.UtilAdapter
import com.example.homex.app.HOME
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentHomeDetailBinding
import com.example.homex.extension.betweenDays
import com.example.homex.extension.longToDate
import com.example.homex.extension.visible
import com.example.homex.utils.CenterZoomLayoutManager
import com.example.homex.viewmodel.YourHomeViewModel
import com.google.android.material.tabs.TabLayoutMediator
import com.homex.core.model.CalendarDate
import com.homex.core.model.Home
import com.homex.core.model.ImageBase
import com.homex.core.util.AppEvent
import org.koin.androidx.viewmodel.ext.android.viewModel
import java.util.*


class HomeDetailFragment : BaseFragment<FragmentHomeDetailBinding>() {
    override val layoutId: Int = R.layout.fragment_home_detail
    private lateinit var adapter: ImageSlideAdapter
    private lateinit var utilAdapter: UtilAdapter
    private lateinit var rulesAdapter: UtilAdapter
    private lateinit var ratingAdapter: HomeRatingAdapter
    private lateinit var similarHomeAdapter: SimilarHomeAdapter
    private val args: HomeDetailFragmentArgs by navArgs()
    private val viewModel: YourHomeViewModel by viewModel()
    private var selection: Pair<CalendarDate?, CalendarDate?> = Pair(null, null)

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showMessage = false,
            showMenu = false,
            showTitleApp = Pair(true, "Thông tin căn nhà"),
            showBottomNav = false,
            showLogo = false,
            showBoxChatLayout = Pair(false, null),
        )
        viewModel.getHomeDetails(args.id)

        findNavController().currentBackStackEntry?.savedStateHandle?.getLiveData<Pair<CalendarDate?, CalendarDate?>>("DATE")?.observe(viewLifecycleOwner){
                dates->
            val startDate = dates.first?.time?.time?.longToDate()
            val endDate = dates.second?.time?.time?.longToDate()
            binding.homeDateTV.text =  "$startDate - $endDate"
            binding.dayCountTV.text = "${startDate.betweenDays(endDate)} ngày"
            Log.e("betweenDate",  "${startDate.betweenDays(endDate)}")
            selection = dates
        }
    }

    override fun setViewModel() {
        viewModel.homeDetailsLiveData.observe(this){
            if (it != null){
                binding.home = it
                adapter.imgList = it.images
                utilAdapter.itemList = it.utilities
                if(it.utilities != null){
                    if(it.utilities!!.size > 4){
                        binding.showAllUtil.visible()
                    }
                }
                rulesAdapter.itemList = it.rules
                adapter.notifyDataSetChanged()
                utilAdapter.notifyDataSetChanged()
                rulesAdapter.notifyDataSetChanged()
            }
            AppEvent.hideLoading()
        }
    }

    override fun setView() {
        ratingAdapter = HomeRatingAdapter(
            arrayListOf(
                "Nhà đẹp lắm mọi người",
                "Nhà thoải mái, đẹp",
                "Hoàn toàn tuyệt vời"
            )
        )
        binding.homeRatingRecView.adapter = ratingAdapter
        val layoutManager = CenterZoomLayoutManager(requireContext(), LinearLayoutManager.HORIZONTAL, false, mShrinkAmount = 0.05f, mShrinkDistance = 0.8f)
        binding.homeRatingRecView.layoutManager = layoutManager

        val snapHelper = LinearSnapHelper()
        snapHelper.attachToRecyclerView(binding.homeRatingRecView)


        similarHomeAdapter = SimilarHomeAdapter(
            arrayListOf(
                "Nhà của lộc",
                "Nhà của phạm",
                "Nhà của nhật"
            )
        )
        binding.homeSimilarRecView.adapter = similarHomeAdapter
        val layoutManager2 = CenterZoomLayoutManager(requireContext(), LinearLayoutManager.HORIZONTAL, false, mShrinkAmount = 0.05f, mShrinkDistance = 0.8f)
        binding.homeSimilarRecView.layoutManager = layoutManager2

        val snapHelper2 = LinearSnapHelper()
        snapHelper2.attachToRecyclerView(binding.homeSimilarRecView)

        utilAdapter = UtilAdapter(arrayListOf(), showAll =  false, rule = false)
        binding.utilRecView.adapter = utilAdapter
        binding.utilRecView.layoutManager = GridLayoutManager(requireContext(), 2)

        rulesAdapter = UtilAdapter(arrayListOf(), showAll =  false, rule = true)
        binding.rulesRecView.adapter = rulesAdapter
        binding.rulesRecView.layoutManager = GridLayoutManager(requireContext(), 2)

        setupViewPager()
        setupTabLayout()

        val cal = Calendar.getInstance()
        val first = CalendarDate(cal.time, cal.get(Calendar.DAY_OF_MONTH).toString())
        Log.e("first", cal.get(Calendar.DAY_OF_MONTH).toString())
        cal.add(Calendar.DATE, 7)
        val second = CalendarDate(cal.time, cal.get(Calendar.DAY_OF_MONTH).toString())
        Log.e("second", cal.get(Calendar.DAY_OF_MONTH).toString())
        selection = Pair(
            first, second
        )
        val from = first.time?.time?.longToDate()
        val to = second.time?.time?.longToDate()
        binding.homeDateTV.text = "$from - $to"
        binding.dayCountTV.text = "7 ngày"
    }

    private fun setupTabLayout(){
        TabLayoutMediator(binding.tabLayout, binding.imgSlideViewPager){ _, _ ->}.attach()

        binding.imgSlideViewPager.registerOnPageChangeCallback(object : ViewPager2.OnPageChangeCallback(){
            override fun onPageSelected(position: Int) {
                binding.tabLayout.selectTab(binding.tabLayout.getTabAt(position))
            }
        })
    }

    private fun setupViewPager(){
        adapter = ImageSlideAdapter(listOf())
        binding.imgSlideViewPager.adapter = adapter
        binding.imgSlideViewPager.offscreenPageLimit = 3
        binding.imgSlideViewPager.clipToOutline = false
        binding.imgSlideViewPager.clipToPadding = false
    }

    override fun setEvent() {
        binding.contactBtn.setOnClickListener {
            findNavController().navigate(R.id.action_homeDetailFragment_to_messageBoxFragment)
        }
        binding.changeDateBtn.setOnClickListener {
            val action = HomeDetailFragmentDirections.actionHomeDetailFragmentToBottomSheetChangeDateFragment(selection.first, selection.second)
            findNavController().navigate(action)
        }
        binding.createRequestBtn.setOnClickListener {
            val action = HomeDetailFragmentDirections.actionHomeDetailFragmentToCreateRequestFragment(selection.first, selection.second)
            findNavController().navigate(action)
        }
        binding.showAllUtil.setOnClickListener {
            if(utilAdapter.showAll){
                binding.showAllUtil.text = "Xem thêm"
                utilAdapter.showAll = false
            }else{
                binding.showAllUtil.text = "Ẩn bớt"
                utilAdapter.showAll = true
            }
            utilAdapter.notifyDataSetChanged()

        }
    }
}