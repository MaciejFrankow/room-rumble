using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicLooper : MonoBehaviour
{
    public List<AudioClip> songs;
    private AudioSource audioSource;
    private int currentSongIndex = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (songs.Count > 0)
        {
            audioSource.clip = songs[currentSongIndex];
            audioSource.Play();
            StartCoroutine(PlayNextSong());
        }
    }

    IEnumerator PlayNextSong()
    {
        while (true)
        {
            yield return new WaitForSeconds(audioSource.clip.length);

            currentSongIndex = (currentSongIndex + 1) % songs.Count;
            audioSource.clip = songs[currentSongIndex];
            audioSource.Play();
        }
    }
}
