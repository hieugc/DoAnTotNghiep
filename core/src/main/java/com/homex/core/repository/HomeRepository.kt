package com.homex.core.repository

import androidx.lifecycle.LiveData
import com.homex.core.model.Home
import com.homex.core.model.Location
import com.homex.core.model.general.ResultResponse

interface HomeRepository {
    suspend fun getPopularLocation(): LiveData<ResultResponse<ArrayList<Location>>>

    suspend fun getPopularHome(): LiveData<ResultResponse<ArrayList<Home>>>
}