package com.example.homex.activity.home.addhome

import android.os.Bundle
import android.view.View
import androidx.viewpager2.widget.ViewPager2
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.AddHomeViewPager
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentAddHomeBinding


class AddHomeFragment : BaseFragment<FragmentAddHomeBinding>() {
    override val layoutId: Int = R.layout.fragment_add_home

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showBottomNav = false,
            showMessage = false,
            showMenu = false,
            showTitleApp = Pair(true, "Thêm nhà"),
            showBoxChatLayout = Pair(false, "")
        )
    }

    override fun setEvent() {
        val adapter = AddHomeViewPager(
            this,
            listOf(
                AddHome1Fragment(),
                AddHome2Fragment(),
                AddHome3Fragment(),
                AddHome4Fragment()
            )
        )
        binding.addHomeViewPager.adapter = adapter

        binding.addHomeViewPager.registerOnPageChangeCallback(object: ViewPager2.OnPageChangeCallback(){
            override fun onPageSelected(position: Int) {
                super.onPageSelected(position)
                binding.stepView.go(position, true)
            }
        })

        binding.btnNextSlide.setOnClickListener {
            binding.addHomeViewPager.currentItem = binding.addHomeViewPager.currentItem + 1
        }

        binding.btnPreviousSlide.setOnClickListener {
            binding.addHomeViewPager.currentItem = binding.addHomeViewPager.currentItem - 1
        }
    }
}