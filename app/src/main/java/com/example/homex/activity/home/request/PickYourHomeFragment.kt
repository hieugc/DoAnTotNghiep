package com.example.homex.activity.home.request

import android.os.Bundle
import android.util.Log
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
import com.homex.core.util.AppEvent
import org.koin.androidx.viewmodel.ext.android.viewModel

class PickYourHomeFragment : BaseFragment<FragmentPickYourHomeBinding>() {
    override val layoutId: Int = R.layout.fragment_pick_your_home
    private lateinit var adapter: SearchHomeAdapter
    private val yourHomeViewModel: YourHomeViewModel by viewModel()
    private var page = 0
    private var myHomeList = arrayListOf<Home>()

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
        yourHomeViewModel.getMyHomes(page)
        AppEvent.showLoading()
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
                val homes = it.homes
                Log.e("homes", "$homes")
                adapter.searchList?.clear()
                if (homes != null){
                    if(homes.size > 0){
                        adapter.searchList?.addAll(homes)
                        adapter.notifyDataSetChanged()
                    }
                }
                if(adapter.searchList.isNullOrEmpty()){
                    binding.pickYourHomeRecView.gone()
                    binding.noHomeTxt.visible()
                    binding.addHomeBtn.visible()
                    binding.appCompatTextView28.gone()
                }else{
                    binding.pickYourHomeRecView.visible()
                    binding.noHomeTxt.gone()
                    binding.addHomeBtn.gone()
                    binding.appCompatTextView28.visible()
                }
            }
            AppEvent.hideLoading()
        }
    }
}