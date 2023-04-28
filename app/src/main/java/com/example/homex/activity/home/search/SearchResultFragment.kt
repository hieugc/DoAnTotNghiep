package com.example.homex.activity.home.search

import android.Manifest
import android.graphics.BitmapFactory
import android.graphics.Color
import android.graphics.PointF
import android.location.LocationManager
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.util.Log
import android.view.View
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.bumptech.glide.Glide
import com.example.homex.BuildConfig
import com.example.homex.R
import com.example.homex.activity.home.HomeActivity
import com.example.homex.adapter.SearchHomeAdapter
import com.example.homex.app.*
import com.example.homex.base.BaseFragment
import com.example.homex.databinding.FragmentSearchResultBinding
import com.example.homex.viewmodel.HomeViewModel
import com.homex.core.model.Home
import com.homex.core.util.AppEvent
import com.microsoft.maps.*
import com.microsoft.maps.search.MapLocationFinder
import com.microsoft.maps.search.MapLocationFinderStatus
import com.microsoft.maps.search.MapLocationOptions
import org.koin.androidx.viewmodel.ext.android.viewModel
import pub.devrel.easypermissions.EasyPermissions
import pub.devrel.easypermissions.PermissionRequest
import java.net.URL
import java.util.*
import java.util.concurrent.TimeUnit


class SearchResultFragment : BaseFragment<FragmentSearchResultBinding>(), EasyPermissions.PermissionCallbacks {
    override val layoutId: Int = R.layout.fragment_search_result

    private lateinit var adapter: SearchHomeAdapter
    private var homeList = arrayListOf<Home>()
    private lateinit var mapView: MapView
    private lateinit var mPinLayer: MapElementLayer
    private var requestingLocationPermission = false
    private lateinit var userLocationTrackingState: MapUserLocationTrackingState
    private lateinit var userLocation: MapUserLocation
    private var page = 1
    private val viewModel: HomeViewModel by viewModel()
    private lateinit var mapImage: MapImage



    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        (activity as HomeActivity).setPropertiesScreen(
            showLogo = false,
            showMessage = false,
            showBottomNav = false,
            showMenu = false,
            showTitleApp = Pair(false, ""),
            showBoxChatLayout = Pair(false, null),
            showSearchLayout = true
        )

        val idCity = arguments?.getInt(CITY)
        val idDistrict = arguments?.getInt(DISTRICT)
        val people = arguments?.getInt(PEOPLE)
        val startDate = arguments?.getString(START_DATE)
        val endDate = arguments?.getString(END_DATE)
        val cityName = arguments?.getString(CITY_NAME)
        val districtName = arguments?.getString(DISTRICT_NAME)
        Log.e("cittyName", "$cityName")
        if (idCity != null && idCity != 0) {
            viewModel.searchHome(
                idCity = idCity,
                people = people,
                idDistrict = idDistrict,
                startDate = startDate,
                endDate = endDate,
                sortBy = 0,
                page = page++,
                limit = 20
            )
        }
        var location = ""
        Log.e("cittyName", "$idDistrict")
        if (idDistrict != 0){
            location = "$districtName, $cityName"
        }else{
            if (cityName != null) {
                location = cityName
            }
        }
        (activity as HomeActivity).setSearchParam(location, startDate?:"Bất kỳ", endDate?:"Bấy kỳ")

        //Setup Map
        mapView = MapView(requireContext(), MapRenderMode.VECTOR)
        mapView.setCredentialsKey(BuildConfig.CREDENTIALS_KEY)
        binding.mapView.addView(mapView)

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

    override fun setEvent() {
        binding.iconFilter.setOnClickListener {
            findNavController().navigate(R.id.action_searchResultFragment_to_filterBottomSheetFragment)
        }
    }

