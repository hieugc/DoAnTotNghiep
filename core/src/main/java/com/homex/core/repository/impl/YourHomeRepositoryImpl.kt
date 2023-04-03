package com.homex.core.repository.impl

import androidx.lifecycle.LiveData
import com.google.gson.JsonObject
import com.homex.core.api.ApiService
import com.homex.core.data.NetworkBoundResource
import com.homex.core.model.BingLocation
import com.homex.core.model.Home
import com.homex.core.model.general.ListResponse
import com.homex.core.model.general.ObjectResponse
import com.homex.core.model.general.ResultResponse
import com.homex.core.model.response.MyHomeResponse
import com.homex.core.param.yourhome.IdParam
import com.homex.core.repository.YourHomeRepository
import okhttp3.MultipartBody
import okhttp3.RequestBody
import retrofit2.Response

class YourHomeRepositoryImpl(private val api: ApiService): YourHomeRepository {
    override suspend fun createHome(body: RequestBody): LiveData<ResultResponse<JsonObject>> {
        return object : NetworkBoundResource<ObjectResponse<JsonObject>, JsonObject>(){
            override fun processResponse(response: ObjectResponse<JsonObject>): JsonObject? = response.data
            override suspend fun createCall(): Response<ObjectResponse<JsonObject>> = api.createNewHome(body)
        }.build().asLiveData()
    }

    override suspend fun editHome(
        body: RequestBody
    ): LiveData<ResultResponse<JsonObject>> {
        return object : NetworkBoundResource<ObjectResponse<JsonObject>, JsonObject>(){
            override fun processResponse(response: ObjectResponse<JsonObject>): JsonObject? = response.data
            override suspend fun createCall(): Response<ObjectResponse<JsonObject>> = api.editHome(body)
        }.build().asLiveData()
    }

    override suspend fun deleteHome(id: Int): LiveData<ResultResponse<JsonObject>> {
        return object : NetworkBoundResource<ObjectResponse<JsonObject>, JsonObject>(){
            override fun processResponse(response: ObjectResponse<JsonObject>): JsonObject? = response.data
            override suspend fun createCall(): Response<ObjectResponse<JsonObject>> = api.deleteHome(id)
        }.build().asLiveData()
    }

    override suspend fun getMyHomes(page: Int): LiveData<ResultResponse<MyHomeResponse>> {
        return object : NetworkBoundResource<ObjectResponse<MyHomeResponse>, MyHomeResponse>(){
            override fun processResponse(response: ObjectResponse<MyHomeResponse>): MyHomeResponse? = response.data
            override suspend fun createCall(): Response<ObjectResponse<MyHomeResponse>> = api.getMyHome(page)
        }.build().asLiveData()
    }

    override suspend fun getHomeByDetails(id: Int): LiveData<ResultResponse<Home>> {
        return object : NetworkBoundResource<ObjectResponse<Home>, Home>(){
            override fun processResponse(response: ObjectResponse<Home>): Home? = response.data
            override suspend fun createCall(): Response<ObjectResponse<Home>> = api.getHomeDetails(id)
        }.build().asLiveData()
    }

    override suspend fun getHomeByUser(userAccess: String): LiveData<ResultResponse<ArrayList<Home>>> {
        return object : NetworkBoundResource<ListResponse<Home>, ArrayList<Home>>(){
            override fun processResponse(response: ListResponse<Home>): ArrayList<Home>? = response.data
            override suspend fun createCall(): Response<ListResponse<Home>> = api.getHomeByUser(userAccess)
        }.build().asLiveData()
    }

    override suspend fun getCity(): LiveData<ResultResponse<ArrayList<BingLocation>>> {
        return object : NetworkBoundResource<ListResponse<BingLocation>, ArrayList<BingLocation>>(){
            override fun processResponse(response: ListResponse<BingLocation>): ArrayList<BingLocation>? = response.data
            override suspend fun createCall(): Response<ListResponse<BingLocation>> = api.getCity()
        }.build().asLiveData()
    }

    override suspend fun getDistrict(id: Int): LiveData<ResultResponse<ArrayList<BingLocation>>> {
        return object : NetworkBoundResource<ListResponse<BingLocation>, ArrayList<BingLocation>>(){
            override fun processResponse(response: ListResponse<BingLocation>): ArrayList<BingLocation>? = response.data
            override suspend fun createCall(): Response<ListResponse<BingLocation>> = api.getDistrict(id)
        }.build().asLiveData()
    }

    override suspend fun getWard(id: Int): LiveData<ResultResponse<ArrayList<BingLocation>>> {
        return object : NetworkBoundResource<ListResponse<BingLocation>, ArrayList<BingLocation>>(){
            override fun processResponse(response: ListResponse<BingLocation>): ArrayList<BingLocation>? = response.data
            override suspend fun createCall(): Response<ListResponse<BingLocation>> = api.getWard(id)
        }.build().asLiveData()
    }
}