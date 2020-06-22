/*-------------------------------------------------------------------*
|  Title:			Sound
|
|  Author:			Max Atkinson
| 
|  Description:		A sound object which contains all the details of
					a specific named audio clip and audio source.
*-------------------------------------------------------------------*/

using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound 
{
    public string name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;
    public bool loop;
	public bool randomizePitch = false;	//If set to true, each time the sound plays its pitch will be slightly up or down

    [HideInInspector]
    public AudioSource source;

}
