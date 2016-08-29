using UnityEngine;
using System.Collections;

public class RandomMusic : MonoBehaviour {

	public AudioClip[] musicclips;
	private AudioSource audiosource;

	// Use this for initialization
	void Start () {
		audiosource = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!audiosource.isPlaying) {
			audiosource.clip = musicclips [Random.Range (0, musicclips.Length - 1)];
			audiosource.Play ();
		}
	}
}
