using UnityEngine;
using System.Collections;
using NAudio;
using NAudio.Wave;

public class InitAudio : MonoBehaviour {

	private IWavePlayer mWaveOutDevice;
	private WaveStream mMainOutputStream;
	private WaveChannel32 mVolumeStream;
	private AudioSource audioSource;


	// Use this for initialization
	void Awake () {
		GameObject selectedSongInfo = GameObject.Find("selectedSongInfo");
		audioSource = gameObject.AddComponent<AudioSource>();
		PreserveData[] pData = selectedSongInfo.GetComponents<PreserveData>();
		WWW loadAudio = new WWW(pData[0].path);
		audioSource.clip = loadAudio.GetAudioClipCompressed();
		audioSource.Play();
	}

	private bool LoadAudioFromData(byte[] data)
	{
		try
		{
			BitStream tmpStr = new BitStream(data);
			mMainOutputStream = new Mp3FileReader(tmpStr);
			mVolumeStream = new WaveChannel32(mMainOutputStream);
			mWaveOutDevice = new WaveOut();
			mWaveOutDevice.Init(mVolumeStream);
			return true;
		}
		catch (System.Exception ex)
		{
			Debug.LogWarning("Error! " + ex.Message);
		}
		return false;
	}
}
