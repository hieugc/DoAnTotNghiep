package com.example.homex.activity.home.addhome

import android.net.Uri
import android.os.Bundle
import android.util.Log
import android.view.View
import androidx.navigation.fragment.findNavController
import androidx.viewpager2.widget.ViewPager2
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.AddHomeViewPager
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentAddHomeBinding
import com.example.homex.extension.disable
import com.example.homex.extension.enable
import com.example.homex.extension.gone
import com.example.homex.extension.visible


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

    fun openBottomSheet(){
        findNavController().navigate(R.id.action_addHomeFragment_to_bottomSheetDialogSelectImage)
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
                when(position){
                    0 -> {
                        binding.prevSlideBtn.gone()
                        binding.nextSlideBtn.visible()
                    }
                    3->{
                        binding.prevSlideBtn.gone()
                        binding.nextSlideBtn.gone()
                    }
                    else->{
                        binding.prevSlideBtn.visible()
                        binding.nextSlideBtn.visible()
                    }
                }
            }
        })

        binding.btnNextSlide.setOnClickListener {
            binding.addHomeViewPager.currentItem = binding.addHomeViewPager.currentItem + 1
        }

        binding.btnPreviousSlide.setOnClickListener {
            binding.addHomeViewPager.currentItem = binding.addHomeViewPager.currentItem - 1
        }

        binding.nextSlideBtn.setOnClickListener {
            binding.addHomeViewPager.currentItem = binding.addHomeViewPager.currentItem + 1
        }

        binding.prevSlideBtn.setOnClickListener {
            binding.addHomeViewPager.currentItem = binding.addHomeViewPager.currentItem - 1
        }
    }
}