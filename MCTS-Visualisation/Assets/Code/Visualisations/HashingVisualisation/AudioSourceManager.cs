using System.Collections.Generic;
using UnityEngine;

public class AudioSourceManager : MonoBehaviour {

	private static AudioSourceManager manager;
	private static AudioClip cNote;

	private Queue<AudioSource> sources;

	private const int MAX_SOURCES = 12;

	private void Awake () {
		//Check to see if the singleton reference is already set
		if(manager != null)
		{
			throw new System.Exception("Singleton reference for AudioSourceManager already exists...");
		}

		//Set the singleton reference
		manager = this;

		//Initialise the sources queue and load CNote from Resources
		sources = new Queue<AudioSource>();
		cNote = Resources.Load<AudioClip>("CNote");

		for(int i = 0; i < MAX_SOURCES; i++)
		{
			//Create a new gameobject for the new source
			GameObject newSourceObject = new GameObject();
			newSourceObject.transform.parent = transform;
			newSourceObject.name = "AudioSource " + i;

			//Attach an audiosource to the new gameobject
			AudioSource newSource = newSourceObject.AddComponent<AudioSource>();

			//Set properties of new audiosource
			newSource.clip = cNote;
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
	
	public static AudioSource GetNextSource(Vector3 positon)
	{
		//Get the next source from the front of the queue and set its position
		AudioSource nextSource = manager.sources.Dequeue();
		nextSource.transform.position = positon;

		//Add the source to the back of the queue and return a reference to it
		manager.sources.Enqueue(nextSource);
		return nextSource;
	}
}
