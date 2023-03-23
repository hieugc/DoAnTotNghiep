package com.homex.core.param.chat

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

class GetMessagesParam(
    val idRoom: Int,
    val pagination: Pagination
)

@Parcelize
class Pagination(
    val page: Int,
    val limit: Int
): Parcelable