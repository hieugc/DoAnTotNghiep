package com.homex.core.repository.impl

import androidx.lifecycle.LiveData
import com.homex.core.api.ApiService
import com.homex.core.data.NetworkBoundResource
import com.homex.core.model.Home
import com.homex.core.model.Location
import com.homex.core.model.LocationSuggestion
import com.homex.core.model.general.ListResponse
import com.homex.core.model.general.ObjectResponse
import com.homex.core.model.general.ResultResponse
import com.homex.core.model.response.SearchHomeResponse
import com.homex.core.repository.HomeRepository
import retrofit2.Response

class HomeRepositoryImpl(val api: ApiService): HomeRepository {
    override suspend fun getPopularLocation(): LiveData<ResultResponse<ArrayList<Location>>> {
        return object : NetworkBoundResource<ListResponse<Location>, ArrayList<Location>>(){
            override fun processResponse(response: ListResponse<Location>): ArrayList<Location>? = response.data

            override suspend fun createCall(): Response<ListResponse<Location>> = api.getPopularCity()
        }.build().asLiveData()
    }

    override suspend fun getPopularHome(): LiveData<ResultResponse<ArrayList<Home>>> {
        return object: NetworkBoundResource<ListResponse<Home>, ArrayList<Home>>(){
            override fun processResponse(response: ListResponse<Home>): ArrayList<Home>? = response.data

            override suspend fun createCall(): Response<ListResponse<Home>> = api.getPopularHome()
        }.build().asLiveData()
    }

    override suspend fun searchHome(
        idCity: Int,
        people: Int?,
        idDistrict: Int?,
        startDate: String?,
        endDate: String?,
        startPrice: Int?,
        endPrice: Int?,
        utilities: ArrayList<Int>?,
        sortBy: Int,
        page: Int,
        limit: Int
    ): LiveData<ResultResponse<SearchHomeResponse>> {
        return object : NetworkBoundResource<ObjectResponse<SearchHomeResponse>, SearchHomeResponse>(){
            override fun processResponse(response: ObjectResponse<SearchHomeResponse>): SearchHomeResponse? = response.data

            override suspend fun createCall(): Response<ObjectResponse<SearchHomeResponse>> = api.searchHome(idCity, people, idDistrict, startDate, endDate, startPrice, endPrice, utilities, sortBy, page, limit)
        }.build().asLiveData()
    }

    override suspend fun getLocationSuggestion(query: String): LiveData<ResultResponse<ArrayList<LocationSuggestion>>> {
        return object : NetworkBoundResource<ListResponse<LocationSuggestion>, ArrayList<LocationSuggestion>>(){
            override fun processResponse(response: ListResponse<LocationSuggestion>): ArrayList<LocationSuggestion>? = response.data
            override suspend fun createCall(): Response<ListResponse<LocationSuggestion>> = api.getLocationSuggestion(query)
        }.build().asLiveData()
    }
}