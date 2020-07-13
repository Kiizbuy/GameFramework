using UnityEngine;
using UnityEngine.Audio;

namespace GameFramework.Settings
{
    public class SoundVolumeSettings : MonoBehaviour
    {
        private AudioMixer mainMixer;

        private readonly string mainMixerName = "GameMasterSound";
        private readonly string musicVolumeMixerName = "Music";
        private readonly string sfxVolumeMixerName = "SFX";
        private readonly string voicesVolumeMixerName = "Voices";
        private readonly float volumeMultiplier = 20f;

        private void Start()
        {
            mainMixer = Resources.Load<AudioMixer>(mainMixerName);
        }

        public void SetVolume(float normalizedMusicVolume = 1f, float normalizedSFXVolume = 1f, float normalizedVoiceVolume = 1f)
        {
            if (normalizedMusicVolume > 1f)
                normalizedMusicVolume = 1f;

            if (normalizedSFXVolume > 1f)
                normalizedSFXVolume = 1f;

            if (normalizedVoiceVolume > 1f)
                normalizedVoiceVolume = 1f;

            mainMixer.SetFloat(musicVolumeMixerName, Mathf.Log10(normalizedMusicVolume) * volumeMultiplier);
            mainMixer.SetFloat(sfxVolumeMixerName, Mathf.Log10(normalizedSFXVolume) * volumeMultiplier);
            mainMixer.SetFloat(voicesVolumeMixerName, Mathf.Log10(normalizedVoiceVolume) * volumeMultiplier); 
        }
    }
}

