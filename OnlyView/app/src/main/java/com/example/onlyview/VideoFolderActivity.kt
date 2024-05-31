package com.example.onlyview
import android.content.Intent
import android.net.Uri
import android.os.Bundle
import android.provider.MediaStore
import android.view.View
import android.widget.AdapterView
import android.widget.Button
import android.widget.GridView
import androidx.appcompat.app.AppCompatActivity

class VideoFolderActivity : AppCompatActivity() {

    private lateinit var gridView: GridView
    private lateinit var videoAdapter: VideoGridAdapter
    private var videoList = ArrayList<Uri>()
    private var folderName: String? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_video_folder)

        gridView = findViewById(R.id.gridView)
        val buttonChangeView: Button = findViewById(R.id.buttonChangeView)

        folderName = intent.getStringExtra("folderName")
        supportActionBar?.title = folderName

        loadVideos()

        // Establecer un escuchador de clic para el botón de cambiar vista
        buttonChangeView.setOnClickListener {
            toggleGridViewLayout()
        }
    }

    private fun loadVideos() {
        videoList.clear()
        val projection = arrayOf(MediaStore.Video.Media._ID, MediaStore.Video.Media.DISPLAY_NAME)
        val selection = "${MediaStore.Video.Media.DATA} LIKE ?"
        val selectionArgs = arrayOf("%/$folderName/%")
        val cursor = contentResolver.query(MediaStore.Video.Media.EXTERNAL_CONTENT_URI, projection, selection, selectionArgs, null)
        cursor?.use {
            val idColumn = it.getColumnIndexOrThrow(MediaStore.Video.Media._ID)
            while (it.moveToNext()) {
                val id = it.getLong(idColumn)
                val contentUri = Uri.withAppendedPath(MediaStore.Video.Media.EXTERNAL_CONTENT_URI, id.toString())
                videoList.add(contentUri)
            }
        }
        videoAdapter = VideoGridAdapter(this, videoList)
        gridView.adapter = videoAdapter

        gridView.onItemClickListener = AdapterView.OnItemClickListener { _, _, position, _ ->
            val selectedVideoUri = videoList[position]
            val intent = Intent(this, VideoPlaybackActivity::class.java)
            intent.putExtra("videoUri", selectedVideoUri.toString())
            startActivity(intent)
        }
    }

    // Método para cambiar la vista del GridView
    private fun toggleGridViewLayout() {
        // Aquí puedes implementar la lógica para cambiar la vista del GridView
        // Por ejemplo, alternar entre una cuadrícula y una lista
    }
}
