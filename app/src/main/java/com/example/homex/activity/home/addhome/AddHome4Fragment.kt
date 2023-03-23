package com.example.homex.activity.home.addhome

import android.content.ContentResolver
import android.graphics.Bitmap
import android.graphics.BitmapFactory
import android.graphics.ImageDecoder
import android.net.Uri
import android.os.Build
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.provider.MediaStore
import android.util.Log
import android.view.View
import androidx.fragment.app.activityViewModels
import androidx.fragment.app.viewModels
import androidx.lifecycle.Lifecycle
import androidx.lifecycle.LifecycleEventObserver
import androidx.navigation.fragment.findNavController
import androidx.navigation.navGraphViewModels
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.R
import com.example.homex.adapter.AddImageAdapter
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentAddHome4Binding
import com.example.homex.extension.disable
import com.example.homex.extension.enable
import com.example.homex.extension.gone
import com.example.homex.extension.visible
import com.example.homex.utils.RealPathUtil
import com.homex.core.model.ImageBase
import okhttp3.MediaType
import okhttp3.MediaType.Companion.toMediaType
import okhttp3.MultipartBody
import okhttp3.RequestBody.Companion.asRequestBody
import java.io.File
import java.io.FileOutputStream


class AddHome4Fragment : BaseFragment<FragmentAddHome4Binding>() {
    override val layoutId: Int = R.layout.fragment_add_home4
    private lateinit var adapter: AddImageAdapter
    private var imgList = arrayListOf<Pair<Uri,Boolean>>()
    private var fileList = arrayListOf<ImageBase>()
    private var idRemove = arrayListOf<Int>()
    private val viewModel: AddHomeViewModel by viewModels({requireParentFragment()})
    private val fileViewModel: FileViewModel by activityViewModels()

    override fun setView() {
        adapter = AddImageAdapter(
            mutableListOf(),
            {
                imgList.removeAt(it)
                val list = mutableListOf<Pair<Uri, Boolean>>()
                list.addAll(imgList)
                fileViewModel.file.postValue(list)
            }, mutableListOf(),
            {
                if (!idRemove.contains(it.id)){
                    it.id?.let { it1 -> idRemove.add(it1) }
                }
                fileList.remove(it)
                viewModel.idRemove.postValue(idRemove)
                val list = mutableListOf<ImageBase>()
                list.addAll(fileList)
                viewModel.images.postValue(list)
            })
        adapter.imgList = imgList
        adapter.images = fileList
        binding.homeImageRecView.adapter = adapter
        binding.homeImageRecView.layoutManager = LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.finishBtn.disable()
    }

    override fun setEvent() {
        binding.btnAddImage.setOnClickListener {
            (parentFragment as AddHomeFragment).openBottomSheet()
        }
        binding.finishBtn.setOnClickListener {
            (parentFragment as AddHomeFragment).createHome()
        }
    }

    override fun setViewModel() {
        fileViewModel.file.observe(viewLifecycleOwner){
            if (it != null) {
                imgList.clear()
                Log.e("viewModelList", "$it")
                if (it.isNotEmpty()){
                    binding.homeImageRecView.visible()
                    binding.finishBtn.enable()
                    imgList.addAll(it)
                }
                Handler(Looper.getMainLooper()).post {
                    when(imgList.size + fileList.size){
                        5->{
                            binding.btnAddImage.gone()
                        }
                        0->{
                            binding.homeImageRecView.gone()
                            binding.finishBtn.disable()
                            binding.btnAddImage.text = getString(R.string.upload_image)
                        }
                        else->{
                            binding.btnAddImage.visible()
                            binding.btnAddImage.text = "Thêm Hình Ảnh ${imgList.size + fileList.size}/5"
                        }
                    }
                }
                adapter.notifyDataSetChanged()
            }
        }

        viewModel.images.observe(viewLifecycleOwner){
            if (it != null){
                fileList.clear()
                if(it.isNotEmpty()){
                    binding.homeImageRecView.visible()
                    binding.finishBtn.enable()
                    fileList.addAll(it)
                }
                Log.e("viewModelList", "$fileList")
                Handler(Looper.getMainLooper()).post {
                    when(imgList.size + fileList.size){
                        5->{
                            binding.btnAddImage.gone()
                        }
                        0->{
                            binding.homeImageRecView.gone()
                            binding.finishBtn.disable()
                            binding.btnAddImage.text = getString(R.string.upload_image)
                        }
                        else->{
                            binding.btnAddImage.visible()
                            binding.btnAddImage.text = "Thêm Hình Ảnh ${imgList.size + fileList.size}/5"
                        }
                    }
                }
                adapter.notifyDataSetChanged()
            }
        }
    }


}