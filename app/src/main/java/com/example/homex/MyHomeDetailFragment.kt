package com.example.homex

import android.os.Bundle
import android.util.Log
import android.view.*
import android.widget.Toast
import androidx.fragment.app.Fragment
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.viewpager2.widget.ViewPager2
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.HomeRatingAdapter
import com.example.homex.adapter.ImageSlideAdapter
import com.example.homex.app.HOME
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentMyHomeDetailBinding
import com.example.homex.utils.CenterZoomLayoutManager
import com.example.homex.viewmodel.YourHomeViewModel
import com.google.android.material.dialog.MaterialAlertDialogBuilder
import com.google.android.material.tabs.TabLayoutMediator
import com.homex.core.model.Home
import com.homex.core.model.ImageBase
import com.homex.core.param.yourhome.IdParam
import com.homex.core.util.AppEvent
import org.koin.androidx.viewmodel.ext.android.viewModel


class MyHomeDetailFragment : BaseFragment<FragmentMyHomeDetailBinding>() {
    override val layoutId: Int = R.layout.fragment_my_home_detail
    private lateinit var ratingAdapter: HomeRatingAdapter
    private var imgList: List<ImageBase>? = null
    private lateinit var adapter: ImageSlideAdapter
    private val viewModel: YourHomeViewModel by viewModel()
    private var actionMode: ActionMode? = null

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showMessage = false,
            showMenu = false,
            showTitleApp = Pair(true, "Thông tin nhà của bạn"),
            showBottomNav = false,
            showLogo = false,
            showBoxChatLayout = Pair(false, "")
        )

        arguments?.getParcelable<Home>(HOME)?.let {
            binding.home = it
            imgList = it.images
            adapter.imgList = imgList
        }
    }

    override fun setView() {
        val callback : ActionMode.Callback = object : ActionMode.Callback {

            override fun onCreateActionMode(mode: ActionMode?, menu: Menu?): Boolean {
                activity?.menuInflater?.inflate(R.menu.my_home_menu, menu)
                return true
            }

            override fun onPrepareActionMode(mode: ActionMode?, menu: Menu?): Boolean {
                return false
            }

            override fun onActionItemClicked(mode: ActionMode?, item: MenuItem?): Boolean {
                return when (item?.itemId) {
                    R.id.edit -> {
                        // Handle share icon press
                        true
                    }
                    R.id.delete -> {
                        // Handle delete icon press
                        MaterialAlertDialogBuilder(requireContext())
                            .setTitle("Xóa nhà")
                            .setMessage("Bạn có thật sự muốn xóa nhà này?")
                            .setNegativeButton(resources.getString(R.string.cancel)) { dialog, which ->
                                // Respond to negative button press
                            }
                            .setPositiveButton(resources.getString(R.string.confirm)) { dialog, which ->
                                // Respond to positive button press
                                dialog.dismiss()
                                binding.home?.id?.apply {
                                    viewModel.deleteHome(this)
                                }
                            }
                            .show()
                        true
                    }
                    else -> false
                }
            }

            override fun onDestroyActionMode(mode: ActionMode?) {
                findNavController().popBackStack()
            }
        }

        actionMode = activity?.startActionMode(callback)
        actionMode?.title = "Thông tin nhà của bạn"

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
        adapter = ImageSlideAdapter(
            listOf()
        )
        binding.imgSlideViewPager.adapter = adapter
        binding.imgSlideViewPager.offscreenPageLimit = 3
        binding.imgSlideViewPager.clipToOutline = false
        binding.imgSlideViewPager.clipToPadding = false
    }

    override fun setViewModel() {
        viewModel.messageLiveData.observe(viewLifecycleOwner){
            actionMode?.finish()
            Toast.makeText(requireContext(), "Xóa nhà thành công", Toast.LENGTH_LONG).show()
            AppEvent.hideLoading()
        }
    }
}