package com.example.homex.utils

import android.content.Context
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import androidx.recyclerview.widget.RecyclerView.Recycler
import kotlin.math.abs

class CenterZoomLayoutManager(context: Context?, orientation: Int, reverseLayout: Boolean, private val mShrinkAmount: Float, private val mShrinkDistance: Float, private val itemWidth: Double) :
    LinearLayoutManager(
        context,
        orientation,
        reverseLayout
    ) {

    override fun checkLayoutParams(lp: RecyclerView.LayoutParams): Boolean {
        lp.width = (width / itemWidth).toInt()
        return true
    }

    override fun onLayoutCompleted(state: RecyclerView.State) {
        super.onLayoutCompleted(state)
        scaleMiddleItem()
    }


    private fun scaleMiddleItem() {
        val midpoint = width / 2f
        val d1 = mShrinkDistance * midpoint
        val s0 = 1f
        for (i in 0 until childCount) {
            val child = getChildAt(i)
            val childMidpoint = (getDecoratedRight(child!!) + getDecoratedLeft(
                child
            )) / 2f
            val d = d1.coerceAtMost(abs(midpoint - childMidpoint))
            val scale = s0 - mShrinkAmount * d / d1
            child.scaleX = scale
            child.scaleY = scale
        }
    }

    override fun scrollHorizontallyBy(dx: Int, recycler: Recycler, state: RecyclerView.State): Int {
        val orientation = orientation
        return if (orientation == HORIZONTAL) {
            val scrolled = super.scrollHorizontallyBy(dx, recycler, state)
            scaleMiddleItem()
            scrolled
        } else {
            0
        }
    }
}