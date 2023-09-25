using System;
using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour {
    public AudioClip mainMenuTheme;
    public AudioClip inGameTrack;

    private AudioSource audioSource;

    private static MusicManager _instance;

    [Range(0, 5)]
    public float fadeDuration = 1f;

    private float volume = 0.5f;

    public static MusicManager Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<MusicManager>();
                if (_instance == null) {
                    GameObject newObject = new GameObject("MusicManager");
                    _instance = newObject.AddComponent<MusicManager>();
                    DontDestroyOnLoad(newObject);
                }
            }

            return _instance;
        }
    }

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    private void Start() {
        Instance.PlayMainMenuTheme();
    }

    public void SetMusicVolume(float _volume) {
        volume = _volume;
        audioSource.volume = volume;
    }

    public void PlayMainMenuTheme() {
        StartCoroutine(FadeToNewTrack(mainMenuTheme));
    }

    public void PlayWorldTheme() {
        StartCoroutine(FadeToNewTrack(inGameTrack));
    }

    private IEnumerator FadeToNewTrack(AudioClip _newTrack) {
        float startVolume = volume;

        // Fade out current track.
        for (float t = 0; t < fadeDuration; t += Time.deltaTime) {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = 0;
        audioSource.clip = _newTrack;
        audioSource.Play();

        // Fade in new track.
        for (float t = 0; t < fadeDuration; t += Time.deltaTime) {
            audioSource.volume = Mathf.Lerp(0, startVolume, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = startVolume;
    }

    public void StopMusic() {
        audioSource.Stop();
    }
}