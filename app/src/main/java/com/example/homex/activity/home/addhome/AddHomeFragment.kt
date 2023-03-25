package com.example.homex.activity.home.addhome

import android.content.ContentResolver
import android.graphics.Bitmap
import android.graphics.BitmapFactory
import android.graphics.ImageDecoder
import android.net.Uri
import android.os.*
import android.provider.MediaStore
import android.util.Log
import android.view.View
import android.widget.Toast
import androidx.fragment.app.activityViewModels
import androidx.fragment.app.viewModels
import androidx.lifecycle.Lifecycle
import androidx.lifecycle.LifecycleEventObserver
import androidx.navigation.fragment.findNavController
import androidx.navigation.navGraphViewModels
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
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentAddHomeBinding
import com.example.homex.extension.disable
import com.example.homex.extension.enable
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
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel
import java.io.File
import java.io.FileOutputStream
import java.text.SimpleDateFormat
import java.util.*


class AddHomeFragment : BaseFragment<FragmentAddHomeBinding>() {
    override val layoutId: Int = R.layout.fragment_add_home
    private val viewModel: AddHomeViewModel by viewModels()
    private val homeViewModel: YourHomeViewModel by viewModel()
    private val fileViewModel: FileViewModel by activityViewModels()
    private val tmpFiles = mutableListOf<File>()
    private val tmp = mutableListOf<File>()

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
        Log.e("viewCreated", "Hello")
    }

    fun openBottomSheet(){
        findNavController().navigate(R.id.action_addHomeFragment_to_bottomSheetDialogSelectImage)
    }

    override fun setEvent() {
        val adapter = AddHomeViewPager(
            this,
            listOf(
                AddHome1Fragment(),
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
                    3->{
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

        binding.btnNextSlide.setOnClickListener {
            binding.addHomeViewPager.currentItem = binding.addHomeViewPager.currentItem + 1
        }

        binding.btnPreviousSlide.setOnClickListener {
            binding.addHomeViewPager.currentItem = binding.addHomeViewPager.currentItem - 1
        }

        binding.nextSlideBtn.setOnClickListener {
            Log.e("pos", binding.addHomeViewPager.currentItem.toString())
            Log.e("checkUI", checkInput(binding.addHomeViewPager.currentItem).toString())

            if(!checkInput(binding.addHomeViewPager.currentItem)){
                AppEvent.showPopUpError("Hãy điền các thông tin trước khi qua màn hình mơi")
                return@setOnClickListener
            }
            binding.addHomeViewPager.currentItem = binding.addHomeViewPager.currentItem + 1
        }

        binding.prevSlideBtn.setOnClickListener {
            binding.addHomeViewPager.currentItem = binding.addHomeViewPager.currentItem - 1
        }
    }

    override fun setViewModel() {
        viewModel.idRemove.observe(viewLifecycleOwner){
            Log.e("idRemove", "$it")
        }
        viewModel.rules.observe(viewLifecycleOwner){
            Log.e("rules", "$it")
        }
        viewModel.utilities.observe(viewLifecycleOwner){
            Log.e("utilities", "$it")
        }
        viewModel.files.observe(viewLifecycleOwner){
            Log.e("files", "$it")
        }
        viewModel.images.observe(viewLifecycleOwner){
            Log.e("images", "$it")
        }

        homeViewModel.messageLiveData.observe(viewLifecycleOwner){
            if(it != null){
                findNavController().popBackStack()
                Toast.makeText(requireContext(), "Tạo nhà thành công", Toast.LENGTH_LONG).show()
            }
            AppEvent.hideLoading()
        }

        fileViewModel.file.observe(viewLifecycleOwner){ list ->
            Log.e("images", "$list")
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
                                        Log.e("tmp", "$tmp")
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
                                Log.e("tmp", "$tmp")
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
                if (viewModel.name.value == ""
                    || viewModel.square.value == 0
                    || viewModel.price.value == 0
                    || viewModel.description.value == ""
                    || viewModel.location.value == ""
                )
                    return false
            }
            2->{
                if (viewModel.bedroom.value == 0
                    || viewModel.bathroom.value == 0
                    || viewModel.people.value == 0
                )
                    return false
            }
            3->{
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
                    || viewModel.location.value == ""
                )
                    return false
                if (viewModel.bedroom.value == 0
                    || viewModel.bathroom.value == 0
                    || viewModel.people.value == 0
                )
                    return false
                if (viewModel.files.value?.size == 0)
                    return false
            }
        }
        return true
    }

    fun createHome(){
        AppEvent.showLoading()
        if(!checkInput(-1))
            return
        val builder : MultipartBody.Builder = MultipartBody.Builder().setType(MultipartBody.FORM)
        builder.addFormDataPart("name", viewModel.name.value.toString())
        builder.addFormDataPart("option", viewModel.option.value.toString())
        builder.addFormDataPart("description", viewModel.description.value.toString())
        builder.addFormDataPart("people", viewModel.people.value.toString())
        builder.addFormDataPart("bedroom", viewModel.bedroom.value.toString())
        builder.addFormDataPart("bathroom", viewModel.bathroom.value.toString())
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
//                builder.addFormDataPart("files", file.name, requestBody)
//                Log.e("fileName", file.name)
            }
        }


        if(viewModel.id.value != 0){
            Log.e("id", "${viewModel.id}")
            Log.e("status", "${viewModel.status}")
            Log.e("images", "${viewModel.images}")
            Log.e("idRemove", "${viewModel.idRemove}")
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
        Log.e("destroy", "$tmpFiles")
        fileViewModel.file.postValue(mutableListOf())
        for(item in tmpFiles){
            Log.e("item", item.path)
            item.delete()
        }
        fileViewModel.tmpFiles.postValue(mutableListOf())
        super.onDestroy()
        Log.e("destroy", "$tmpFiles")
    }

    private fun createTempFile(uri: Uri?) : File?{
        val file = File(uri?.path ?: "")
        val bitmap: Bitmap?
        val contentResolver: ContentResolver = requireContext().contentResolver
        try {
            bitmap = if (Build.VERSION.SDK_INT < 28) {
                MediaStore.Images.Media.getBitmap(contentResolver, uri)
            } else {
                uri?.let{
                    val source = ImageDecoder.createSource(contentResolver, it)
                    ImageDecoder.decodeBitmap(source)
                }
                null
            }
            Log.e("file", "${file.exists()}")
            val outputStream = FileOutputStream(file)
            bitmap?.compress(Bitmap.CompressFormat.JPEG, 100, outputStream)
            outputStream.flush()
            outputStream.close()
            return file
        } catch (e: Exception) {
            e.printStackTrace()
        }
        return null
    }

    private fun convertUriToBitmap(uri: Uri?): Bitmap?{
        val contentResolver: ContentResolver = requireContext().contentResolver
        try {
            return if (Build.VERSION.SDK_INT < Build.VERSION_CODES.P) {
                MediaStore.Images.Media.getBitmap(contentResolver, uri)
            } else {
                uri?.let{
                    val source = ImageDecoder.createSource(contentResolver, it)
                    ImageDecoder.decodeBitmap(source)
                }
                null
            }
        } catch (e: Exception) {
            e.printStackTrace()
        }
        return null
    }

    override fun setView() {
        arguments?.getParcelable<Home>(HOME)?.let {
            (activity as HomeActivity).setPropertiesScreen(
                showLogo = false,
                showBottomNav = false,
                showMessage = false,
                showMenu = false,
                showTitleApp = Pair(true, "Sửa thông tin nhà"),
                showBoxChatLayout = Pair(false, null),
            )
            Log.e("setView", "Hello")

            viewModel.option.postValue(it.option)
            viewModel.name.postValue(it.name)
            viewModel.square.postValue(it.square)
            viewModel.location.postValue(it.location)
            viewModel.description.postValue(it.description)
            viewModel.price.postValue(it.price)
            viewModel.bedroom.postValue(it.bedRoom)
            viewModel.bathroom.postValue(it.bathRoom)
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
        }
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
        Log.e("filePath", file.path)
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