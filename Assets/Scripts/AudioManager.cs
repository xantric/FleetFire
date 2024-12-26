using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public PauseMenu pauseMenu;
    private bool musicPlaying;

    [Header("-------Audio Source-------")]

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    public AudioSource movementSource;

    [Header("-------Audio Clip-------")]

    public AudioClip movement;
    public AudioClip background;
    public AudioClip death;
    public AudioClip jump;
    public AudioClip shooting;
    public AudioClip reload;



    private void Start() {
        musicSource.clip = background;
        musicSource.Play();
        movementSource.clip = movement;
    }

    private void Update() {
        if (pauseMenu.isPaused) {
            musicSource.Pause();
            musicPlaying = false;
        }
        else if (!pauseMenu.isPaused && !musicPlaying) {
            musicSource.Play();
            musicPlaying = true;
        }

    }

    public void PlaySFX(AudioClip clip) {
        sfxSource.PlayOneShot(clip);
    }
}