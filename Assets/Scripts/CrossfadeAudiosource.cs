using System.Collections;
using UnityEngine;

public class CrossfadeAudiosource : MonoBehaviour {
	public void ChangeMusic(GameManager manager, AudioClip bg_music, float targetVolume) {
		if(manager.music == 1) { 
			StartCoroutine(FadeIt(bg_music, targetVolume));
		}
	}
	IEnumerator FadeIt(AudioClip clip, float volume) {

		///Add new audiosource and set it to all parameters of original audiosource
		AudioSource fadeOutSource = gameObject.AddComponent<AudioSource>();
		fadeOutSource.clip = GetComponent<AudioSource>().clip;
		fadeOutSource.time = GetComponent<AudioSource>().time;
		fadeOutSource.volume = GetComponent<AudioSource>().volume;
		fadeOutSource.outputAudioMixerGroup = GetComponent<AudioSource>().outputAudioMixerGroup;

		//make it start playing
		fadeOutSource.Play();

		//set original audiosource volume and clip
		GetComponent<AudioSource>().volume = 0f;
		GetComponent<AudioSource>().clip = clip;
		float t = 0;
		float v = fadeOutSource.volume;
		GetComponent<AudioSource>().Play();

		//begin fading in original audiosource with new clip as we fade out new audiosource with old clip
		while(t < 0.98f) {
			t = Mathf.Lerp(t, 1f, Time.deltaTime * 0.75f);
			fadeOutSource.volume = Mathf.Lerp(v, 0f, t);
			GetComponent<AudioSource>().volume = Mathf.Lerp(0f, volume, t);
			yield return null;
		}
		GetComponent<AudioSource>().volume = volume;
		//destroy the fading audiosource
		Destroy(fadeOutSource);
		yield break;
	}
}