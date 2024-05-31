package com.example.onlyview

import android.content.Context
import android.database.Cursor
import android.net.Uri
import android.provider.MediaStore
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.BaseAdapter
import android.widget.ImageView
import android.widget.TextView
import com.bumptech.glide.Glide

class VideoGridAdapter(private val context: Context, private val videoList: List<Uri>) : BaseAdapter() {

    override fun getCount(): Int {
        return videoList.size
    }

    override fun getItem(position: Int): Any {
        return videoList[position]
    }

    override fun getItemId(position: Int): Long {
        return position.toLong()
    }

    override fun getView(position: Int, convertView: View?, parent: ViewGroup?): View {
        val view: View
        val viewHolder: ViewHolder

        if (convertView == null) {
            view = LayoutInflater.from(context).inflate(R.layout.video_item, parent, false)
            viewHolder = ViewHolder(view)
            view.tag = viewHolder
        } else {
            view = convertView
            viewHolder = view.tag as ViewHolder
        }

        val videoUri = videoList[position]
        viewHolder.title.text = getVideoName(videoUri)

        // Usando Glide para cargar la miniatura del video
        Glide.with(context)
            .load(videoUri)
            .placeholder(R.drawable.ic_video_placeholder)  // Asegúrate de tener un recurso drawable llamado ic_video_placeholder
            .into(viewHolder.thumbnail)

        return view
    }

    private fun getVideoName(videoUri: Uri): String {
        var name = "Unknown"
        val projection = arrayOf(MediaStore.Video.Media.DISPLAY_NAME)
        context.contentResolver.query(videoUri, projection, null, null, null)?.use { cursor ->
            if (cursor.moveToFirst()) {
                val columnIndex = cursor.getColumnIndex(MediaStore.Video.Media.DISPLAY_NAME)
                if (columnIndex >= 0) {
                    name = cursor.getString(columnIndex)
                }
            }
        }
        return name
    }

    private class ViewHolder(view: View) {
        val title: TextView = view.findViewById(R.id.title)
        val thumbnail: ImageView = view.findViewById(R.id.thumbnail)
    }
}
