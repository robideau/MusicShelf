using UnityEngine;
using System.Collections;

public class ParticleControl : MonoBehaviour {
	
	private ParticleSystem particleSys;
	
	void Start()
	{
		particleSys = GetComponent<ParticleSystem> ();

		particleSys.loop = true;
		particleSys.enableEmission = true;
	}
	
	void Update() 
	{
		if ((Time.frameCount % 10) == 0) {
			particleSys.Emit(1);
		}
		
	}
	
}