package com.example.homex.activity.home.addhome

import android.graphics.Bitmap
import android.os.Bundle
import android.os.Environment
import android.view.View
import androidx.fragment.app.activityViewModels
import androidx.fragment.app.viewModels
import androidx.navigation.fragment.findNavController
import androidx.viewpager2.widget.ViewPager2
import com.bumptech.glide.Glide
import com.bumptech.glide.load.DataSource
import com.bumptech.glide.load.engine.GlideException
import com.bumptech.glide.request.RequestListener
import com.bumptech.glide.request.target.Target
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.AddHomeViewPager
import com.example.homex.app.HOME
import com.example.homex.base.BaseActivity
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentAddHomeBinding
import com.example.homex.extension.gone
import com.example.homex.extension.visible
import com.example.homex.utils.RealPathUtil
import com.example.homex.viewmodel.YourHomeViewModel
import com.homex.core.model.Home
import com.homex.core.util.AppEvent
import okhttp3.MediaType
import okhttp3.MediaType.Companion.toMediaType
import okhttp3.MultipartBody
import okhttp3.RequestBody
import okhttp3.RequestBody.Companion.asRequestBody
import org.koin.androidx.viewmodel.ext.android.viewModel
import java.io.File
import java.io.FileOutputStream
import java.util.Date
import java.util.Random


class AddHomeFragment : BaseFragment<FragmentAddHomeBinding>() {
    override val layoutId: Int = R.layout.fragment_add_home
    private val viewModel: AddHomeViewModel by viewModels()
    private val homeViewModel: YourHomeViewModel by viewModel()
    private val fileViewModel: FileViewModel by activityViewModels()
    private val tmpFiles = mutableListOf<File>()
    private val tmp = mutableListOf<File>()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        fileViewModel.file.postValue(mutableListOf())
        viewModel.idCity.observe(this){
            if (it != null && it != 0){
                predictHouse()
            }
        }

