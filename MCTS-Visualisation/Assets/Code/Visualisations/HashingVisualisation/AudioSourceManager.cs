using System.Collections.Generic;
using UnityEngine;

public class AudioSourceManager : MonoBehaviour {

	private Queue<AudioSource> sources;

	private const int MAX_SOURCES = 6;

	private void Start () {
		sources = new Queue<AudioSource>();

		for(int i = 0; i < MAX_SOURCES; i++)
		{
			//Create a new gameobject for the new source
			GameObject newSourceObject = new GameObject();
			newSourceObject.transform.parent = transform;
			newSourceObject.name = "AudioSource " + i;

			//Attach an audiosource to the new gameobject
			AudioSource newSource = newSourceObject.AddComponent<AudioSource>();

			//Set properties of new audiosource
			newSource.priority = 128;
			newSource.volume = 0.3f;
			newSource.pitch = 1;
			newSource.panStereo = 0;
			newSource.spatialBlend = 0;
			newSource.reverbZoneMix = 0;
			newSource.dopplerLevel = 1;
			newSource.spread = 0;
			newSource.rolloffMode = AudioRolloffMode.Logarithmic;
			newSource.minDistance = 1;
			newSource.maxDistance = 500;

			//Add the new audiosource to the queue of all audiosources
			sources.Enqueue(newSource);
		}
	}
	
	public AudioSource GetNextSource(Vector3 positon)
	{
		//Get the next source from the front of the queue and set its position
		AudioSource nextSource = sources.Dequeue();
		nextSource.transform.position = positon;

		//Add the source to the back of the queue and return a reference to it
		sources.Enqueue(nextSource);
		return nextSource;
	}
}
