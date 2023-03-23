package com.example.homex

import android.os.Bundle
import android.util.Log
import android.view.*
import android.widget.Toast
import androidx.core.os.bundleOf
import androidx.fragment.app.Fragment
import androidx.navigation.fragment.findNavController
import androidx.navigation.fragment.navArgs
import androidx.recyclerview.widget.GridLayoutManager
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.viewpager2.widget.ViewPager2
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.HomeRatingAdapter
import com.example.homex.adapter.ImageSlideAdapter
import com.example.homex.adapter.UtilAdapter
import com.example.homex.app.HOME
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentMyHomeDetailBinding
import com.example.homex.extension.visible
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
    private lateinit var utilAdapter: UtilAdapter
    private lateinit var rulesAdapter: UtilAdapter
    private var editFinish = false
    private lateinit var adapter: ImageSlideAdapter
    private val viewModel: YourHomeViewModel by viewModel()
    private val args: MyHomeDetailFragmentArgs by navArgs()
    private var actionMode: ActionMode? = null

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showMessage = false,
            showMenu = false,
            showTitleApp = Pair(true, "Thông tin nhà của bạn"),
            showBottomNav = false,
            showLogo = false,
            showBoxChatLayout = Pair(false, null),
        )

        viewModel.getHomeDetails(args.id)
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
                        editFinish = true
                        actionMode?.finish()
                        findNavController().navigate(R.id.action_myHomeDetailFragment_to_addHomeFragment, bundleOf(
                            HOME to binding.home))
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
                if(!editFinish)
                    findNavController().popBackStack()
                else
                    editFinish = false
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

        utilAdapter = UtilAdapter(arrayListOf(), showAll =  false, rule = false)
        binding.utilRecView.adapter = utilAdapter
        binding.utilRecView.layoutManager = GridLayoutManager(requireContext(), 2)

        rulesAdapter = UtilAdapter(arrayListOf(), showAll =  false, rule = true)
        binding.rulesRecView.adapter = rulesAdapter
        binding.rulesRecView.layoutManager = GridLayoutManager(requireContext(), 2)

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

        viewModel.homeDetailsLiveData.observe(this){
            if (it != null){
                binding.home = it
                adapter.imgList = it.images
                utilAdapter.itemList = it.utilities
                utilAdapter.itemList = it.utilities
                if(it.utilities != null){
                    if(it.utilities!!.size > 4){
                        binding.showAllUtil.visible()
                    }
                }
                rulesAdapter.itemList = it.rules
            }
            AppEvent.hideLoading()
        }
    }

    override fun setEvent() {
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