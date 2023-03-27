package com.example.homex.activity.home.request

import android.os.Bundle
import android.util.Log
import android.view.View
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
import org.koin.androidx.viewmodel.ext.android.viewModel


class PickHomeFragment : BaseFragment<FragmentPickHomeBinding>() {
    override val layoutId: Int = R.layout.fragment_pick_home
    private lateinit var adapter: SearchHomeAdapter
    private val yourHomeViewModel: YourHomeViewModel by viewModel()
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
        }
        arguments?.getString(USER_ACCESS)?.let {
            yourHomeViewModel.getHomeByUser(it)
        }
    }

    override fun setView() {
        adapter = SearchHomeAdapter(
            arrayListOf()
        ){
            findNavController().previousBackStackEntry?.savedStateHandle?.set("TARGET_HOUSE", it)
            findNavController().popBackStack()
        }
        binding.pickHomeRecView.adapter = adapter
        val layoutManager = LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.pickHomeRecView.layoutManager = layoutManager
    }

    override fun setViewModel() {
        yourHomeViewModel.listHomeLiveData.observe(this){
            if (it != null){
                Log.e("homes", "$it")
                val size = adapter.searchList?.size
                if (size != null){
                    if (size > 0){
                        adapter.searchList?.clear()
                        adapter.notifyItemRangeRemoved(0, size)
                    }
                }
                if(it.size > 0){
                    adapter.searchList?.addAll(it)
                    adapter.notifyItemRangeInserted(0, it.size)
                    binding.homeShimmer.stopShimmer()
                    binding.homeShimmer.gone()
                    isShimmer = false
                }else{
                    binding.homeShimmer.stopShimmer()
                    binding.homeShimmer.gone()
                    isShimmer = false
                }
                if(adapter.searchList.isNullOrEmpty()){
                    binding.pickHomeRecView.gone()
                    binding.noHomeTxt.visible()
                    binding.appCompatTextView28.gone()
                }else{
                    binding.pickHomeRecView.visible()
                    binding.noHomeTxt.gone()
                    binding.appCompatTextView28.visible()
                }
            }else{
                binding.homeShimmer.stopShimmer()
                binding.homeShimmer.gone()
                isShimmer = false
            }
        }
    }
}