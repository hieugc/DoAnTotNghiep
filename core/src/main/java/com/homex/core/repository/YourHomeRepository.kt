package com.homex.core.repository

import androidx.lifecycle.LiveData
import com.google.gson.JsonObject
import com.homex.core.model.Home
import com.homex.core.model.general.ResultResponse
import com.homex.core.model.response.MyHomeResponse
import com.homex.core.param.yourhome.IdParam
import okhttp3.MultipartBody
import okhttp3.RequestBody

interface YourHomeRepository {
    suspend fun createHome(
        body: RequestBody
    ): LiveData<ResultResponse<JsonObject>>

    suspend fun editHome(
        body: RequestBody
    ): LiveData<ResultResponse<JsonObject>>

    suspend fun deleteHome(
        id: Int
    ): LiveData<ResultResponse<JsonObject>>

    suspend fun getMyHomes(
        page: Int
    ): LiveData<ResultResponse<MyHomeResponse>>

    suspend fun getHomeByDetails(
        id: Int
    ): LiveData<ResultResponse<Home>>

    suspend fun getHomeByUser(
        userAccess: String
    ): LiveData<ResultResponse<ArrayList<Home>>>
}