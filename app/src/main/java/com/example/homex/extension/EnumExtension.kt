package com.example.homex.extension

import android.content.Context
import android.graphics.drawable.Drawable
import androidx.core.content.ContextCompat
import com.example.homex.R

enum class Utilities{
    WIFI{
        override fun getDrawable(context: Context): Drawable? = ContextCompat.getDrawable(context, R.drawable.ic_wifi)
        override fun getString(context: Context): String = context.getString(R.string.wifi)
    },
    COMPUTER{
        override fun getDrawable(context: Context): Drawable? = ContextCompat.getDrawable(context, R.drawable.ic_computer)
        override fun getString(context: Context): String = context.getString(R.string.computer)
    },
    TV{
        override fun getDrawable(context: Context): Drawable? = ContextCompat.getDrawable(context, R.drawable.ic_tv)
        override fun getString(context: Context): String = context.getString(R.string.smart_tv)
    },
    BATHTUB{
        override fun getDrawable(context: Context): Drawable? = ContextCompat.getDrawable(context, R.drawable.ic_bath)
        override fun getString(context: Context): String = context.getString(R.string.bath_tub)
    },
    PARKING_LOT{
        override fun getDrawable(context: Context): Drawable? = ContextCompat.getDrawable(context, R.drawable.ic_parking_lot)
        override fun getString(context: Context): String = context.getString(R.string.pool)
    },
    AIR_CONDITIONER{
        override fun getDrawable(context: Context): Drawable? = ContextCompat.getDrawable(context, R.drawable.ic_air_conditioning)
        override fun getString(context: Context): String = context.getString(R.string.air_conditioner)
    },
    WASHING_MACHINE{
        override fun getDrawable(context: Context): Drawable? = ContextCompat.getDrawable(context, R.drawable.ic_wash_machine)
        override fun getString(context: Context): String = context.getString(R.string.washing_machine)
    },
    FRIDGE{
        override fun getDrawable(context: Context): Drawable? = ContextCompat.getDrawable(context, R.drawable.ic_fridge)
        override fun getString(context: Context): String = context.getString(R.string.fridge)
    },
    POOL{
        override fun getDrawable(context: Context): Drawable? = ContextCompat.getDrawable(context, R.drawable.ic_pool)
        override fun getString(context: Context): String = context.getString(R.string.pool)
    };

    abstract fun getDrawable(context: Context): Drawable?
    abstract fun getString(context: Context): String
}

enum class Rules{
    NO_SMOKING{
        override fun getDrawable(context: Context): Drawable? = ContextCompat.getDrawable(context, R.drawable.ic_no_smoking)
        override fun getString(context: Context): String = context.getString(R.string.no_smoking)
    },
    NO_PET{
        override fun getDrawable(context: Context): Drawable? = ContextCompat.getDrawable(context, R.drawable.ic_no_pet)
        override fun getString(context: Context): String = context.getString(R.string.no_pet)
    };

    abstract fun getDrawable(context: Context): Drawable?
    abstract fun getString(context: Context): String
}

enum class RequestStatus{
    WAITING,
    ACCEPTED,
    REJECTED,
    CHECK_IN,
    REVIEWING,
    DONE
}

enum class RequestType{
    NONE,
    BY_POINT,
    BY_HOME
}

enum class SearchBy{
    CLOSET,
    RATING,
    PRICE_LOW_TO_HIGH,
    PRICE_HIGH_TO_LOW
}

enum class Payment{
    ALL,
    VALID,
    USED
}

enum class NotificationType{
    REQUEST,
    COMMENT,
    RESPONSE,
    ADMIN_REPORT,
    MESSAGE,
    CIRCLE_SWAP,
    PAYMENT
}

enum class StatusWaitingRequest
{
    INIT,//mới khởi tạo
    IN_CIRCLE,//trong vòng + chưa xác nhận
    DISABLE,//không muốn vào vòng => không gợi ý nữa
    ACCEPT,//đã xác nhận
    CHECK_IN,
    CHECK_OUT,
    ENDED
}
