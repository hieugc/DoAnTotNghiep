package com.example.homex.activity.home.myhome

import android.os.Bundle
import android.view.View
import androidx.core.view.isGone
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.MyHomeAdapter
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentMyHomeBinding
import com.example.homex.extension.gone
import com.example.homex.extension.visible
import com.example.homex.viewmodel.YourHomeViewModel
import com.homex.core.model.Home
import com.homex.core.util.AppEvent
import org.koin.androidx.viewmodel.ext.android.viewModel


class MyHomeFragment : BaseFragment<FragmentMyHomeBinding>() {
    override val layoutId: Int = R.layout.fragment_my_home
    private lateinit var adapter: MyHomeAdapter
    private val viewModel: YourHomeViewModel by viewModel()
    private val homeList = arrayListOf<Home>()
    private var page = 0
    private var isShimmer = true

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showBottomNav = false,
            showTitleApp = Pair(true, "Nhà của bạn"),
            showMenu = false,
            showMessage = false,
            showBoxChatLayout = Pair(false, null),
        )
        binding.homeShimmer.gone()
        if (isShimmer){
            binding.homeShimmer.startShimmer()
            binding.homeShimmer.visible()
            binding.mainHomeRecView.visibility = View.INVISIBLE
        }
        viewModel.getMyHomes(page)
        initSwipeLayout()
    }

    private fun initSwipeLayout(){
        binding.swipeRefreshLayout.setOnRefreshListener {
            if (!isShimmer){
                AppEvent.showPopUp()
                isShimmer = true
                binding.homeShimmer.startShimmer()
                binding.homeShimmer.visible()
                homeList.clear()
                adapter.notifyDataSetChanged()
                binding.mainHomeRecView.visibility = View.INVISIBLE
                page = 0
                viewModel.getMyHomes(page)
                binding.swipeRefreshLayout.isRefreshing = false
            } else {
                binding.swipeRefreshLayout.isRefreshing = false
            }
        }
    }

    override fun setView() {
        adapter = MyHomeAdapter(
            homeList,
            onClick = {
                val action = MyHomeFragmentDirections.actionGlobalMyHomeDetailFragment(id = it)
                findNavController().navigate(action)
            }
        )
        binding.mainHomeRecView.adapter = adapter
        val layoutManager = LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.mainHomeRecView.layoutManager = layoutManager
    }

    override fun setEvent() {
        binding.addHomeFAB.setOnClickListener {
            findNavController().navigate(R.id.action_myHomeFragment_to_addHomeFragment)
        }
    }

    override fun setViewModel() {
        viewModel.myHomesLiveData.observe(viewLifecycleOwner){
            if (it != null){
                homeList.clear()
                val homes = it.houses
                if (homes != null){
                    homeList.addAll(homes)
                    adapter.notifyDataSetChanged()
                    if (homeList.isEmpty()){
                        binding.homeShimmer.stopShimmer()
                        binding.homeShimmer.gone()
                        binding.noHomeTxt.visible()
                        isShimmer = false
                    }else{
                        if (isShimmer){
                            binding.homeShimmer.stopShimmer()
                            binding.homeShimmer.gone()
                            isShimmer = false
                        }
                        binding.mainHomeRecView.visible()
                        binding.noHomeTxt.gone()
                    }
                }else{
                    binding.homeShimmer.stopShimmer()
                    binding.homeShimmer.gone()
                    isShimmer = false
                    binding.mainHomeRecView.gone()
                    binding.noHomeTxt.visible()
                }
            }else{
                binding.homeShimmer.stopShimmer()
                binding.homeShimmer.gone()
                isShimmer = false
                binding.mainHomeRecView.gone()
                binding.noHomeTxt.visible()
            }
            if (binding.homeShimmer.isGone)
                AppEvent.closePopup()
        }
    }
}