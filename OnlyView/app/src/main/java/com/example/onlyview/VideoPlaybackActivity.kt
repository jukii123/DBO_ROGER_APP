package com.example.onlyview

import android.net.Uri
import android.os.Bundle
import android.widget.ImageButton
import android.widget.SeekBar
import android.widget.VideoView
import androidx.appcompat.app.AppCompatActivity

class VideoPlaybackActivity : AppCompatActivity() {

    private lateinit var videoView: VideoView
    private lateinit var seekBar: SeekBar
    private lateinit var pauseButton: ImageButton

    private var isPlaying = false

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_video_playback)

        videoView = findViewById(R.id.videoView)
        seekBar = findViewById(R.id.seekBar)
        pauseButton = findViewById(R.id.pauseButton)

        val videoUriString = intent.getStringExtra("videoUri")
        videoUriString?.let { uriString ->
            val videoUri = Uri.parse(uriString)
            videoView.setVideoURI(videoUri)
            videoView.setOnPreparedListener {
                // Configurar la duración máxima de la barra de progreso
                seekBar.max = videoView.duration
                // Iniciar la reproducción del video
                videoView.start()
                isPlaying = true
                // Actualizar la posición del indicador de progreso cada segundo
                updateSeekBar()
            }
        }

        // Manejar clics en el botón de pausa
        pauseButton.setOnClickListener {
            if (isPlaying) {
                videoView.pause()
                isPlaying = false
                pauseButton.setImageResource(R.drawable.ic_play)
            } else {
                videoView.start()
                isPlaying = true
                pauseButton.setImageResource(R.drawable.ic_pause)
                updateSeekBar()
            }
        }

        // Actualizar la posición del video cuando se mueve la barra de progreso
        seekBar.setOnSeekBarChangeListener(object : SeekBar.OnSeekBarChangeListener {
            override fun onProgressChanged(seekBar: SeekBar?, progress: Int, fromUser: Boolean) {
                if (fromUser) {
                    videoView.seekTo(progress)
                }
            }

            override fun onStartTrackingTouch(seekBar: SeekBar?) {
                // No se necesita implementar
            }

            override fun onStopTrackingTouch(seekBar: SeekBar?) {
                // No se necesita implementar
            }
        })
    }

    private fun updateSeekBar() {
        // Actualizar la posición del indicador de progreso cada segundo
        val currentPosition = videoView.currentPosition
        seekBar.progress = currentPosition
        if (isPlaying) {
            seekBar.postDelayed({ updateSeekBar() }, 1000)
        }
    }

    override fun onDestroy() {
        super.onDestroy()
        videoView.stopPlayback()
    }
}
