// Initializes audio playback in Particle Room, plays song passed in through 'selectedSongInfo' object

using UnityEngine;
using System.Collections;
using NAudio;
using NAudio.Wave;

public class InitAudio : MonoBehaviour {

	private IWavePlayer waveOutDevice;
	private AudioFileReader audioFileReader;

	// Use this for initialization
	void Awake () {
		GameObject selectedSongInfo = GameObject.Find("selectedSongInfo");
		PreserveData[] pData = selectedSongInfo.GetComponents<PreserveData>();
		string path = pData[0].path;

		CloseWaveOut();
		PlayMp3(path);
//		CloseWaveOut();

//		audioSource = gameObject.AddComponent<AudioSource>();
//		WWW loadAudio = new WWW(pData[0].path);
//		audioSource.clip = loadAudio.GetAudioClipCompressed();
//		audioSource.Play();
	}

	void OnApplicationQuit()
	{
		CloseWaveOut();
	}

	private bool PlayMp3(string path)
	{
		try
		{
			waveOutDevice = new WaveOut();
			audioFileReader = new AudioFileReader(path);
			waveOutDevice.Init(audioFileReader);
			waveOutDevice.Play();
		}
		catch (System.Exception ex)
		{
			UnityEngine.Debug.LogWarning("Error! " + ex.Message);
		}
		return false;
	}

	private void CloseWaveOut()
	{
		if (waveOutDevice != null)
		{
			waveOutDevice.Stop();
		}
		if (audioFileReader != null)
		{
			audioFileReader.Dispose();
			audioFileReader = null;
		}
		if (waveOutDevice != null)
		{
			waveOutDevice.Dispose();
			waveOutDevice = null;
		}
	}
}
