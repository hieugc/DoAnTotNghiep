package com.homex.core.param.yourhome

import com.homex.core.model.ImageBase

class CreateHomeParam(
    val name: String,
    val option:  Int,
    val description: String,
    val people: Int,
    val bathRoom: Int,
    val bedRoom: Int,
    val square: Int,
    val location: String,
    val price: Int,
    val utilities: List<Int>,
    val rules: List<Int>,
    val images: List<ImageBase>
)