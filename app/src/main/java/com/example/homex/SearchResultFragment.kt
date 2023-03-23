package com.example.homex

import android.Manifest
import android.os.Bundle
import android.util.Log
import android.view.View
import androidx.lifecycle.Transformations.map
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.SearchHomeAdapter
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentSearchResultBinding
import com.microsoft.maps.*
import pub.devrel.easypermissions.EasyPermissions


class SearchResultFragment : BaseFragment<FragmentSearchResultBinding>(), EasyPermissions.PermissionCallbacks {
    override val layoutId: Int = R.layout.fragment_search_result

    private lateinit var adapter: SearchHomeAdapter
    private lateinit var mapView: MapView
    private val LAKE_WASHINGTON = Geopoint(47.609466, -122.265185)
    private lateinit var mPinLayer: MapElementLayer
    private var requestingLocationPermission = false
    private lateinit var userLocationTrackingState: MapUserLocationTrackingState
    private lateinit var userLocation: MapUserLocation


    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showMessage = false,
            showBottomNav = false,
            showMenu = false,
            showTitleApp = Pair(false, ""),
            showBoxChatLayout = Pair(false, null),
        )

        (activity as HomeActivity).showSearchLayout()

        mapView = MapView(requireContext(), MapRenderMode.RASTER)
        mapView.setCredentialsKey(BuildConfig.CREDENTIALS_KEY)
        binding.mapView.addView(mapView)
        mapView.onCreate(savedInstanceState)

        requestLocationPermission()

        val customMapStyleString = """
            {
                "version": "1.0",
                "settings": {
                    "landColor": "#e7e6e5",
                    "shadedReliefVisible": false
                },
                "elements": {
                    "vegetation": {
                        "fillColor": "#c5dea2"
                    },
                    "naturalPoint": {
                        "visible": false,
                        "labelVisible": false
                    },
                    "transportation": {
                        "labelOutlineColor": "#ffffff",
                        "fillColor": "#ffffff",
                        "strokeColor": "#d7d6d5"
                    },
                    "water": {
                        "fillColor": "#b1bdd6",
                        "labelColor": "#ffffff",
                        "labelOutlineColor": "#9aa9ca"
                    },
                    "structure": {
                        "fillColor": "#d7d6d5"
                    },
                    "indigenousPeoplesReserve": {
                        "visible": false
                    },
                    "military": {
                        "visible": false
                    }
                }
            }
        """.trimIndent()

        val style = MapStyleSheet.fromJson(customMapStyleString)
        if (style != null) {
            mapView.mapStyleSheet = style
        } else {
            // Custom style JSON is invalid
            mapView.mapStyleSheet = MapStyleSheets.roadDark()
        }

        val uiOptions: MapUserInterfaceOptions = mapView.userInterfaceOptions
        uiOptions.isTiltButtonVisible = false
        uiOptions.isTiltGestureEnabled = false

        userLocation = mapView.userLocation

        userLocationTrackingState = userLocation.startTracking(
            GPSMapLocationProvider.Builder(requireContext().applicationContext).build()
        )
        if (userLocationTrackingState == MapUserLocationTrackingState.PERMISSION_DENIED) {
            // request for user location permissions and then call startTracking again
            requestLocationPermission()
        } else if (userLocationTrackingState == MapUserLocationTrackingState.READY) {
            // handle the case where location tracking was successfully started
        } else if (userLocationTrackingState == MapUserLocationTrackingState.DISABLED) {
            // handle the case where all location providers were disabled
        }
    }

    override fun setEvent() {
        binding.iconFilter.setOnClickListener {
            findNavController().navigate(R.id.action_searchResultFragment_to_filterBottomSheetFragment)
        }
    }

    override fun onStart() {
        super.onStart()
        mapView.onStart()
        trackingUserLocation()
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

    override fun setView() {
        adapter =
            SearchHomeAdapter(
                arrayListOf(
                    "Nhà của Hiếu", "Nhà của tui", "Nhà Nhà Nhà", "Hello mudafakar"
                )
            )

        binding.searchHomeRecView.adapter = adapter
        val layoutManager = LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.searchHomeRecView.layoutManager = layoutManager
    }

    private fun requestLocationPermission(){
        if (EasyPermissions.hasPermissions(requireContext(), Manifest.permission.ACCESS_FINE_LOCATION)) {
            trackingUserLocation()
        } else if (!requestingLocationPermission) {
            requestingLocationPermission = true
            EasyPermissions.requestPermissions(this, "We use your location to suggest your destination.", 1, Manifest.permission.ACCESS_FINE_LOCATION)
        }
    }

    override fun onPermissionsGranted(requestCode: Int, perms: MutableList<String>) {
        requestingLocationPermission = false
        trackingUserLocation()
    }

    private fun trackingUserLocation(){
        userLocation = mapView.userLocation

        userLocationTrackingState = userLocation.startTracking(
            GPSMapLocationProvider.Builder(requireContext().applicationContext).build()
        )
        val locationData = userLocation.lastLocationData
        if (locationData != null){
            Log.e("lat", "${locationData.latitude}")
            Log.e("long", "${locationData.longitude}")
            mapView.setScene(
                MapScene.createFromLocationAndZoomLevel(Geopoint(locationData.latitude, locationData.longitude), 10.0),
                MapAnimationKind.LINEAR);

        }
    }

    override fun onPermissionsDenied(requestCode: Int, perms: MutableList<String>) {
        requestingLocationPermission = false
    }
}