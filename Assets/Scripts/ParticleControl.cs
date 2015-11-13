// Controls particle system emission tendencies
//
// ONLY ATTACH PARTICLE SYSTEMS TO PARTICLECONTROLLER

using UnityEngine;
using System.Collections;

public class ParticleControl : MonoBehaviour {
	private Random rand;
	private int timer;
	private int curSelected;
	private Transform[] plist;

	// Use this for initialization
	void Awake () {
		plist = new Transform[transform.childCount];

		for(int i = 0; i < plist.Length; i++)
		{
			transform.GetChild(i).GetComponent<ParticleSystem>().enableEmission = false;
			plist[i] = transform.GetChild(i);
		}

		int seed = 0;
		GameObject selectedSongInfo = GameObject.Find("selectedSongInfo");
		// If we found the selectedSongInfo object, use artist info for seed.
		// Otherwise use 0
		if(selectedSongInfo)
		{
			PreserveData[] pData = selectedSongInfo.GetComponents<PreserveData>();
			string path = pData[0].path;

			//grab seed, we'll use path for now
			foreach(char c in path)
				seed += c;
		}

		Random.seed = seed;
		curSelected = Random.Range(0, plist.Length);
		plist[curSelected].GetComponent<ParticleSystem>().enableEmission = true;

		timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if(timer >= 500)
		{
			plist[curSelected].GetComponent<ParticleSystem>().enableEmission = false;
			curSelected = Random.Range(0, plist.Length);
			plist[curSelected].GetComponent<ParticleSystem>().enableEmission = true;
			timer = 0;
		}
		timer++;
	}
}
