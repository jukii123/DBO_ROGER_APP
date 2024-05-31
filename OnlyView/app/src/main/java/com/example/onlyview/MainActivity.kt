package com.example.onlyview

import android.Manifest
import android.content.Intent
import android.content.pm.PackageManager
import android.net.Uri
import android.os.Build
import android.os.Bundle
import android.provider.MediaStore
import android.util.Log
import android.view.Menu
import android.view.MenuItem
import android.widget.AdapterView
import android.widget.GridView
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.Toolbar
import androidx.core.app.ActivityCompat
import androidx.core.content.ContextCompat
import java.io.File

class MainActivity : AppCompatActivity() {

    companion object {
        const val REQUEST_STORAGE_PERMISSION = 1001
    }

    private lateinit var gridView: GridView
    private lateinit var videoAdapter: VideoGridAdapter
    private var videoList = ArrayList<Uri>()
    private var folderList = ArrayList<VideoFolder>()
    private var isShowingFolders = true

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        val toolbar: Toolbar = findViewById(R.id.toolbar)
        setSupportActionBar(toolbar)

        gridView = findViewById(R.id.gridView)

        // Solicitar permiso de almacenamiento
        requestStoragePermission()
    }

    override fun onCreateOptionsMenu(menu: Menu): Boolean {
        menuInflater.inflate(R.menu.menu_main, menu)
        Log.d("MainActivity", "onCreateOptionsMenu called")
        return true
    }

    override fun onOptionsItemSelected(item: MenuItem): Boolean {
        return when (item.itemId) {
            R.id.action_view_all -> {
                isShowingFolders = !isShowingFolders
                loadVideos()
                true
            }
            else -> super.onOptionsItemSelected(item)
        }
    }

    private fun requestStoragePermission() {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
            if (ContextCompat.checkSelfPermission(this, Manifest.permission.READ_MEDIA_VIDEO) != PackageManager.PERMISSION_GRANTED) {
                ActivityCompat.requestPermissions(this, arrayOf(Manifest.permission.READ_EXTERNAL_STORAGE), REQUEST_STORAGE_PERMISSION)
            } else {
                loadVideos()
            }
        } else {
            loadVideos()
        }
    }

    override fun onRequestPermissionsResult(requestCode: Int, permissions: Array<String>, grantResults: IntArray) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults)
        if (requestCode == REQUEST_STORAGE_PERMISSION) {
            if ((grantResults.isNotEmpty() && grantResults[0] == PackageManager.PERMISSION_GRANTED)) {
                loadVideos()
            } else {
                Toast.makeText(this, "Storage permission is required to access videos", Toast.LENGTH_SHORT).show()
            }
        }
    }

    private fun loadVideos() {
        videoList.clear()
        folderList.clear()
        val projection = arrayOf(MediaStore.Video.Media._ID, MediaStore.Video.Media.DISPLAY_NAME, MediaStore.Video.Media.DATA)
        val cursor = contentResolver.query(MediaStore.Video.Media.EXTERNAL_CONTENT_URI, projection, null, null, null)
        cursor?.use {
            val idColumn = it.getColumnIndexOrThrow(MediaStore.Video.Media._ID)
            val dataColumn = it.getColumnIndexOrThrow(MediaStore.Video.Media.DATA)
            val displayNameColumn = it.getColumnIndexOrThrow(MediaStore.Video.Media.DISPLAY_NAME)
            while (it.moveToNext()) {
                val id = it.getLong(idColumn)
                val data = it.getString(dataColumn)
                val displayName = it.getString(displayNameColumn)
                val contentUri = Uri.withAppendedPath(MediaStore.Video.Media.EXTERNAL_CONTENT_URI, id.toString())

                if (isShowingFolders) {
                    val folder = File(data).parentFile
                    val folderName = folder?.name ?: "Unknown"
                    val videoFolder = folderList.find { it.name == folderName }
                    if (videoFolder == null) {
                        folderList.add(VideoFolder(folderName, contentUri))
                    }
                } else {
                    videoList.add(contentUri)
                }
            }
        }
        if (isShowingFolders) {
            val folderAdapter = FolderGridAdapter(this, folderList)
            gridView.adapter = folderAdapter

            gridView.onItemClickListener = AdapterView.OnItemClickListener { _, _, position, _ ->
                val selectedFolder = folderList[position]
                val intent = Intent(this, VideoFolderActivity::class.java)
                intent.putExtra("folderUri", selectedFolder.uri.toString())
                intent.putExtra("folderName", selectedFolder.name)
                startActivity(intent)
            }
        } else {
            videoAdapter = VideoGridAdapter(this, videoList)
            gridView.adapter = videoAdapter

            gridView.onItemClickListener = AdapterView.OnItemClickListener { _, _, position, _ ->
                val selectedVideoUri = videoList[position]
                val intent = Intent(this, VideoPlaybackActivity::class.java)
                intent.putExtra("videoUri", selectedVideoUri.toString())
                startActivity(intent)
            }
        }
    }
}
