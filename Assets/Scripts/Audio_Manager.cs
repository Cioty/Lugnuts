/*-------------------------------------------------------------------*
|  Title:			AudioManager
|
|  Author:			Max Atkinson / Seth Johnston
| 
|  Description:		Contains an array of sounds to be played statically
					from any other script.
*-------------------------------------------------------------------*/

using UnityEngine.Audio;
using UnityEngine;
using System;

public class Audio_Manager : MonoBehaviour
{
	public static Audio_Manager instance;	//Store a static instance of itself

	public Sound[] sounds;	//An array of sounds which get set in the inspector
	
    void Awake()
    {
		instance = this;    //Set the instance to itself

		foreach (Sound s in instance.sounds)					//For all the sounds,
        {
            s.source = gameObject.AddComponent<AudioSource>();	//Set its audio source to a new source on this object
            s.source.clip = s.clip;								//Set the clip of the audio source

            s.source.volume = s.volume;
            s.source.loop = s.loop;
        }
    }

	//Plays a sound with the given name
	public static void Play (string name)
    {
		Sound s = Array.Find(instance.sounds, sound => sound.name == name); //Find the sound with the correct name
																			// warning message if it cant find the right sounds 
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		if (s.randomizePitch)	//If the sound is set to randomize pitch,
			s.source.pitch = UnityEngine.Random.Range(0.85f, 1.15f);	//Move the pitch slightly up or down

		s.source.Play();
    }

    //Plays a sound with the given name
    public static void PlayDelayed(string name, float delay)
    {
        Sound s = Array.Find(instance.sounds, sound => sound.name == name); //Find the sound with the correct name
                                                                            // warning message if it cant find the right sounds 
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        if (s.randomizePitch)   //If the sound is set to randomize pitch,
            s.source.pitch = UnityEngine.Random.Range(0.85f, 1.15f);    //Move the pitch slightly up or down

        s.source.PlayDelayed(delay);
    }


    //Stops a sound with a given name 
    public static void Stop(string name)
	{
       Sound s = Array.Find(instance.sounds, sound => sound.name == name);	//Find the sound with the correct name
       s.source.Stop();
	}

    
}
