package com.homex.core.model

import android.app.Dialog
import android.os.Parcel
import android.os.Parcelable
import android.view.View


data class PopUp(
    var isBlur: Boolean? = false,
    var popupId: Int? = 0,
    var blurAt: View? = null,
    var messageQueue: ArrayList<(view: View?, dialog: Dialog?) -> Unit>? = null) : Parcelable {
    constructor(parcel: Parcel) : this(
        parcel.readValue(Boolean::class.java.classLoader) as? Boolean,
        parcel.readValue(Int::class.java.classLoader) as? Int)

    override fun writeToParcel(parcel: Parcel, flags: Int) {
        parcel.writeValue(isBlur)
        parcel.writeValue(popupId)
    }

    override fun describeContents(): Int {
        return 0
    }

    companion object CREATOR : Parcelable.Creator<PopUp> {
        override fun createFromParcel(parcel: Parcel): PopUp {
            return PopUp(parcel)
        }

        override fun newArray(size: Int): Array<PopUp?> {
            return arrayOfNulls(size)
        }
    }
}