package com.example.homex.activity.home.addhome

import android.Manifest
import android.location.LocationManager
import android.os.Bundle
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
import com.example.homex.viewmodel.YourHomeViewModel
import com.homex.core.model.BingLocation
import com.microsoft.maps.*
import org.koin.androidx.viewmodel.ext.android.viewModel
import pub.devrel.easypermissions.EasyPermissions
import pub.devrel.easypermissions.PermissionRequest
import java.util.ArrayList
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

        mapView.addOnMapHoldingListener {
            val pushpin = MapIcon()
            pushpin.location = it.location
            mPinLayer.elements.add(pushpin)
            mapView.setScene(
                MapScene.createFromLocationAndZoomLevel(it.location, 12.0),
                MapAnimationKind.LINEAR
            )
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
        }
    }

    override fun setEvent() {
        binding.homeAddressEdtTxt.addTextChangedListener {
            viewModel.location.postValue(it?.toString()?:"")
        }

        binding.cityTV.setOnItemClickListener { parent, view, position, id ->
            val item = cityList[position]
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
        }

        binding.districtTV.setOnItemClickListener { parent, view, position, id ->
            val item = districtList[position]
            binding.districtTV.setText(item.name, false)
            binding.wardTV.text.clear()
            wardList.clear()
            wardAdapter.notifyDataSetChanged()
            item.id?.let {
                homeViewModel.getWard(id = it)
            }
        }

        binding.wardTV.setOnItemClickListener { paren, view, position, id ->
            val item = wardList[position]
            binding.wardTV.setText(item.name, false)
        }

    }

    override fun setViewModel() {
        homeViewModel.cityLiveData.observe(this){
            if (it != null){
                cityList.clear()
                cityList.addAll(it)
                cityAdapter.notifyDataSetChanged()
            }
        }

        homeViewModel.districtLiveData.observe(this){
            if (it != null){
                districtList.clear()
                districtList.addAll(it)
                districtAdapter.notifyDataSetChanged()
            }
        }

        homeViewModel.wardLiveData.observe(this){
            if (it != null){
                wardList.clear()
                wardList.addAll(it)
                wardAdapter.notifyDataSetChanged()
            }
        }
    }

    override fun onStart() {
        super.onStart()
        mapView.onStart()
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