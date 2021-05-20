using System.Collections.Generic;
using UnityEngine;

public static class MusicPlayer
{
    public static AudioSource[] audioSource = new AudioSource[2];
    public static List<AudioClip> trackList = new List<AudioClip>();
    public static float[] volume = new float[2];

    // Used to set up the music player.
    public static void Initialize()
    {
        Object[] trackListArray = Resources.LoadAll("Audio/Music/", typeof(AudioClip));
        foreach (var c in trackListArray)
        {
            Debug.Log("Music Player: Added " + c.name + " to the Track List.");
            trackList.Add(c as AudioClip);
        }

        audioSource[0] = new GameObject("Music Player").AddComponent<AudioSource>();
        audioSource[1] = new GameObject("Music Player 2").AddComponent<AudioSource>();
        audioSource[0].loop = true;
        audioSource[1].loop = true;
        audioSource[0].tag = "Music Player";
        audioSource[1].tag = "Music Player";

        GameObject.DontDestroyOnLoad(audioSource[0].gameObject);
        GameObject.DontDestroyOnLoad(audioSource[1].gameObject);
    }

    #region PlayTrack Methods

    public static bool Exists()
    {
        return GameObject.FindGameObjectWithTag("Music Player");
    }

    // Plays the current AudioClip.
    public static void Play(int audioSourceNum)
    {
        try
        {
            audioSource[audioSourceNum].Play();
        }
        catch
        {
            Debug.LogError("The Music Player is not playing any AudioClips.\nPerhaps you intended to use another Play method?");
        }
    }

    // Plays an AudioClip based on an AudioClip.
    public static void Play(int audioSourceNum, AudioClip c)
    {
        try
        {
            audioSource[audioSourceNum].clip = c;
            audioSource[audioSourceNum].Play();
        }
        catch
        {
            Debug.LogError("The Music Player does not contain a track with the AudioClip of " + c + ".\nPerhaps you need to add it to the Tracklist or use another clip?");
        }
    }

    // Plays an AudioClip based on an index of the trackList array.
    public static void Play(int audioSourceNum, int i)
    {
        try
        {
            audioSource[audioSourceNum].clip = trackList[i];
            audioSource[audioSourceNum].Play();
        }
        catch
        {
            Debug.LogError("The Music Player does not contain a track at index " + i + ".\nPerhaps you need to add it to the Tracklist or use another clip?");
        }
    }

    // Plays an AudioClip based on a name as a string.
    public static void Play(int audioSourceNum, string s)
    {
        try
        {
            foreach (AudioClip c in trackList)
            {
                if (c.name == s)
                {
                    audioSource[audioSourceNum].clip = c;
                    audioSource[audioSourceNum].Play();

                    break;
                }
            }
        }
        catch
        {
            Debug.LogError("The Music Player does not contain a track named " + s + "./nPerhaps you need to add it to the Tracklist or use another clip?");
        }
    }

    #endregion

    #region Return Track AudioClips

    // Returns an AudioClip based on a name.
    public static AudioClip Track(int audioSourceNum, string s)
    {
        foreach (AudioClip c in trackList)
        {
            if (c.name == s)
                return c;
        }

        return null;
    }

    // Returns an AudioClip based on an int.
    public static AudioClip Track(int audioSourceNum, int i)
    {
        try
        {
            return trackList[i];
        }
        catch
        {
            Debug.LogError("There is no track at index " + i + " of Music Player.");
        }

        return null;
    }

    // Returns the AudioClip that's playing.
    public static AudioClip CurrentTrack(int audioSourceNum)
    {
        if (audioSource[audioSourceNum].isPlaying)
            return audioSource[audioSourceNum].clip;

        return null;
    }

    #endregion

    // Returns whether there's anything playing.
    public static bool IsPlaying(int audioSourceNum)
    {
        return audioSource[audioSourceNum].isPlaying;
    }

    // Changes the volume of the MusicPlayer.
    public static void SetVolume(int audioSourceNum, float f)
    {
        volume[audioSourceNum] = f;
        audioSource[audioSourceNum].volume = volume[audioSourceNum];
    }

    public static void ChangeVolume(int audioSourceNum, float f)
    {
        volume[audioSourceNum] -= f;
        audioSource[audioSourceNum].volume = volume[audioSourceNum];
    }

    // Mutes the audio playback.
    public static void Mute(int audioSourceNum)
    {
        volume[audioSourceNum] = 0f;
        audioSource[audioSourceNum].volume = 0f;
    }

    #region Playback Controls

    // Stops the music playback and removes the current AudioClip.
    public static void Pause(int audioSourceNum)
    {
        audioSource[audioSourceNum].Stop();
    }

    // Stops the music playback and removes the current AudioClip.
    public static void Stop(int audioSourceNum)
    {
        audioSource[audioSourceNum].Stop();
        audioSource[audioSourceNum].clip = null;
    }

    // Restarts the current track.
    public static void Restart(int audioSourceNum)
    {
        audioSource[audioSourceNum].Stop();
        audioSource[audioSourceNum].Play();
    }

    // Goes to the next track.
    public static void Next(int audioSourceNum)
    {
        try
        {
            audioSource[audioSourceNum].clip = trackList[trackList.IndexOf(audioSource[audioSourceNum].clip) + 1];
            audioSource[audioSourceNum].Play();
        }
        catch
        {
            Debug.LogError("There is no track after " + audioSource[audioSourceNum].clip.name + " in the Track List.");
        }
    }

    // Goes to the previous track.
    public static void Previous(int audioSourceNum)
    {
        try
        {
            audioSource[audioSourceNum].clip = trackList[trackList.IndexOf(audioSource[audioSourceNum].clip) - 1];
            audioSource[audioSourceNum].Play();
        }
        catch
        {
            Debug.LogError("There is no track before " + audioSource[audioSourceNum].clip.name + " in the Track List.");
        }
    }

    #endregion
}