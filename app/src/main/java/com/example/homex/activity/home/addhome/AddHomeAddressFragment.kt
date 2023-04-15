package com.example.homex.activity.home.addhome

import android.Manifest
import android.graphics.BitmapFactory
import android.graphics.PointF
import android.location.LocationManager
import android.os.Bundle
import android.text.Editable
import android.text.TextWatcher
import android.util.Log
import android.view.View
import android.widget.ArrayAdapter
import androidx.core.widget.addTextChangedListener
import androidx.fragment.app.viewModels
import com.example.homex.BuildConfig
import com.example.homex.R
import com.example.homex.adapter.LocationAdapter
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentAddHomeAddressBinding
import com.example.homex.extension.gone
import com.example.homex.extension.visible
import com.example.homex.viewmodel.YourHomeViewModel
import com.homex.core.model.BingLocation
import com.microsoft.maps.*
import com.microsoft.maps.search.MapLocationFinder
import com.microsoft.maps.search.MapLocationFinderStatus
import com.microsoft.maps.search.MapLocationOptions
import org.koin.androidx.viewmodel.ext.android.viewModel
import pub.devrel.easypermissions.EasyPermissions
import pub.devrel.easypermissions.PermissionRequest
import java.util.*
import java.util.concurrent.TimeUnit


class AddHomeAddressFragment : BaseFragment<FragmentAddHomeAddressBinding>(), EasyPermissions.PermissionCallbacks{
    override val layoutId: Int
        get() = R.layout.fragment_add_home_address

    private val viewModel: AddHomeViewModel by viewModels({requireParentFragment()})
    private val homeViewModel: YourHomeViewModel by viewModel()
    private val cityList = arrayListOf<BingLocation>()
    private val districtList = arrayListOf<BingLocation>()
    private val wardList = arrayListOf<BingLocation>()

    private lateinit var cityAdapter: ArrayAdapter<BingLocation>
    private lateinit var districtAdapter: ArrayAdapter<BingLocation>
    private lateinit var wardAdapter: ArrayAdapter<BingLocation>

    //Map
    private lateinit var mapView: MapView
    private lateinit var mPinLayer: MapElementLayer
    private var requestingLocationPermission = false
    private lateinit var userLocationTrackingState: MapUserLocationTrackingState
    private lateinit var userLocation: MapUserLocation
    private lateinit var locationPin: Geopoint
    private lateinit var options: MapLocationOptions

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        homeViewModel.getCity()
        //Setup Map
        mapView = MapView(requireContext(), MapRenderMode.VECTOR)
        mapView.setCredentialsKey(BuildConfig.CREDENTIALS_KEY)
        binding.mapView.addView(mapView)

        //Add Layer
        mPinLayer = MapElementLayer()
        mapView.layers.add(mPinLayer)

        options = MapLocationOptions()
            .setCulture("vn")
            .setRegion("VN")
            .setIncludeQueryParse(true)
            .setMaxResults(20)

        //Custom MapStyle
        val customMapStyleString = "{\n" +
                "  \"version\": \"1.0\",\n" +
                "  \"settings\": {\n" +
                "    \"landColor\": \"#e7e6e5\",\n" +
                "    \"shadedReliefVisible\": false\n" +
                "  },\n" +
                "  \"elements\": {\n" +
                "    \"vegetation\": {\n" +
                "      \"fillColor\": \"#c5dea2\"\n" +
                "    },\n" +
                "    \"naturalPoint\": {\n" +
                "      \"visible\": false,\n" +
                "      \"labelVisible\": false\n" +
                "    },\n" +
                "    \"transportation\": {\n" +
                "      \"labelOutlineColor\": \"#ffffff\",\n" +
                "      \"fillColor\": \"#ffffff\",\n" +
                "      \"strokeColor\": \"#d7d6d5\"\n" +
                "    },\n" +
                "    \"water\": {\n" +
                "      \"fillColor\": \"#b1bdd6\",\n" +
                "      \"labelColor\": \"#ffffff\",\n" +
                "      \"labelOutlineColor\": \"#9aa9ca\"\n" +
                "    },\n" +
                "    \"structure\": {\n" +
                "      \"fillColor\": \"#d7d6d5\"\n" +
                "    },\n" +
                "    \"indigenousPeoplesReserve\": {\n" +
                "      \"visible\": false\n" +
                "    },\n" +
                "    \"military\": {\n" +
                "      \"visible\": false\n" +
                "    }\n" +
                "  }\n" +
                "}"

