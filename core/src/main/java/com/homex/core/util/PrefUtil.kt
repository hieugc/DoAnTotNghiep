package com.homex.core.util

import android.content.Context
import android.content.SharedPreferences
import com.google.gson.Gson
import com.google.gson.reflect.TypeToken
import com.homex.core.model.Profile

//KEY WORD
const val USER_PROFILE = "USER"
const val TOKEN = "TOKEN"
const val EVENT_ID = "EVENT_ID"
const val ALERT_SETTING = "ALERT_SETTING"
const val PROFILE_LIST = "PROFILE_LIST"
const val USER_PARTICIPANT = "USER_PARTICIPANT"

class PrefUtil constructor(
    private val context: Context,
    private val prefs: SharedPreferences,
    private val gSon: Gson
) {


    fun clearAllData() = prefs.edit().clear().commit()

    var profile: Profile?
        get() {
            return try {
                gSon.fromJson(
                    prefs.getString(USER_PROFILE, null),
                    Profile::class.java
                )
            } catch (e: Exception) {
                null
            }
        }
        set(value) = prefs.edit().putString(
            USER_PROFILE,
            gSon.toJson(value)
        ).apply()

    var token: String?
        get() = prefs.getString(TOKEN, null)
        set(value) = prefs.edit().putString(TOKEN, value).apply()

    var eventID: String?
        get() = prefs.getString(EVENT_ID, null)
        set(value) = prefs.edit().putString(EVENT_ID, value).apply()

    var listProfile: List<Profile>?
        get() {
            val serializedObj = prefs.getString(PROFILE_LIST, null)
            if(serializedObj != null){
                val type = object: TypeToken<List<Profile>>(){}.type
                return try{
                    gSon.fromJson(serializedObj, type)
                }catch (e: Exception){
                    null
                }
            }
            return null
        }
        set(value) = prefs.edit().putString(
            PROFILE_LIST,
            gSon.toJson(value)
        ).apply()

}