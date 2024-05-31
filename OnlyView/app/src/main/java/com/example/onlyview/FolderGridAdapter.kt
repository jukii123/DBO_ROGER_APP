package com.example.onlyview

import android.content.Context
import android.net.Uri
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.BaseAdapter
import android.widget.ImageView
import android.widget.TextView
import com.bumptech.glide.Glide

class FolderGridAdapter(private val context: Context, private val folders: ArrayList<VideoFolder>) : BaseAdapter() {

    override fun getCount(): Int = folders.size

    override fun getItem(position: Int): Any = folders[position]

    override fun getItemId(position: Int): Long = position.toLong()

    override fun getView(position: Int, convertView: View?, parent: ViewGroup?): View {
        val folder = getItem(position) as VideoFolder
        val view = convertView ?: LayoutInflater.from(context).inflate(R.layout.item_folder, parent, false)

        val folderName = view.findViewById<TextView>(R.id.folderName)
        val folderThumbnail = view.findViewById<ImageView>(R.id.folderThumbnail)

        folderName.text = folder.name
        Glide.with(context).load(folder.uri).into(folderThumbnail)

        return view
    }
}
