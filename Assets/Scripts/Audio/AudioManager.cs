using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

[Serializable]
public class MusicTrack
{
    public string trackName;
    public string resourcePath;
    [Range(0f, 1f)]
    public float volume = 1f;
    [HideInInspector]
    public AudioSource audioSource;
}

[Serializable]
public class SceneMusic
{
    public string sceneName;
    public string musicTrackName;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music Settings")]
    [SerializeField] private List<MusicTrack> musicTracks = new List<MusicTrack>();
    [SerializeField] private float crossFadeDuration = 2.0f;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Startup Music")]
    [SerializeField] private string startupTrackName = "";
    [SerializeField] private bool playMusicOnAwake = true;

    [Header("Scene-Specific Music")]
    [SerializeField] private List<SceneMusic> sceneMusics = new List<SceneMusic>();

    private MusicTrack currentTrack;
    private Coroutine fadeCoroutine;

    [Header("Audio Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float masterVolume = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 1f;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();

            // Subscribe to scene loading events
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find music for this scene
        string sceneName = scene.name;
        SceneMusic sceneMusic = sceneMusics.Find(sm => sm.sceneName == sceneName);

        if (sceneMusic != null && !string.IsNullOrEmpty(sceneMusic.musicTrackName))
        {
            PlayMusic(sceneMusic.musicTrackName);
        }
    }

    private void Start()
    {
        // Play startup music if enabled
        if (playMusicOnAwake && !string.IsNullOrEmpty(startupTrackName))
        {
            PlayMusic(startupTrackName);
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from scene events
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void InitializeAudioSources()
    {
        // Create audio sources for each track
        foreach (MusicTrack track in musicTracks)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = true;
            source.volume = 0;

            // Load the clip from Resources
            AudioClip clip = Resources.Load<AudioClip>(track.resourcePath);
            if (clip == null)
            {
                Debug.LogError($"Failed to load audio clip at path: {track.resourcePath}");
                continue;
            }

            source.clip = clip;
            track.audioSource = source;
        }
    }

    /// <summary>
    /// Play a music track by name with crossfading
    /// </summary>
    /// <param name="trackName">Name of the track to play</param>
    public void PlayMusic(string trackName)
    {
        MusicTrack track = musicTracks.Find(t => t.trackName == trackName);

        if (track == null)
        {
            Debug.LogError($"Music track '{trackName}' not found!");
            return;
        }

        if (track == currentTrack && track.audioSource.isPlaying)
        {
            // Already playing this track
            return;
        }

        // Start crossfade
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(CrossFadeMusic(track));
    }

    /// <summary>
    /// Stop all music with a fade out
    /// </summary>
    public void StopMusic()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeOutAllMusic());
    }

    /// <summary>
    /// Crossfade between the current track and a new track
    /// </summary>
    private IEnumerator CrossFadeMusic(MusicTrack newTrack)
    {
        // Start the new track at volume 0
        newTrack.audioSource.volume = 0;
        newTrack.audioSource.Play();

        float startTime = Time.time;
        float endTime = startTime + crossFadeDuration;

        // If we have a current track, fade it out
        AudioSource currentSource = currentTrack?.audioSource;
        float currentStartVolume = currentSource != null ? currentSource.volume : 0;

        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / crossFadeDuration;
            float curvedT = fadeCurve.Evaluate(t);

            // Fade in new track
            float targetVolume = newTrack.volume * musicVolume * masterVolume;
            newTrack.audioSource.volume = Mathf.Lerp(0, targetVolume, curvedT);

            // Fade out current track if it exists
            if (currentSource != null)
            {
                currentSource.volume = Mathf.Lerp(currentStartVolume, 0, curvedT);
            }

            yield return null;
        }

        // Ensure final volumes are correct
        newTrack.audioSource.volume = newTrack.volume * musicVolume * masterVolume;

        // Stop the old track
        if (currentSource != null)
        {
            currentSource.Stop();
            currentSource.volume = 0;
        }

        // Set the new current track
        currentTrack = newTrack;
    }

    /// <summary>
    /// Fade out all music
    /// </summary>
    private IEnumerator FadeOutAllMusic()
    {
        // If no music is playing, return
        if (currentTrack == null || !currentTrack.audioSource.isPlaying)
        {
            yield break;
        }

        float startTime = Time.time;
        float endTime = startTime + crossFadeDuration;
        float startVolume = currentTrack.audioSource.volume;

        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / crossFadeDuration;
            float curvedT = fadeCurve.Evaluate(t);

            // Fade out current track
            currentTrack.audioSource.volume = Mathf.Lerp(startVolume, 0, curvedT);

            yield return null;
        }

        // Ensure final volume is correct and stop the track
        currentTrack.audioSource.volume = 0;
        currentTrack.audioSource.Stop();
        currentTrack = null;
    }

    /// <summary>
    /// Set the master volume
    /// </summary>
    /// <param name="volume">Volume value between 0 and 1</param>
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    /// <summary>
    /// Set the music volume
    /// </summary>
    /// <param name="volume">Volume value between 0 and 1</param>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    /// <summary>
    /// Update all active audio source volumes
    /// </summary>
    private void UpdateVolumes()
    {
        if (currentTrack != null && currentTrack.audioSource != null)
        {
            currentTrack.audioSource.volume = currentTrack.volume * musicVolume * masterVolume;
        }
    }

    /// <summary>
    /// Load and add a new music track at runtime
    /// </summary>
    /// <param name="trackName">Unique name for the track</param>
    /// <param name="resourcePath">Path within Resources folder</param>
    /// <param name="volume">Volume multiplier for this track</param>
    public void AddMusicTrack(string trackName, string resourcePath, float volume = 1f)
    {
        // Check if track already exists
        if (musicTracks.Exists(t => t.trackName == trackName))
        {
            Debug.LogError($"Music track '{trackName}' already exists!");
            return;
        }

        // Create new track
        MusicTrack newTrack = new MusicTrack
        {
            trackName = trackName,
            resourcePath = resourcePath,
            volume = Mathf.Clamp01(volume)
        };

        // Create audio source for the track
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = true;
        source.volume = 0;

        // Load the clip from Resources
        AudioClip clip = Resources.Load<AudioClip>(resourcePath);
        if (clip == null)
        {
            Debug.LogError($"Failed to load audio clip at path: {resourcePath}");
            return;
        }

        source.clip = clip;
        newTrack.audioSource = source;

        // Add to list
        musicTracks.Add(newTrack);
    }

    /// <summary>
    /// Change the startup track
    /// </summary>
    /// <param name="trackName">Name of track to play on startup</param>
    public void SetStartupTrack(string trackName)
    {
        if (string.IsNullOrEmpty(trackName) || musicTracks.Exists(t => t.trackName == trackName))
        {
            startupTrackName = trackName;
        }
        else
        {
            Debug.LogWarning($"Cannot set startup track to '{trackName}' because track doesn't exist");
        }
    }

    /// <summary>
    /// Set scene-specific music
    /// </summary>
    /// <param name="sceneName">Name of the scene</param>
    /// <param name="trackName">Name of track to play for this scene</param>
    public void SetSceneMusic(string sceneName, string trackName)
    {
        // Check if entry already exists
        SceneMusic existing = sceneMusics.Find(sm => sm.sceneName == sceneName);

        if (existing != null)
        {
            existing.musicTrackName = trackName;
        }
        else
        {
            sceneMusics.Add(new SceneMusic
            {
                sceneName = sceneName,
                musicTrackName = trackName
            });
        }
    }
}