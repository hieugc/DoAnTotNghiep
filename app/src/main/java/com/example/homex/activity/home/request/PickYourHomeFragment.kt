package com.example.homex.activity.home.request

import android.os.Bundle
import android.view.View
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.SearchHomeAdapter
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentPickYourHomeBinding
import com.example.homex.extension.gone
import com.example.homex.extension.visible
import com.example.homex.viewmodel.YourHomeViewModel
import com.homex.core.model.Home
import org.koin.androidx.viewmodel.ext.android.viewModel

class PickYourHomeFragment : BaseFragment<FragmentPickYourHomeBinding>() {
    override val layoutId: Int = R.layout.fragment_pick_your_home
    private lateinit var adapter: SearchHomeAdapter
    private val yourHomeViewModel: YourHomeViewModel by viewModel()
    private var page = 0
    private var myHomeList = arrayListOf<Home>()
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
            binding.pickYourHomeRecView.visibility = View.INVISIBLE
        }
        yourHomeViewModel.getMyHomes(page)
    }

    override fun setEvent() {
        binding.addHomeBtn.setOnClickListener {
            findNavController().navigate(R.id.action_pickYourHomeFragment_to_myHomeFragment)
        }
    }

    override fun setView() {
        adapter = SearchHomeAdapter(
            myHomeList
        ){
            findNavController().previousBackStackEntry?.savedStateHandle?.set("SWAP_HOUSE", it)
            findNavController().popBackStack()
        }
        binding.pickYourHomeRecView.adapter = adapter
        val layoutManager = LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.pickYourHomeRecView.layoutManager = layoutManager
    }

    override fun setViewModel() {
        yourHomeViewModel.myHomesLiveData.observe(this){
            if (it != null){
                val homes = it.houses
                myHomeList.clear()
                if (homes != null){
                    myHomeList.addAll(homes)
                    adapter.notifyDataSetChanged()
                    if (myHomeList.isEmpty()){
                        binding.homeShimmer.stopShimmer()
                        binding.homeShimmer.gone()
                        isShimmer = false
                        binding.appCompatTextView28.gone()
                        binding.noHomeTxt.visible()
                        binding.addHomeBtn.visible()
                    }else{
                        if (isShimmer){
                            binding.homeShimmer.stopShimmer()
                            binding.homeShimmer.gone()
                            isShimmer = false
                        }
                        binding.pickYourHomeRecView.visible()
                        binding.appCompatTextView28.visible()
                        binding.noHomeTxt.gone()
                        binding.addHomeBtn.gone()
                    }
                }else{
                    binding.homeShimmer.stopShimmer()
                    binding.homeShimmer.gone()
                    isShimmer = false
                    binding.pickYourHomeRecView.gone()
                    binding.appCompatTextView28.gone()
                    binding.noHomeTxt.visible()
                    binding.addHomeBtn.visible()
                }
            }else{
                binding.homeShimmer.stopShimmer()
                binding.homeShimmer.gone()
                isShimmer = false
                binding.pickYourHomeRecView.gone()
                binding.addHomeBtn.visible()
                binding.noHomeTxt.visible()
                binding.appCompatTextView28.gone()
            }
        }
    }
}