package com.example.onlyview

import android.content.Intent
import android.net.Uri
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.provider.MediaStore
import android.view.Menu
import android.view.MenuItem
import android.view.View
import android.view.animation.AlphaAnimation
import android.view.animation.Animation
import android.widget.ImageButton
import android.widget.SeekBar
import android.widget.Toast
import android.widget.VideoView
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.Toolbar
import androidx.core.app.ShareCompat

class VideoPlaybackActivity : AppCompatActivity() {

    private lateinit var videoView: VideoView
    private lateinit var seekBar: SeekBar
    private lateinit var pauseButton: ImageButton
    private lateinit var nextButton: ImageButton
    private lateinit var prevButton: ImageButton
    private lateinit var toolbar: Toolbar

    private var isPlaying = false
    private var isFavorite = false // Default value for favorite status
    private val handler = Handler(Looper.getMainLooper())
    private val updateSeekBarRunnable = object : Runnable {
        override fun run() {
            updateSeekBar()
            handler.postDelayed(this, 1000)
        }
    }

    private val hideControlsRunnable = object : Runnable {
        override fun run() {
            hideControls()
        }
    }

    private val delayMillis: Long = 3000 // Time in milliseconds to wait before hiding controls

    // List of video URIs
    private lateinit var videoList: MutableList<Uri>
    private var currentVideoIndex = 0
    private var currentVideoUri: Uri? = null // Store the current video URI

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_video_playback)

        toolbar = findViewById(R.id.toolbar)
        setSupportActionBar(toolbar)

        videoView = findViewById(R.id.videoView)
        seekBar = findViewById(R.id.seekBar)
        pauseButton = findViewById(R.id.pauseButton)
        nextButton = findViewById(R.id.nextButton)
        prevButton = findViewById(R.id.prevButton)

        // Initialize video list and current index
        val videoUriStringList = intent.getStringArrayListExtra("videoList") ?: arrayListOf()
        videoList = videoUriStringList.map { Uri.parse(it) }.toMutableList()

        val videoUriString = intent.getStringExtra("videoUri")
        val songTitle = intent.getStringExtra("songTitle")
        videoUriString?.let { uriString ->
            currentVideoUri = Uri.parse(uriString)
            currentVideoUri?.let { uri ->
                currentVideoIndex = videoList.indexOf(uri)
                videoView.setVideoURI(uri)
                videoView.setOnPreparedListener {
                    seekBar.max = videoView.duration
                    videoView.start()
                    isPlaying = true
                    handler.post(updateSeekBarRunnable)
                    showControls() // Show controls when video starts playing
                }
            }
        }

        songTitle?.let { title ->
            toolbar.title = title
        }

        pauseButton.setOnClickListener {
            if (isPlaying) {
                videoView.pause()
                isPlaying = false
                pauseButton.setImageResource(R.drawable.ic_play)
            } else {
                videoView.start()
                isPlaying = true
                pauseButton.setImageResource(R.drawable.ic_pause)
                handler.post(updateSeekBarRunnable)
            }
            showControls() // Show controls when paused
        }

        nextButton.setOnClickListener {
            playNextVideo()
        }

        prevButton.setOnClickListener {
            playPreviousVideo()
        }

        seekBar.setOnSeekBarChangeListener(object : SeekBar.OnSeekBarChangeListener {
            override fun onProgressChanged(seekBar: SeekBar?, progress: Int, fromUser: Boolean) {
                if (fromUser) {
                    videoView.seekTo(progress)
                }
            }

            override fun onStartTrackingTouch(seekBar: SeekBar?) {
                showControls() // Show controls when user interacts with seekbar
            }

            override fun onStopTrackingTouch(seekBar: SeekBar?) {
                hideControls() // Hide controls after user stops interacting with seekbar
            }
        })

        videoView.setOnErrorListener { _, _, _ ->
            Toast.makeText(this, "Error durante la reproducción", Toast.LENGTH_SHORT).show()
            true
        }

        // Hide controls after inactivity
        videoView.setOnTouchListener { _, _ ->
            showControls()
            handler.removeCallbacks(hideControlsRunnable)
            handler.postDelayed(hideControlsRunnable, delayMillis)
            true
        }
    }

    private fun playNextVideo() {
        if (videoList.isNotEmpty()) {
            currentVideoIndex = (currentVideoIndex + 1) % videoList.size
            playCurrentVideo()
        }
    }

    private fun playPreviousVideo() {
        if (videoList.isNotEmpty()) {
            currentVideoIndex = (currentVideoIndex - 1 + videoList.size) % videoList.size
            playCurrentVideo()
        }
    }

    private fun playCurrentVideo() {
        val currentVideoUri = videoList[currentVideoIndex]
        this.currentVideoUri = currentVideoUri // Update the current video URI
        videoView.setVideoURI(currentVideoUri)
        videoView.start()
        isPlaying = true
        handler.post(updateSeekBarRunnable)
        showControls()
        toolbar.title = getVideoName(currentVideoUri)
    }

    private fun getVideoName(videoUri: Uri): String {
        var name = "Unknown"
        val projection = arrayOf(MediaStore.Video.Media.DISPLAY_NAME)
        contentResolver.query(videoUri, projection, null, null, null)?.use { cursor ->
            if (cursor.moveToFirst()) {
                val columnIndex = cursor.getColumnIndex(MediaStore.Video.Media.DISPLAY_NAME)
                if (columnIndex >= 0) {
                    name = cursor.getString(columnIndex)
                }
            }
        }
        return name
    }

    private fun updateSeekBar() {
        val currentPosition = videoView.currentPosition
        seekBar.progress = currentPosition
    }

    private fun showControls() {
        val fadeIn = AlphaAnimation(0.0f, 1.0f).apply {
            duration = 500
        }

        findViewById<View>(R.id.toolbar).apply {
            visibility = View.VISIBLE
            startAnimation(fadeIn)
        }
        findViewById<View>(R.id.seekBar).apply {
            visibility = View.VISIBLE
            startAnimation(fadeIn)
        }
        findViewById<View>(R.id.pauseButton).apply {
            visibility = View.VISIBLE
            startAnimation(fadeIn)
        }
        findViewById<View>(R.id.nextButton).apply {
            visibility = View.VISIBLE
            startAnimation(fadeIn)
        }
        findViewById<View>(R.id.prevButton).apply {
            visibility = View.VISIBLE
            startAnimation(fadeIn)
        }
    }

    private fun hideControls() {
        val fadeOut = AlphaAnimation(1.0f, 0.0f).apply {
            duration = 500
        }
        fadeOut.setAnimationListener(object : Animation.AnimationListener {
            override fun onAnimationEnd(animation: Animation?) {
                findViewById<View>(R.id.toolbar).visibility = View.INVISIBLE
                findViewById<View>(R.id.seekBar).visibility = View.INVISIBLE
                findViewById<View>(R.id.pauseButton).visibility = View.INVISIBLE
                findViewById<View>(R.id.nextButton).visibility = View.INVISIBLE
                findViewById<View>(R.id.prevButton).visibility = View.INVISIBLE
            }

            override fun onAnimationRepeat(animation: Animation?) {}
            override fun onAnimationStart(animation: Animation?) {}
        })

        findViewById<View>(R.id.toolbar).startAnimation(fadeOut)
        findViewById<View>(R.id.seekBar).startAnimation(fadeOut)
        findViewById<View>(R.id.pauseButton).startAnimation(fadeOut)
        findViewById<View>(R.id.nextButton).startAnimation(fadeOut)
        findViewById<View>(R.id.prevButton).startAnimation(fadeOut)
    }

    override fun onCreateOptionsMenu(menu: Menu): Boolean {
        menuInflater.inflate(R.menu.menu_video_playback, menu)
        return true
    }

    override fun onOptionsItemSelected(item: MenuItem): Boolean {
        return when (item.itemId) {
            R.id.action_share -> {
                shareVideo()
                true
            }
            R.id.action_favorite -> {
                toggleFavorite(item)
                true
            }
            else -> super.onOptionsItemSelected(item)
        }
    }

    private fun shareVideo() {
        currentVideoUri?.let { uri ->
            val intent = ShareCompat.IntentBuilder.from(this)
                .setType("video/*")
                .setStream(uri)
                .setChooserTitle("Compartir video")
                .createChooserIntent()
                .apply {
                    addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION)
                }
            startActivity(intent)
        }
    }

    private fun toggleFavorite(item: MenuItem) {
        isFavorite = !isFavorite
        val iconResId = if (isFavorite) R.drawable.ic_favorite else R.drawable.ic_favorite_border
        item.icon = getDrawable(iconResId)
        // Aquí puedes guardar el estado de favorito en una base de datos o en SharedPreferences
    }

    override fun onResume() {
        super.onResume()
        if (isPlaying) {
            handler.post(updateSeekBarRunnable)
        }
    }

    override fun onPause() {
        super.onPause()
        handler.removeCallbacks(updateSeekBarRunnable)
    }

    override fun onDestroy() {
        super.onDestroy()
        handler.removeCallbacks(updateSeekBarRunnable)
        handler.removeCallbacks(hideControlsRunnable)
    }
}