    override fun onStart() {
        super.onStart()
        mapView.onStart()
//        mapView.setScene(
//            MapScene.createFromLocationAndZoomLevel(Geopoint(51.50632, -0.12714), 10.0),
//            MapAnimationKind.LINEAR
//        )

//        val location = Geopoint(47.599025, -122.339901)
//        val pushpin =  MapIcon()
//        pushpin.location = location
//        pushpin.title = "Seattle"
//        pushpin.image = MapImage(
//            BitmapFactory.decodeResource(resources, R.mipmap.location_pin)
//        )
//        pushpin.normalizedAnchorPoint = PointF(0.5f, 1.0f)
//
//        val flyout = MapFlyout()
//        flyout.title = "Test"
//        flyout.description = "Sample description"
//        pushpin.flyout = flyout
//
//        mPinLayer.elements.add(pushpin)
//        val seattle =
//            GeoboundingBox(Geoposition(47.599025, -122.339901), Geoposition(47.589908, -122.313251))
//        mapView.setScene(MapScene.createFromBoundingBox(seattle), MapAnimationKind.LINEAR)
//
//        val url = URL("https://bingmapsisdk.blob.core.windows.net/isdksamples/us_counties.png")
//        val boundingBox = GeoboundingBox(Geoposition(50.0,  -126.0),
//        Geoposition(25.0, -66.0))
//
//        try{
//            Thread().run {
//                val image = Glide.with(requireContext())
//                    .asBitmap()
//                    .load(url.toString())
//                    .submit()
//                    .get()
//                val groundOverlayLayer = GroundOverlayMapLayer(
//                    image,
//                    boundingBox)
//
//                val scene = MapScene.createFromLocationAndZoomLevel(Geopoint(40.0, -98.0), 4.0)
//                mapView.setScene(scene, MapAnimationKind.NONE)
//                mapView.layers.add(groundOverlayLayer)
//            }
//
//        }
//        catch (e: Exception){
//            Log.e("exception", e.message.toString())
//        }
        //highlightArea()
        //trackingUserLocation()
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
                homeList
            ){}

        binding.searchHomeRecView.adapter = adapter
        val layoutManager = LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false)
        binding.searchHomeRecView.layoutManager = layoutManager
    }

    override fun setViewModel() {
        viewModel.searchList.observe(this){
            if (it != null){
//                val pagination = it.pagination
//                if (pagination != null){
//                    binding.numberOfHomeFoundTV.text = "${pagination.total} kết quả tìm được"
//                }
                if (page == 1){
                    homeList.clear()
                }
                val houses = it.houses
                if (houses != null) {
                    homeList.addAll(houses)
                    var setScene = false
                    for (home in houses){
                        val pushpin =  MapIcon()
                        val lat = home.lat?:continue
                        val lng = home.lng?:continue
                        val location = Geopoint(lat, lng)
                        pushpin.location = location
                        pushpin.image = mapImage
                        pushpin.normalizedAnchorPoint = PointF(0.5f, 1.0f)

                        if (!setScene){
                            mapView.setScene(
                                MapScene.createFromLocationAndZoomLevel(location, 18.0),
                                MapAnimationKind.LINEAR
                            )
                            setScene = true
                        }

                        val flyout = MapFlyout()
                        flyout.title = home.name?:""
                        flyout.description = home.description?:""
                        pushpin.flyout = flyout
                        mPinLayer.elements.add(pushpin)
                    }
                }
                adapter.notifyDataSetChanged()
                binding.numberOfHomeFoundTV.text = "${homeList.size} kết quả tìm được"
            }
            AppEvent.closePopup()
        }
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

    fun drawLineOnMap() {
        val center = mapView.center.position
        val geopoints = ArrayList<Geoposition>()
        geopoints.add(Geoposition(center.latitude - 0.0005, center.longitude - 0.001))
        geopoints.add(Geoposition(center.latitude + 0.0005, center.longitude + 0.001))
        val mapPolyline = MapPolyline()
        mapPolyline.path = Geopath(geopoints)
        mapPolyline.strokeColor = Color.BLACK
        mapPolyline.strokeWidth = 3
        mapPolyline.isStrokeDashed = true

        // Add Polyline to a layer on the map control.
        val linesLayer = MapElementLayer()
        linesLayer.zIndex = 1.0f
        linesLayer.elements.add(mapPolyline)
        mapView.layers.add(linesLayer)
    }

    fun highlightArea() {
        val center = mapView.center.position
        val geopoints = ArrayList<Geoposition>()
        geopoints.add(Geoposition(center.latitude + 0.0005, center.longitude - 0.001))
        geopoints.add(Geoposition(center.latitude - 0.0005, center.longitude - 0.001))
        geopoints.add(Geoposition(center.latitude - 0.0005, center.longitude + 0.001))
        val mapPolygon = MapPolygon()
        mapPolygon.paths = listOf(Geopath(geopoints))
        mapPolygon.fillColor = Color.RED
        mapPolygon.strokeColor = Color.BLUE
        mapPolygon.strokeWidth = 3
        mapPolygon.isStrokeDashed = false
        val highlightsLayer = MapElementLayer()
        highlightsLayer.zIndex = 1.0f
        highlightsLayer.elements.add(mapPolygon)
        mapView.layers.add(highlightsLayer)
    }
}