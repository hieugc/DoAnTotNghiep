package com.homex.core.param.request

class EditRequestParam(
    val idHouse: Int,
    val type: Int,
    val price: Int? = null,
    val idSwapHouse: Int? = null,
    val startDate: String,
    val endDate: String,
    val id: Int
)