        val styleSheetFromJson = MapStyleSheet.fromJson(customMapStyleString)
        if (styleSheetFromJson != null) {
            mapView.mapStyleSheet = styleSheetFromJson
        }else{
            mapView.mapStyleSheet = MapStyleSheets.roadLight()
        }

        userLocation = mapView.userLocation

        val pinBitmap = BitmapFactory.decodeResource(resources, R.mipmap.loc_pin)
        val mapImage = MapImage(pinBitmap)
        locationPin = mapView.center

        val uiOptions: MapUserInterfaceOptions = mapView.userInterfaceOptions
        uiOptions.isTiltButtonVisible = false
        uiOptions.isTiltGestureEnabled = false
        uiOptions.isUserLocationButtonVisible = true
        uiOptions.isDirectionsButtonVisible = false
        uiOptions.isCompassButtonVisible = false
        uiOptions.setUserLocationButtonAlignment(
            MapToolbarHorizontalAlignment.RIGHT,
            MapToolbarVerticalAlignment.TOP
        )
        uiOptions.addOnUserLocationButtonTappedListener {
            trackingUserLocation()
            false
        }


        mapView.addOnMapCameraChangedListener {
            Log.e("camera", "${it.changeReason}")
            mPinLayer.elements.clear()
            val pushpin = MapIcon()
            pushpin.location = mapView.center
            pushpin.image = mapImage
            pushpin.normalizedAnchorPoint = PointF(0.5f, 1.0f)
            mPinLayer.elements.add(pushpin)
            locationPin = mapView.center
            Log.e("lng", "${mapView.center.position.longitude}")
            Log.e("lat", "${mapView.center.position.latitude}")
//            viewModel.lat.postValue(mapView.center.position.latitude)
//            viewModel.lng.postValue(mapView.center.position.longitude)
            Log.e("center", "${it.camera.location}")
            true
        }
        mapView.onCreate(savedInstanceState)

    }

    override fun setView() {
        cityAdapter = LocationAdapter(requireContext(), R.layout.sex_item, cityList)
        binding.cityTV.setAdapter(cityAdapter)

        districtAdapter = LocationAdapter(requireContext(), R.layout.sex_item, districtList)
        binding.districtTV.setAdapter(districtAdapter)

        wardAdapter = LocationAdapter(requireContext(), R.layout.sex_item, wardList)
        binding.wardTV.setAdapter(wardAdapter)

        viewModel.apply {
            binding.homeAddressEdtTxt.setText(this.location.value)

            if (this.idCity.value != 0){
                this.idCity.value?.let { homeViewModel.getDistrict(it) }
            }

            if (this.idDistrict.value != 0){
                this.idDistrict.value?.let { homeViewModel.getWard(it) }
            }
        }
    }

    override fun setEvent() {
        binding.homeAddressEdtTxt.addTextChangedListener {
            viewModel.location.postValue(it?.toString()?:"")
        }

        binding.mapBtn.setOnClickListener {
            viewModel.showMap.postValue(true)
        }

        binding.confirmButton.setOnClickListener {
            viewModel.lat.postValue(mapView.center.position.latitude)
            viewModel.lng.postValue(mapView.center.position.longitude)
            viewModel.showMap.postValue(false)
        }

        binding.btnBack.setOnClickListener {
            viewModel.showMap.postValue(false)
        }

        binding.cityTV.setOnItemClickListener { parent, view, position, id ->
            val item = cityList[position]
            viewModel.idCity.postValue(item.id)
            viewModel.idDistrict.postValue(0)
            viewModel.idWard.postValue(0)
            binding.cityTV.setText(item.name, false)
            binding.districtTV.text.clear()
            binding.wardTV.text.clear()
            districtList.clear()
            wardList.clear()
            districtAdapter.notifyDataSetChanged()
            wardAdapter.notifyDataSetChanged()
            item.id?.let {
                homeViewModel.getDistrict(id = it)
            }
            findLocation()
        }

        binding.districtTV.setOnItemClickListener { parent, view, position, id ->
            val item = districtList[position]
            viewModel.idDistrict.postValue(item.id)
            viewModel.idWard.postValue(0)
            binding.districtTV.setText(item.name, false)
            binding.wardTV.text.clear()
            wardList.clear()
            wardAdapter.notifyDataSetChanged()
            item.id?.let {
                homeViewModel.getWard(id = it)
            }
            findLocation()
        }

        binding.wardTV.setOnItemClickListener { paren, view, position, id ->
            val item = wardList[position]
            binding.wardTV.setText(item.name, false)
            viewModel.idWard.postValue(item.id)
            findLocation()
        }



        binding.homeAddressEdtTxt.addTextChangedListener(object : TextWatcher{
            private val delay: Long = 1000
            private var timer = Timer()
            override fun beforeTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {}

            override fun onTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {}

            override fun afterTextChanged(p0: Editable?) {
                timer.cancel()
                timer = Timer()
                timer.schedule(object : TimerTask(){
                    override fun run() {
                        findLocation()
                    }
                }, delay)
            }
        })
    }

    private fun findLocation(){
        val ward = binding.wardTV.text
        val district = binding.districtTV.text
        val city = binding.cityTV.text
        val location = binding.homeAddressEdtTxt.text
        val query = "$location, $ward, $district, Thành phố $city Việt Nam"
        Log.e("queryLocation", query)
        MapLocationFinder.findLocations(query, options){
            if (it.status == MapLocationFinderStatus.SUCCESS){
                Log.e("location", "${it.locations}")
                for ((idx, location) in it.locations.withIndex()){
                    Log.e("location", "${location.address}")
                    Log.e("location", "${location.entityType}")
                    Log.e("location", "${location.displayName}")
                    Log.e("locationAdd", "${location.address.addressLine}")
                    Log.e("locationAdd", "${location.address.locality}")
                    Log.e("locationAdd", "${location.address.formattedAddress}")
                    Log.e("locationAdd", "${location.address.landmark}")
                    Log.e("locationAdd", "${location.address.neighborhood}")
                    Log.e("locationAdd", "${location.address.adminDistrict2}")
                    Log.e("locationAdd", "${location.address.adminDistrict}")
                    if (location.entityType == "Address"){
                        mapView.setScene(MapScene.createFromLocationAndZoomLevel(location.point, 18.0), MapAnimationKind.LINEAR)
                        mPinLayer.elements.clear()
                        break
                    }
                    if (idx == it.locations.size - 1){
                        mapView.setScene(MapScene.createFromLocationAndZoomLevel(location.point, 18.0), MapAnimationKind.LINEAR)
                    }
                }
            }
        }
    }

    override fun setViewModel() {
        homeViewModel.cityLiveData.observe(this){
            if (it != null){
                cityList.clear()
                cityList.addAll(it)
                cityAdapter.notifyDataSetChanged()
                if (viewModel.idCity.value != 0){
                    for ( city in cityList){
                        if (city.id == viewModel.idCity.value){
                            binding.cityTV.setText(city.name, false)
                            break
                        }
                    }
                }
            }
        }

        homeViewModel.districtLiveData.observe(this){
            if (it != null){
                districtList.clear()
                districtList.addAll(it)
                districtAdapter.notifyDataSetChanged()
                if (viewModel.idDistrict.value != 0){
                    for ( district in districtList){
                        if (district.id == viewModel.idDistrict.value){
                            binding.districtTV.setText(district.name, false)
                            break
                        }
                    }
                }
            }
        }

        homeViewModel.wardLiveData.observe(this){
            if (it != null){
                wardList.clear()
                wardList.addAll(it)
                wardAdapter.notifyDataSetChanged()
                if (viewModel.idWard.value != 0){
                    for ( ward in wardList){
                        if (ward.id == viewModel.idWard.value){
                            binding.wardTV.setText(ward.name, false)
                            break
                        }
                    }
                }
            }
        }
        viewModel.showMap.observe(this){
            if (it == true){
                binding.mapView.visible()
                binding.btnBack.visible()
                binding.confirmButton.visible()
            }else{
                binding.mapView.gone()
                binding.btnBack.gone()
                binding.confirmButton.gone()
            }
        }
    }

    override fun onStart() {
        super.onStart()
        mapView.onStart()
        viewModel.apply {
            if (this.lat.value != 0.0 && this.lng.value != 0.0){
                mapView.setScene(
                    MapScene.createFromLocationAndZoomLevel(Geopoint(this.lat.value!!, this.lng.value!!), 18.0),
                    MapAnimationKind.LINEAR
                )
            }
        }
    }

    override fun onResume() {
        super.onResume()
        mapView.onResume()
    }

    override fun onPause() {
        super.onPause()
        mapView.onPause()
    }

    override fun onSaveInstanceState(outState: Bundle) {
        super.onSaveInstanceState(outState)
        mapView.onSaveInstanceState(outState)
    }

    override fun onStop() {
        super.onStop()
        mapView.onStop()
    }

    override fun onDestroy() {
        super.onDestroy()
        mapView.onDestroy()
    }

    override fun onLowMemory() {
        super.onLowMemory()
        mapView.onLowMemory()
    }
    private fun requestLocationPermission(){
        if (EasyPermissions.hasPermissions(requireContext(), Manifest.permission.ACCESS_FINE_LOCATION)) {
            trackingUserLocation()
        } else if (!requestingLocationPermission) {
            requestingLocationPermission = true
            EasyPermissions.requestPermissions(
                PermissionRequest.Builder(this,
                    1,
                    Manifest.permission.ACCESS_FINE_LOCATION)
                    .setRationale(R.string.location_rationale)
                    .setPositiveButtonText(R.string.ok)
                    .setNegativeButtonText(R.string.cancel)
                    .setTheme(R.style.AlertDialog)
                    .build()
            )
        }
    }

    override fun onPermissionsGranted(requestCode: Int, perms: MutableList<String>) {
        Log.e("granted", "hello")
        requestingLocationPermission = false
        trackingUserLocation()
    }

    private fun trackingUserLocation(){
        activity?.applicationContext?.let {
            userLocationTrackingState = userLocation.startTracking(
                GPSMapLocationProvider.Builder(it)
                    .setDesiredProviders(
                        ArrayList(
                            listOf(
                                LocationManager.NETWORK_PROVIDER,
                                LocationManager.GPS_PROVIDER
                            )
                        )
                    )
                    .setMinTime(TimeUnit.SECONDS.toMillis(0))
                    .build()
            )
        }

        if (userLocationTrackingState == MapUserLocationTrackingState.PERMISSION_DENIED) {
            // request for user location permissions and then call startTracking again
            requestLocationPermission()
            Log.e("denied", "hello")
        } else if (userLocationTrackingState == MapUserLocationTrackingState.READY) {
            // handle the case where location tracking was successfully started
            userLocation.trackingMode = MapUserLocationTrackingMode.CENTERED_ON_USER
            Log.e("ready", "hello")
        } else if (userLocationTrackingState == MapUserLocationTrackingState.DISABLED) {
            // handle the case where all location providers were disabled
            Log.d("boo" , "fu")
        }
    }

    override fun onPermissionsDenied(requestCode: Int, perms: MutableList<String>) {
        requestingLocationPermission = false
    }

    override fun onRequestPermissionsResult(
        requestCode: Int,
        permissions: Array<out String>,
        grantResults: IntArray
    ) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults)

        EasyPermissions.onRequestPermissionsResult(requestCode, permissions, grantResults, this)
    }

}