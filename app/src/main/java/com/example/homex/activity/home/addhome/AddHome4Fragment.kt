package com.example.homex.activity.home.addhome

import android.net.Uri
import android.os.Handler
import android.os.Looper
import androidx.fragment.app.activityViewModels
import androidx.fragment.app.viewModels
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.R
import com.example.homex.adapter.AddImageAdapter
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentAddHome4Binding
import com.example.homex.extension.disable
import com.example.homex.extension.enable
import com.example.homex.extension.gone
import com.example.homex.extension.visible
import com.homex.core.model.ImageBase


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
            { realPos, pos->
                imgList.removeAt(realPos)
                adapter.notifyItemRemoved(pos)
                val list = mutableListOf<Pair<Uri, Boolean>>()
                list.addAll(imgList)
                fileViewModel.file.postValue(list)
            }, mutableListOf(),
            { it, pos->
                if (!idRemove.contains(it.id)){
                    it.id?.let { it1 -> idRemove.add(it1) }
                }
                fileList.remove(it)
                adapter.notifyItemRemoved(pos)
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
                imgList.addAll(it)
                adapter.notifyDataSetChanged()
                if (imgList.isNotEmpty()){
                    binding.homeImageRecView.visible()
                    binding.finishBtn.enable()
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
                            binding.btnAddImage.text = getString(R.string.upload_image_2, (imgList.size + fileList.size))
                        }
                    }
                }
            }
        }

        viewModel.images.observe(viewLifecycleOwner){
            if (it != null){
                fileList.clear()
                fileList.addAll(it)
                adapter.notifyDataSetChanged()
                if(it.isNotEmpty()){
                    binding.homeImageRecView.visible()
                    binding.finishBtn.enable()
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
                            binding.btnAddImage.text = getString(R.string.upload_image_2, (imgList.size + fileList.size))
                        }
                    }
                }
            }
        }
    }


}