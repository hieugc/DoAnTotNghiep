package com.example.homex.activity.home

import android.Manifest
import android.graphics.BitmapFactory
import android.graphics.PointF
import android.location.LocationManager
import android.os.Bundle
import android.view.View
import android.view.ViewGroup
import com.example.homex.BuildConfig
import com.example.homex.R
import com.example.homex.app.HOME
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentMapBinding
import com.homex.core.model.Home
import com.microsoft.maps.GPSMapLocationProvider
import com.microsoft.maps.Geopoint
import com.microsoft.maps.MapAnimationKind
import com.microsoft.maps.MapElementLayer
import com.microsoft.maps.MapFlyout
import com.microsoft.maps.MapIcon
import com.microsoft.maps.MapImage
import com.microsoft.maps.MapRenderMode
import com.microsoft.maps.MapScene
import com.microsoft.maps.MapStyleSheet
import com.microsoft.maps.MapStyleSheets
import com.microsoft.maps.MapToolbarHorizontalAlignment
import com.microsoft.maps.MapToolbarVerticalAlignment
import com.microsoft.maps.MapUserInterfaceOptions
import com.microsoft.maps.MapUserLocation
import com.microsoft.maps.MapUserLocationTrackingMode
import com.microsoft.maps.MapUserLocationTrackingState
import com.microsoft.maps.MapView
import pub.devrel.easypermissions.EasyPermissions
import pub.devrel.easypermissions.PermissionRequest
import java.util.concurrent.TimeUnit


class MapFragment : BaseFragment<FragmentMapBinding>(), EasyPermissions.PermissionCallbacks {
    override val layoutId: Int
        get() = R.layout.fragment_map

    private lateinit var mapView: MapView
    private lateinit var mPinLayer: MapElementLayer
    private var requestingLocationPermission = false
    private lateinit var userLocationTrackingState: MapUserLocationTrackingState
    private lateinit var userLocation: MapUserLocation
    private lateinit var mapImage: MapImage


    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        //Setup Map
        mapView = MapView(requireContext(), MapRenderMode.VECTOR)
        mapView.setCredentialsKey(BuildConfig.CREDENTIALS_KEY)

        mapImage = MapImage(
            BitmapFactory.decodeResource(resources, R.mipmap.location_pin)
        )

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

        mapView.onCreate(savedInstanceState)
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showMessage = false,
            showBoxChatLayout = Pair(false, null  ),
            showSearchLayout = false,
            showMenu = false,
            showTitleApp = Pair(true, "Bản đổ"),
            showBottomNav = false,
            showLogo = false
        )
        if (mapView.parent != null)
            (mapView.parent as ViewGroup).removeView(mapView)
        binding.mapView.addView(mapView)
        val home = arguments?.getParcelable<Home>(HOME)
        if (home != null){
            val pushpin =  MapIcon()
            val lat = home.lat
            val lng = home.lng
            if (lat != null && lng != null){
                val location = Geopoint(lat, lng)
                pushpin.location = location
                pushpin.image = mapImage
                pushpin.normalizedAnchorPoint = PointF(0.5f, 1.0f)
                mapView.setScene(
                    MapScene.createFromLocationAndZoomLevel(location, 18.0),
                    MapAnimationKind.LINEAR
                )
                val flyout = MapFlyout()
                flyout.title = home.name?:""
                flyout.description = home.description?:""
                pushpin.flyout = flyout
                mPinLayer.elements.add(pushpin)
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

        when (userLocationTrackingState) {
            MapUserLocationTrackingState.PERMISSION_DENIED -> {
                // request for user location permissions and then call startTracking again
                requestLocationPermission()
            }
            MapUserLocationTrackingState.READY -> {
                // handle the case where location tracking was successfully started
                userLocation.trackingMode = MapUserLocationTrackingMode.CENTERED_ON_USER
            }
            MapUserLocationTrackingState.DISABLED -> {
                // handle the case where all location providers were disabled
            }
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
        @Suppress("DEPRECATION")
        super.onRequestPermissionsResult(requestCode, permissions, grantResults)

        EasyPermissions.onRequestPermissionsResult(requestCode, permissions, grantResults, this)
    }
}