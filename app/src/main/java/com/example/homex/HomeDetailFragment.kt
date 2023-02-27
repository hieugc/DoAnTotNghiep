package com.example.homex

import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.LinearSnapHelper
import androidx.viewpager2.widget.ViewPager2
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.HomeRatingAdapter
import com.example.homex.adapter.ImageSlideAdapter
import com.example.homex.adapter.SimilarHomeAdapter
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentHomeDetailBinding
import com.example.homex.utils.CenterZoomLayoutManager
import com.google.android.material.tabs.TabLayoutMediator


class HomeDetailFragment : BaseFragment<FragmentHomeDetailBinding>() {
    override val layoutId: Int = R.layout.fragment_home_detail

    private lateinit var ratingAdapter: HomeRatingAdapter
    private lateinit var similarHomeAdapter: SimilarHomeAdapter

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showMessage = false,
            showMenu = false,
            showTitleApp = Pair(true, "Thông tin căn nhà"),
            showBottomNav = false,
            showLogo = false,
            showBoxChatLayout = Pair(false, "")
        )
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

        setupViewPager()
        setupTabLayout()
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
        val adapter = ImageSlideAdapter(
            listOf(
                "https://www.mymove.com/wp-content/uploads/2014/05/GettyImages-147205632.jpg",
                "https://assets-news.housing.com/news/wp-content/uploads/2022/03/31010142/Luxury-house-design-Top-10-tips-to-add-luxury-to-your-house-FEATURE-compressed.jpg",
                "https://sadvice.dxmb.vn/wp-content/uploads/2021/03/luxury-home.jpg",
            )
        )
        binding.imgSlideViewPager.adapter = adapter
        binding.imgSlideViewPager.offscreenPageLimit = 3
        binding.imgSlideViewPager.clipToOutline = false
        binding.imgSlideViewPager.clipToPadding = false
    }

    override fun setEvent() {
        binding.contactBtn.setOnClickListener {
        }
    }
}