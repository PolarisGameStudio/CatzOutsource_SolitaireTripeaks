using Nightingale.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace Nightingale.Utilitys
{
	public class AudioUtility : MonoBehaviour
	{
		private static AudioUtility soundManager;

		private static AudioUtility musicManager;

		private float volume = 1f;

		private List<AudioSource> soundEffects = new List<AudioSource>();

		public static AudioUtility GetSound()
		{
			if (soundManager == null)
			{
				GameObject gameObject = new GameObject("Sound Manager");
				soundManager = gameObject.AddComponent<AudioUtility>();
				Object.DontDestroyOnLoad(gameObject);
			}
			return soundManager;
		}

		public static AudioUtility GetMusic()
		{
			if (musicManager == null)
			{
				GameObject gameObject = new GameObject("Music Manager");
				musicManager = gameObject.AddComponent<AudioUtility>();
				Object.DontDestroyOnLoad(gameObject);
			}
			return musicManager;
		}

		public void SetVolume(float volume)
		{
			this.volume = volume;
			AudioSource[] componentsInChildren = base.transform.GetComponentsInChildren<AudioSource>();
			AudioSource[] array = componentsInChildren;
			foreach (AudioSource audioSource in array)
			{
				audioSource.volume = volume;
			}
		}

		public AudioSource Play(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				return null;
			}
			if (Application.isPlaying)
			{
				AudioSource soundEffect = GetSoundEffect();
				soundEffect.clip = SingletonBehaviour<LoaderUtility>.Get().GetAsset<AudioClip>(fileName);
				soundEffect.loop = false;
				soundEffect.volume = volume;
				soundEffect.Play();
				return soundEffect;
			}
			return null;
		}

		public AudioSource PlayLoop(string fileName, int loopCount = -1)
		{
			if (loopCount == 0)
			{
				return null;
			}
			if (string.IsNullOrEmpty(fileName))
			{
				return null;
			}
			return PlayLoop(SingletonBehaviour<LoaderUtility>.Get().GetAsset<AudioClip>(fileName), loopCount);
		}

		public AudioSource PlayLoop(string fileName, float loopTime)
		{
			AudioClip asset = SingletonBehaviour<LoaderUtility>.Get().GetAsset<AudioClip>(fileName);
			return PlayLoop(asset, Mathf.CeilToInt(loopTime / asset.length));
		}

		public AudioSource PlayLoop(AudioClip clip, int loopCount = -1)
		{
			if (loopCount == 0)
			{
				return null;
			}
			if (Application.isPlaying)
			{
				AudioSource source = GetSoundEffect();
				source.clip = clip;
				source.loop = true;
				source.volume = volume;
				source.Play();
				if (loopCount >= 0)
				{
					source.gameObject.DelayDo(new WaitForSecondsRealtime(source.clip.length * (float)loopCount), delegate
					{
						source.Stop();
					});
				}
				return source;
			}
			return null;
		}

		private AudioSource GetSoundEffect()
		{
			AudioSource audioSource = null;
			if (soundEffects.Count > 3)
			{
				audioSource = soundEffects.Find((AudioSource e) => !e.loop && !e.isPlaying);
				if (audioSource != null)
				{
					audioSource.Stop();
					audioSource.gameObject.CancelDelay();
					soundEffects.Remove(audioSource);
				}
			}
			if (audioSource == null)
			{
				GameObject gameObject = new GameObject("Audio Source");
				gameObject.transform.SetParent(base.transform, worldPositionStays: false);
				audioSource = gameObject.AddComponent<AudioSource>();
			}
			soundEffects.Add(audioSource);
			return audioSource;
		}
	}
}
