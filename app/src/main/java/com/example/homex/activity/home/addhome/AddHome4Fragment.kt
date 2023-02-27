package com.example.homex.activity.home.addhome

import android.net.Uri
import android.os.Bundle
import android.util.Log
import android.view.View
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.R
import com.example.homex.adapter.AddImageAdapter
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentAddHome4Binding
import com.example.homex.extension.disable
import com.example.homex.extension.enable
import com.example.homex.extension.gone
import com.example.homex.extension.visible


class AddHome4Fragment : BaseFragment<FragmentAddHome4Binding>() {
    override val layoutId: Int = R.layout.fragment_add_home4
    private lateinit var adapter: AddImageAdapter
    private var imgList = arrayListOf<Uri>()

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        findNavController().currentBackStackEntry?.savedStateHandle?.getLiveData<Uri>("IMG")?.observe(viewLifecycleOwner){
            Log.e("backstackEntry", "$it")
            updateUI(it)
        }
        findNavController().currentBackStackEntry?.savedStateHandle?.getLiveData<Uri?>("Gallery")?.observe(viewLifecycleOwner){
            Log.e("backstackEntryGallery", "$it")
            if (it != null) {
                updateUI(it)
            }
        }
    }

    private fun updateUI(uri: Uri) {
        binding.homeImageRecView.visible()
        binding.finishBtn.enable()
        imgList.add(uri)
        adapter.notifyItemInserted(imgList.size - 1)
        when(imgList.size){
            5->{
                binding.btnAddImage.gone()
            }
            else->{
                binding.btnAddImage.visible()
                binding.btnAddImage.text = "Thêm Hình Ảnh ${imgList.size}/5"
            }
        }
    }

    override fun setView() {
        adapter = AddImageAdapter(
            imgList
        ){
            imgList.removeAt(it)
            adapter.notifyItemRemoved(it)
            when(imgList.size){
                0->{
                    binding.homeImageRecView.gone()
                    binding.finishBtn.disable()
                    binding.btnAddImage.text = getString(R.string.upload_image)
                }
                else->{
                    binding.btnAddImage.text = "Thêm Hình Ảnh ${imgList.size}/5"
                }
            }
        }
        binding.homeImageRecView.adapter = adapter
        binding.homeImageRecView.layoutManager = LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.finishBtn.disable()
    }

    override fun setEvent() {
        binding.btnAddImage.setOnClickListener {
            (parentFragment as AddHomeFragment).openBottomSheet()
        }
    }
}