using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicJukebox : MonoBehaviour {

	// Script Credit: http://answers.unity3d.com/questions/489495/change-to-another-song-when-song-is-finish-playing.html
	private AudioSource audio;
	private int currentTrack = 0;
	private string[] musicClips = {
		"Audio/Music/HiddenPast",
		"Audio/Music/Moorland",
	};
	// Use this for initialization
	void Awake() {
		audio = GetComponent<AudioSource> ();
	}

	void Start () {
		StartAudio ();
	}

	void StartAudio(){
		audio.clip = Resources.Load (musicClips [currentTrack]) as AudioClip;
		audio.Play ();
		currentTrack++;
		if (currentTrack >= musicClips.Length) {
			currentTrack = 0;
		}
		Invoke ("StartAudio", audio.clip.length + 0.5f);

	}
}
