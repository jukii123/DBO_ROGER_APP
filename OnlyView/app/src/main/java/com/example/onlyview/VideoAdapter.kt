package com.example.onlyview

import android.content.Context
import android.net.Uri
import android.provider.MediaStore
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ImageView
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView

import com.bumptech.glide.Glide // Importa la biblioteca Glide si aún no lo has hecho

class VideoAdapter(private val context: Context, private val videoList: ArrayList<Uri>) :
    RecyclerView.Adapter<VideoAdapter.VideoViewHolder>() {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): VideoViewHolder {
        val view = LayoutInflater.from(context).inflate(R.layout.video_item, parent, false)
        return VideoViewHolder(view)
    }

    override fun onBindViewHolder(holder: VideoViewHolder, position: Int) {
        val videoUri = videoList[position]
        // Utiliza Glide para cargar la miniatura del video en el ImageView
        Glide.with(context).load(videoUri).into(holder.thumbnail)
        holder.title.text = getVideoName(videoUri)
    }

    override fun getItemCount(): Int {
        return videoList.size
    }

    inner class VideoViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {
        val thumbnail: ImageView = itemView.findViewById(R.id.thumbnail)
        val title: TextView = itemView.findViewById(R.id.title)
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
}
