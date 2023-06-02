package com.example.homex.activity.home.request

import android.os.Bundle
import android.view.View
import androidx.core.view.isGone
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.SearchHomeAdapter
import com.example.homex.app.USER_ACCESS
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentPickHomeBinding
import com.example.homex.extension.gone
import com.example.homex.extension.visible
import com.example.homex.viewmodel.YourHomeViewModel
import com.homex.core.model.Home
import com.homex.core.util.AppEvent
import org.koin.androidx.viewmodel.ext.android.viewModel


class PickHomeFragment : BaseFragment<FragmentPickHomeBinding>() {
    override val layoutId: Int = R.layout.fragment_pick_home
    private lateinit var adapter: SearchHomeAdapter
    private val yourHomeViewModel: YourHomeViewModel by viewModel()
    private val homeList = arrayListOf<Home>()
    private var isShimmer = true

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showBottomNav = false,
            showMessage = false,
            showMenu = false,
            showTitleApp = Pair(true, "Chọn nhà"),
            showBoxChatLayout = Pair(false, null),
        )
        binding.homeShimmer.gone()
        if (isShimmer){
            binding.homeShimmer.startShimmer()
            binding.homeShimmer.visible()
            binding.pickHomeRecView.visibility = View.INVISIBLE
        }
        arguments?.getString(USER_ACCESS)?.let {
            yourHomeViewModel.getHomeByUser(it)
        }
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
                binding.pickHomeRecView.visibility = View.INVISIBLE
                arguments?.getString(USER_ACCESS)?.let {
                    yourHomeViewModel.getHomeByUser(it)
                }
                binding.swipeRefreshLayout.isRefreshing = false
            } else {
                binding.swipeRefreshLayout.isRefreshing = false
            }
        }
    }

    override fun setView() {
        adapter = SearchHomeAdapter(
            arrayListOf()
        ){
            findNavController().previousBackStackEntry?.savedStateHandle?.set("TARGET_HOUSE", it)
            findNavController().popBackStack()
        }
        adapter.searchList = homeList
        binding.pickHomeRecView.adapter = adapter
        val layoutManager = LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.pickHomeRecView.layoutManager = layoutManager
    }

    override fun setViewModel() {
        yourHomeViewModel.listHomeLiveData.observe(this){
            if (it != null){
                homeList.clear()
                homeList.addAll(it)
                adapter.notifyDataSetChanged()
                if(homeList.isEmpty()){
                    binding.homeShimmer.stopShimmer()
                    binding.homeShimmer.gone()
                    isShimmer = false
                    binding.appCompatTextView28.visible()
                }else{
                    if (isShimmer)
                    {
                        binding.homeShimmer.stopShimmer()
                        binding.homeShimmer.gone()
                        isShimmer = false
                    }
                    binding.pickHomeRecView.visible()
                    binding.appCompatTextView28.gone()
                }
            }else{
                binding.homeShimmer.stopShimmer()
                binding.homeShimmer.gone()
                isShimmer = false
                binding.pickHomeRecView.gone()
                binding.appCompatTextView28.visible()
            }
            if (binding.homeShimmer.isGone)
                AppEvent.closePopup()
        }
    }
}