        viewModel.lng.observe(this){
            if (it != null && it != 0.0){
                predictHouse()
            }
        }
        viewModel.lat.observe(this){
            if (it != null && it != 0.0){
                predictHouse()
            }
        }
        viewModel.square.observe(this){
            if (it != null && it != 0){
                predictHouse()
            }
        }
        homeViewModel.predictLiveData.observe(this){
            if (it != null)
                viewModel.predict.postValue(it)
        }
    }

    private fun predictHouse(){
        val idCity = viewModel.idCity.value?:return
        val lat = viewModel.lat.value?:return
        val lng = viewModel.lng.value?:return
        val area = viewModel.square.value?:return
        val rating = 0.0
        if (idCity != 0 && lat != 0.0 && lng != 0.0 && area != 0){
            homeViewModel.predictHouse(idCity, lat, lng, rating, area)
        }
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showBottomNav = false,
            showMessage = false,
            showMenu = false,
            showTitleApp = Pair(true, "Thêm nhà"),
            showBoxChatLayout = Pair(false, null),
        )
        super.onViewCreated(view, savedInstanceState)
        arguments?.getParcelable<Home>(HOME)?.let {
            (activity as HomeActivity).setPropertiesScreen(
                showLogo = false,
                showBottomNav = false,
                showMessage = false,
                showMenu = false,
                showTitleApp = Pair(true, "Sửa thông tin nhà"),
                showBoxChatLayout = Pair(false, null),
            )

            viewModel.option.postValue(it.option)
            viewModel.name.postValue(it.name)
            viewModel.square.postValue(it.square)
            viewModel.location.postValue(it.location)
            viewModel.description.postValue(it.description)
            viewModel.price.postValue(it.price)
            viewModel.bedroom.postValue(it.bedRoom)
            viewModel.bathroom.postValue(it.bathRoom)
            viewModel.bed.postValue(it.bed)
            viewModel.people.postValue(it.people)
            viewModel.utilities.postValue(it.utilities)
            viewModel.rules.postValue(it.rules)
            viewModel.lat.postValue(it.lat)
            viewModel.lng.postValue(it.lng)
            viewModel.id.postValue(it.id)
            viewModel.status.postValue(it.status)
            viewModel.idCity.postValue(it.idCity)
            viewModel.idDistrict.postValue(it.idDistrict)
            viewModel.idWard.postValue(it.idWard)
            viewModel.images.postValue(it.images)
            arguments?.remove(HOME)
        }
    }

    fun openBottomSheet(){
        findNavController().navigate(R.id.action_addHomeFragment_to_bottomSheetDialogSelectImage)
    }

    override fun setEvent() {
        val adapter = AddHomeViewPager(
            this,
            listOf(
                AddHome1Fragment(),
                AddHomeAddressFragment(),
                AddHome2Fragment(),
                AddHome3Fragment(),
                AddHome4Fragment()
            )
        )
        binding.addHomeViewPager.adapter = adapter
        binding.addHomeViewPager.isUserInputEnabled = false
        binding.addHomeViewPager.registerOnPageChangeCallback(object: ViewPager2.OnPageChangeCallback(){
            override fun onPageSelected(position: Int) {
                super.onPageSelected(position)
                binding.stepView.go(position, true)
                when(position){
                    0 -> {
                        binding.prevSlideBtn.gone()
                        binding.nextSlideBtn.visible()
                    }
                    4->{
                        binding.prevSlideBtn.visible()
                        binding.nextSlideBtn.gone()
                    }
                    else->{
                        binding.prevSlideBtn.visible()
                        binding.nextSlideBtn.visible()
                    }
                }
            }
        })

        binding.nextSlideBtn.setOnClickListener {
            if(!checkInput(binding.addHomeViewPager.currentItem)){
                AppEvent.showPopUpError(getString(R.string.please_fill_up_inputs))
                return@setOnClickListener
            }
            binding.addHomeViewPager.currentItem = binding.addHomeViewPager.currentItem + 1
        }

        binding.prevSlideBtn.setOnClickListener {
            binding.addHomeViewPager.currentItem = binding.addHomeViewPager.currentItem - 1
        }
    }

    override fun setViewModel() {
        viewModel.showMap.observe(viewLifecycleOwner){
            if (it == true){
                binding.nextSlideBtn.gone()
                binding.prevSlideBtn.gone()
            }else{
                binding.nextSlideBtn.visible()
                binding.prevSlideBtn.visible()
            }
        }

        homeViewModel.messageLiveData.observe(viewLifecycleOwner){
            if(it != null){
                findNavController().popBackStack()
                (activity as BaseActivity).displayMessage(getString(R.string.create_home_success))
            }
            AppEvent.closePopup()
        }

        homeViewModel.editMessageLiveData.observe(viewLifecycleOwner){
            if(it != null){
                findNavController().popBackStack()
                (activity as BaseActivity).displayMessage(getString(R.string.edit_home_success))
            }
            AppEvent.closePopup()
        }

        fileViewModel.file.observe(viewLifecycleOwner){ list ->
            if(list != null){
                if(list.size > 0){
                    tmp.clear()
                    for(item in list){
                        val uri = item.first
                        val fromCamera = item.second
                        if(fromCamera){
                            var bitmap: Bitmap?
                            Glide.with(requireContext())
                                .asBitmap()
                                .load(uri)
                                .listener(object : RequestListener<Bitmap> {
                                    override fun onLoadFailed(
                                        e: GlideException?,
                                        model: Any?,
                                        target: Target<Bitmap>?,
                                        isFirstResource: Boolean
                                    ): Boolean {
                                        bitmap = null
                                        return false
                                    }

                                    override fun onResourceReady(
                                        resource: Bitmap?,
                                        model: Any?,
                                        target: Target<Bitmap>?,
                                        dataSource: DataSource?,
                                        isFirstResource: Boolean
                                    ): Boolean {
                                        bitmap = resource
                                        val file = saveBitmapToSDCard(bitmap)
                                        tmpFiles.add(file)
                                        tmp.add(file)
                                        viewModel.files.postValue(tmp)
                                        fileViewModel.tmpFiles.postValue(tmpFiles)
                                        return false
                                    }
                                })
                                .into(binding.tempImg)
                        }else{
                            val path = RealPathUtil.getRealPath(requireContext(), uri)?:""
                            if(path != ""){
                                val file = File(path)
                                tmp.add(file)
                                viewModel.files.postValue(tmp)
                            }
                        }
                    }
                }
            }
        }
    }

    private fun checkInput(position: Int): Boolean{
        when(position){
            0->{
                if (viewModel.option.value != 1 && viewModel.option.value != 2)
                    return false
            }
            1->{
                if (viewModel.location.value == ""
                    || viewModel.lat.value == 0.0
                    || viewModel.lng.value == 0.0
                    || viewModel.idCity.value == 0
                )
                    return false
            }
            2->{
                if (viewModel.name.value == ""
                    || viewModel.square.value == 0
                    || viewModel.price.value == 0
                    || viewModel.description.value == ""
                )
                    return false
            }
            3->{
                if (viewModel.bedroom.value == 0
                    || viewModel.bathroom.value == 0
                    || viewModel.people.value == 0
                    || viewModel.bed.value == 0
                )
                    return false
            }
            4->{
                if (viewModel.files.value?.size == 0)
                    return false
            }
            -1->{
                if (viewModel.option.value != 1 && viewModel.option.value != 2)
                    return false
                if (viewModel.name.value == ""
                    || viewModel.square.value == 0
                    || viewModel.price.value == 0
                    || viewModel.description.value == ""
                )
                    return false
                if (viewModel.bedroom.value == 0
                    || viewModel.bathroom.value == 0
                    || viewModel.people.value == 0
                    || viewModel.bed.value == 0
                )
                    return false
                if (viewModel.files.value?.size == 0 && viewModel.images.value?.size == 0)
                    return false
                if (viewModel.location.value == ""
                    || viewModel.lat.value == 0.0
                    || viewModel.lng.value == 0.0
                    || viewModel.idCity.value == 0
                )
                    return false
            }
        }
        return true
    }

    fun createHome(){
        if(!checkInput(-1))
            return
        val builder : MultipartBody.Builder = MultipartBody.Builder().setType(MultipartBody.FORM)
        builder.addFormDataPart("name", viewModel.name.value.toString())
        builder.addFormDataPart("option", viewModel.option.value.toString())
        builder.addFormDataPart("description", viewModel.description.value.toString())
        builder.addFormDataPart("people", viewModel.people.value.toString())
        builder.addFormDataPart("bedroom", viewModel.bedroom.value.toString())
        builder.addFormDataPart("bathroom", viewModel.bathroom.value.toString())
        builder.addFormDataPart("bed", viewModel.bed.value.toString())
        builder.addFormDataPart("square", viewModel.square.value.toString())
        builder.addFormDataPart("location", viewModel.location.value.toString())
        builder.addFormDataPart("idCity", viewModel.idCity.value.toString())
        builder.addFormDataPart("idDistrict", viewModel.idDistrict.value.toString())
        builder.addFormDataPart("idWard", viewModel.idWard.value.toString())
        builder.addFormDataPart("lat", viewModel.lat.value.toString())
        builder.addFormDataPart("lng", viewModel.lng.value.toString())
        builder.addFormDataPart("price", viewModel.price.value.toString())

        viewModel.utilities.value?.let {
            for ((index, util) in it.withIndex()){
                builder.addFormDataPart("utilities[${index}]", util.toString())
            }
        }
        viewModel.rules.value?.let {
            for ((index, rule) in it.withIndex()){
                builder.addFormDataPart("rules[${index}]", rule.toString())
            }
        }

        viewModel.files.value?.let {
            for(file in it){
                val mediaType : MediaType = "multipart/form-data".toMediaType()
                val requestBody = file.asRequestBody(mediaType)
                val body = MultipartBody.Part.createFormData("files", file.name, requestBody)
                builder.addPart(body)
            }
        }


        if(viewModel.id.value != 0){
            builder.addFormDataPart("id", viewModel.id.value.toString())
            viewModel.idRemove.value?.let {
                for ((index, id) in it.withIndex()){
                    builder.addFormDataPart("idRemove[${index}]", id.toString())
                }
            }
            val body: RequestBody = builder.build()
            homeViewModel.editHome(body)

        }else{
            val body: RequestBody = builder.build()
            homeViewModel.createHome(body)
        }
    }

    override fun onDestroy() {
        fileViewModel.file.postValue(mutableListOf())
        for(item in tmpFiles){
            item.delete()
        }
        fileViewModel.tmpFiles.postValue(mutableListOf())
        super.onDestroy()
    }


    private fun saveBitmapToSDCard(bitmap: Bitmap?): File{
        val dir = context?.getExternalFilesDir(Environment.DIRECTORY_PICTURES)
        val date = Date()
        if(dir?.exists() == false){
            dir.mkdirs()
        }
        val random = Random()
        val n = random.nextInt(10000)
        val file = File(dir, "IMG_${date.time}_${n}.jpg")
        if(file.exists()){
            file.delete()
        }
        val fos = FileOutputStream(file)
        bitmap?.compress(Bitmap.CompressFormat.JPEG, 100, fos)
        fos.flush()
        fos.close()
        return file
    }
}