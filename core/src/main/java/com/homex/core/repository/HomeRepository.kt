package com.homex.core.repository

import androidx.lifecycle.LiveData
import com.homex.core.model.Home
import com.homex.core.model.Location
import com.homex.core.model.general.ResultResponse
import com.homex.core.model.response.SearchHomeResponse
import retrofit2.http.Query

interface HomeRepository {
    suspend fun getPopularLocation(): LiveData<ResultResponse<ArrayList<Location>>>

    suspend fun getPopularHome(): LiveData<ResultResponse<ArrayList<Home>>>

    suspend fun searchHome(
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
    ): LiveData<ResultResponse<SearchHomeResponse>>
}