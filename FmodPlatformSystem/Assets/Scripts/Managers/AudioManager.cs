using System;
using System.Collections.Generic;
using AutoManager;
using FMOD;
using FMOD.Studio;
using FMODPlus;
using FMODUnity;
using NaughtyAttributes;
using UnityEngine;
using AudioType = FMODPlus.AudioType;
using Debug = UnityEngine.Debug;
namespace Managers
{
    [ManagerDefaultPrefab("AudioManager")]
    public class AudioManager : Manager
    {
        #region Public
        [BoxGroup("Audio Emitter")]
        public FMODAudioSource BGMAudioSource;
        [BoxGroup("Audio Emitter")]
        public FMODAudioSource AMBAudioSource;

        [BoxGroup("Bus")]
        public string[] Buses;
        #endregion

        #region Private
        private Bus _masterBus;
        private Bus _bgmBus;
        private Bus _sfxBus;
        #endregion

        private void Awake()
        {
            // Get the Bus for the volume.
            _masterBus = RuntimeManager.GetBus(Buses[0]);
            _bgmBus = RuntimeManager.GetBus(Buses[1]);
            _sfxBus = RuntimeManager.GetBus(Buses[2]);
        }

        /// <summary>
        /// Let the sound play.
        /// </summary>
        /// <param name="audioType">Audio type to play</param>
        public void Play(AudioType audioType)
        {
            switch (audioType)
            {
                case AudioType.AMB:
                    AMBAudioSource.Play();
                    break;
                case AudioType.BGM:
                    BGMAudioSource.Play();
                    break;
                case AudioType.SFX:
                    Debug.LogWarning("SFX는 아직 지원하지 않습니다.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(audioType), audioType, null);
            }
        }

        /// <summary>
        /// Let the sound play.
        /// </summary>
        /// <param name="audioType">Audio type to play</param>
        /// <param name="clip">Audio type to play</param>
        public void Play(AudioType audioType, EventReference clip)
        {
            switch (audioType)
            {
                case AudioType.AMB:
                    AMBAudioSource.Clip = clip;
                    AMBAudioSource.Play();
                    break;
                case AudioType.BGM:
                    BGMAudioSource.Clip = clip;
                    BGMAudioSource.Play();
                    break;
                case AudioType.SFX:
                    Debug.LogWarning("SFX는 아직 지원하지 않습니다.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(audioType), audioType, null);
            }
        }

        /// <summary>
        /// Let the sound play.
        /// </summary>
        /// <param name="audioType">Audio type to play</param>
        /// <param name="clip">Audio type to play</param>
        /// <param name="paramName">Parameter name to change</param>
        /// <param name="value">value to change</param>
        public void Play(AudioType audioType, EventReference clip, string paramName, float value, bool ignoreseekspeed = false)
        {
            switch (audioType)
            {
                case AudioType.AMB:
                    AMBAudioSource.SetParameter(paramName, value, ignoreseekspeed);
                    AMBAudioSource.Clip = clip;
                    AMBAudioSource.Play();
                    break;
                case AudioType.BGM:
                    BGMAudioSource.SetParameter(paramName, value, ignoreseekspeed);
                    BGMAudioSource.Clip = clip;
                    BGMAudioSource.Play();
                    break;
                case AudioType.SFX:
                    Debug.LogWarning("SFX는 아직 지원하지 않습니다.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(audioType), audioType, null);
            }
        }

        /// <summary>
        /// Let the sound play.
        /// </summary>
        /// <param name="audioType">Audio type to play</param>
        /// <param name="clip">Audio type to play</param>
        /// <param name="paramRef">Parameter reference to change</param>
        public void Play(AudioType audioType, EventReference clip, ParamRef paramRef)
        {
            switch (audioType)
            {
                case AudioType.AMB:
                    AMBAudioSource.SetParameter(paramRef);
                    AMBAudioSource.Clip = clip;
                    AMBAudioSource.Play();
                    break;
                case AudioType.BGM:
                    BGMAudioSource.SetParameter(paramRef);
                    BGMAudioSource.Clip = clip;
                    BGMAudioSource.Play();
                    break;
                case AudioType.SFX:
                    Debug.LogWarning("SFX는 아직 지원하지 않습니다.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(audioType), audioType, null);
            }
        }

        /// <summary>
        /// Let the sound play.
        /// </summary>
        /// <param name="audioType">Audio type to play</param>
        /// <param name="clip">Audio type to play</param>
        /// <param name="paramRefs">Parameter references to change</param>
        public void Play(AudioType audioType, EventReference clip, ParamRef[] paramRefs)
        {
            switch (audioType)
            {
                case AudioType.AMB:
                    AMBAudioSource.SetParameter(paramRefs);
                    AMBAudioSource.Clip = clip;
                    AMBAudioSource.Play();
                    break;
                case AudioType.BGM:
                    BGMAudioSource.SetParameter(paramRefs);
                    BGMAudioSource.Clip = clip;
                    BGMAudioSource.Play();
                    break;
                case AudioType.SFX:
                    Debug.LogWarning("SFX는 아직 지원하지 않습니다.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(audioType), audioType, null);
            }
        }

        /// <summary>
        /// Change parameters.
        /// </summary>
        /// <param name="audioType">Audio type to change</param>
        /// <param name="paramName">Parameter name to change</param>
        /// <param name="value">value to change</param>
        /// <param name="ignoreseekspeed"></param>
        public void ChangeParameter(AudioType audioType, string paramName, float value, bool ignoreseekspeed = false)
        {
            switch (audioType)
            {
                case AudioType.AMB:
                    AMBAudioSource.SetParameter(paramName, value, ignoreseekspeed);
                    break;
                case AudioType.BGM:
                    BGMAudioSource.SetParameter(paramName, value, ignoreseekspeed);
                    break;
                case AudioType.SFX:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(audioType), audioType, null);
            }
        }

        /// <summary>
        /// Change parameters.
        /// </summary>
        /// <param name="audioType">Audio type to change</param>
        /// <param name="paramRef">Parameter reference to change</param>
        public void ChangeParameter(AudioType audioType, ParamRef paramRef)
        {
            switch (audioType)
            {
                case AudioType.AMB:
                    AMBAudioSource.SetParameter(paramRef.Name, paramRef.Value);
                    break;
                case AudioType.BGM:
                    BGMAudioSource.SetParameter(paramRef.Name, paramRef.Value);
                    break;
                case AudioType.SFX:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(audioType), audioType, null);
            }
        }

        /// <summary>
        /// Change parameters.
        /// </summary>
        /// <param name="audioType">Audio type to change</param>
        /// <param name="paramRefs">Parameter references to change</param>
        public void ChangeParameter(AudioType audioType, ParamRef[] paramRefs)
        {
            switch (audioType)
            {
                case AudioType.AMB:
                    AMBAudioSource.SetParameter(paramRefs);
                    break;
                case AudioType.BGM:
                    BGMAudioSource.SetParameter(paramRefs);
                    break;
                case AudioType.SFX:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(audioType), audioType, null);
            }
        }

        /// <summary>
        /// Change the sound clip.
        /// </summary>
        /// <param name="audioType">Audio type to change</param>
        /// <param name="clip">Event reference clip to change</param>
        public void ChangeClip(AudioType audioType, EventReference clip)
        {
            switch (audioType)
            {
                case AudioType.AMB:
                    AMBAudioSource.Clip = clip;
                    break;
                case AudioType.BGM:
                    BGMAudioSource.Clip = clip;
                    break;
                case AudioType.SFX:
                    Debug.LogWarning("SFX는 아직 지원하지 않습니다.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(audioType), audioType, null);
            }

        }

        /// <summary>
        /// Returns whether the background music is paused.
        /// </summary>
        /// <param name="audioType">Audio type to check if playing</param>
        /// <returns></returns>
        public bool IsPlaying(AudioType audioType)
        {
            switch (audioType)
            {
                case AudioType.AMB:
                    return AMBAudioSource.IsPlaying();
                case AudioType.BGM:
                    return BGMAudioSource.IsPlaying();
                case AudioType.SFX:
                    Debug.LogWarning("SFX는 아직 지원하지 않습니다.");
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(audioType), audioType, null);
            }
        }

        /// <summary>
        /// Stop the sound.
        /// </summary>
        /// <param name="audioType">Audio type to stop</param>
        /// <param name="fadeOut">If true, it fades.</param>
        public void Stop(AudioType audioType, bool fadeOut = false)
        {
            switch (audioType)
            {
                case AudioType.AMB:
                    AMBAudioSource.AllowFadeout = fadeOut;
                    AMBAudioSource.Stop();
                    break;
                case AudioType.BGM:
                    BGMAudioSource.AllowFadeout = fadeOut;
                    BGMAudioSource.Stop();
                    break;
                case AudioType.SFX:
                    Debug.LogWarning("SFX는 아직 지원하지 않습니다.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(audioType), audioType, null);
            }
        }

        /// <summary>
        /// Pause or resume playing the sound.
        /// </summary>
        /// <param name="audioType">Audio type to stop</param>
        /// <param name="pause">If true, it stops, if false, it plays again.</param>
        public void SetPause(AudioType audioType, bool pause)
        {
            switch (audioType)
            {
                case AudioType.AMB:
                    if (pause)
                        AMBAudioSource.Pause();
                    else
                        AMBAudioSource.UnPause();
                    break;
                case AudioType.BGM:
                    if (pause)
                        BGMAudioSource.Pause();
                    else
                        BGMAudioSource.UnPause();
                    break;
                case AudioType.SFX:
                    Debug.LogWarning("SFX는 아직 지원하지 않습니다.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(audioType), audioType, null);
            }
        }

        /// <summary>
        /// Adjust the Master's volume.
        /// </summary>
        /// <param name="value">A value between 0 and 1, where 0 is muted.</param>
        public void SetMasterVolume(float value) => _masterBus.setVolume(value);

        /// <summary>
        /// Adjust the volume of the BGM.
        /// </summary>
        /// <param name="value">A value between 0 and 1, where 0 is muted.</param>
        public void SetBGMVolume(float value) => _bgmBus.setVolume(value);

        /// <summary>
        /// Adjusts the volume of SFX.
        /// </summary>
        /// <param name="value">A value between 0 and 1, where 0 is muted.</param>
        public void SetSFXVolume(float value) => _sfxBus.setVolume(value);

        /// <summary>
        /// Call Key Off when using Sustain Key Point.
        /// </summary>
        public void KeyOff()
        {
            BGMAudioSource.EventInstance.keyOff();
        }

        /// <summary>
        /// Call Key Off when using Sustain Key Point.
        /// </summary>
        public void TriggerCue()
        {
            KeyOff();
        }

        /// <summary>
        /// Create an instance in-place, play a sound effect, and destroy it immediately.
        /// </summary>
        /// <param name="eventReference">Sound effect path to play</param>
        /// <param name="position">Play a sound at that location.</param>
        public void PlayOneShot(EventReference eventReference, Vector3 position = default)
        {
            RuntimeManager.PlayOneShot(eventReference, position);
        }

        /// <summary>
        /// Parameter compatible, create instance internally, play sound effect, destroy immediately.
        /// </summary>
        /// <param name="eventReference">Event reference to play the sound</param>
        /// <param name="parameterName">Parameter name to change</param>
        /// <param name="parameterValue">Value to change</param>
        /// <param name="position">Play a sound at that location.</param>
        public void PlayOneShot(EventReference eventReference, string parameterName, float parameterValue,
            Vector3 position = new Vector3())
        {
            try
            {
                PlayOneShot(eventReference.Guid, parameterName, parameterValue, position);
            }
            catch (EventNotFoundException)
            {
                RuntimeUtils.DebugLogWarning("[FMOD] Event not found: " + eventReference);
            }
        }

        /// <summary>
        /// Parameter compatible, create instance internally, play sound effect, destroy immediately.
        /// </summary>
        /// <param name="path">Sound effect path to play</param>
        /// <param name="parameterName">Parameter name to change</param>
        /// <param name="parameterValue">Value to change</param>
        /// <param name="position">Play a sound at that location.</param>
        public void PlayOneShot(string path, string parameterName, float parameterValue,
            Vector3 position = new Vector3())
        {
            try
            {
                PlayOneShot(RuntimeManager.PathToGUID(path), parameterName, parameterValue, position);
            }
            catch (EventNotFoundException)
            {
                RuntimeUtils.DebugLogWarning("[FMOD] Event not found: " + path);
            }
        }

        /// <summary>
        /// Parameter compatible, create instance internally, play sound effect, destroy immediately.
        /// </summary>
        /// <param name="eventReference">Event reference to play the sound</param>
        /// <param name="parameters">Parameters to change</param>
        /// <param name="volumeScale">Volume level when playing sound</param>
        /// <param name="position">Play a sound at that location.</param>
        public void PlayOneShot(EventReference eventReference, IReadOnlyList<ParamRef> parameters,
            float volumeScale = 1.0f, Vector3 position = new())
        {
            try
            {
                PlayOneShot(eventReference.Guid, parameters, volumeScale, position);
            }
            catch (EventNotFoundException)
            {
                RuntimeUtils.DebugLogWarning("[FMOD] Event not found: " + eventReference);
            }
        }

        /// <summary>
        /// Parameter compatible, create instance internally, play sound effect, destroy immediately.
        /// </summary>
        /// <param name="path">Sound effect path to play</param>
        /// <param name="parameters">Parameters to change</param>
        /// <param name="volumeScale">Volume level when playing sound</param>
        /// <param name="position">Play a sound at that location.</param>
        public void PlayOneShot(string path, IReadOnlyList<ParamRef> parameters,
            float volumeScale = 1.0f, Vector3 position = new())
        {
            try
            {
                PlayOneShot(RuntimeManager.PathToGUID(path), parameters, volumeScale, position);
            }
            catch (EventNotFoundException)
            {
                RuntimeUtils.DebugLogWarning("[FMOD] Event not found: " + path);
            }
        }

        private void PlayOneShot(GUID guid, string parameterName, float parameterValue,
            Vector3 position = new Vector3())
        {
            var instance = RuntimeManager.CreateInstance(guid);
            instance.set3DAttributes(position.To3DAttributes());
            instance.setParameterByName(parameterName, parameterValue);
            instance.start();
            instance.release();
        }

        private void PlayOneShot(GUID guid, IReadOnlyList<ParamRef> parameters,
            float volumeScale = 1.0f, Vector3 position = new())
        {
            EventInstance instance = RuntimeManager.CreateInstance(guid);
            instance.set3DAttributes(position.To3DAttributes());

            int count = parameters.Count;

            for (int i = 0; i < count; i++)
                instance.setParameterByName(parameters[i].Name, parameters[i].Value);

            instance.setVolume(volumeScale);
            instance.start();
            instance.release();
        }
    }
}
