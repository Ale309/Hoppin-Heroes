using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Mixer : MonoBehaviour
{
	[SerializeField] public Slider musicSlider;
	[SerializeField] public AudioMixer musicMixer;
	private float value;

	[SerializeField] public Slider FXSlider;
	[SerializeField] public AudioMixer FXMixer;
	private float value2;

	private void Start()
	{
		musicMixer.GetFloat("music",out value);
		musicSlider.value = value;

		FXMixer.GetFloat("fx",out value2);
		FXSlider.value = value2;
	}

	public void setVolumeMusic()
	{
		musicMixer.SetFloat("music", musicSlider.value);
	}

	public void setVolumeFX()
	{
		FXMixer.SetFloat("fx", FXSlider.value);
	}
}
