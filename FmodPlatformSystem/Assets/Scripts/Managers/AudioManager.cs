using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using AutoManager;
using FMODPlus;
using NaughtyAttributes;
using UnityEngine;
using AudioType = FMODPlus.AudioType;

namespace Managers
{
    [ManagerDefaultPrefab("AudioManager")]
    public class AudioManager : Manager
    {
        #region Public

        [BoxGroup("Audio Emitter")]
        public FMODAudioSource AMBAudioSource;
        
        [BoxGroup("Audio Emitter")]
        public FMODAudioSource BGMAudioSource;
        
        [BoxGroup("Audio Emitter")] 
        public FMODAudioSource SFXAudioSource;

        [BoxGroup("Bus")] public string[] Buses;

        public static AudioManager Instance => Get<AudioManager>();

        #endregion

        #region Private

        private Bus _masterBus;
        private Bus _ambBus;
        private Bus _bgmBus;
        private Bus _sfxBus;

        #endregion

        private void Awake()
        {
            // Get the Bus for the volume.
            _masterBus = RuntimeManager.GetBus(Buses[0]);
            _ambBus = RuntimeManager.GetBus(Buses[1]);
            _bgmBus = RuntimeManager.GetBus(Buses[2]);
            _sfxBus = RuntimeManager.GetBus(Buses[3]);
        }

        /// <summary>
        /// Let the sound play.
        /// </summary>
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
                    SFXAudioSource.Play();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(audioType), audioType, null);
            }
        }

        /// <summary>
        /// Returns whether the background music is paused.
        /// </summary>
        /// <param name="audioType"></param>
        /// <returns></returns>
        public bool IsPlaying(AudioType audioType)
        {
            switch (audioType)
            {
                case AudioType.AMB:
                    return AMBAudioSource.isPlaying;
                case AudioType.BGM:
                    return BGMAudioSource.isPlaying;
                case AudioType.SFX:
                    return SFXAudioSource.isPlaying;
                default:
                    throw new ArgumentOutOfRangeException(nameof(audioType), audioType, null);
            }
        }

        /// <summary>
        /// Stop the sound.
        /// </summary>
        /// <param name="audioType"></param>
        /// <param name="fadeOut">true이면 페이드를 합니다.</param>
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
                    SFXAudioSource.AllowFadeout = fadeOut;
                    SFXAudioSource.Stop();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(audioType), audioType, null);
            }
        }

        /// <summary>
        /// Pause or resume playing the sound.
        /// </summary>
        /// <param name="pause">true면 정지하고, false면 다시 재생합니다.</param>
        public void SetPause(AudioType audioType, bool pause)
        {
            if (pause)
            {
                switch (audioType)
                {
                    case AudioType.AMB:
                        AMBAudioSource.Pause();
                        break;
                    case AudioType.BGM:
                        BGMAudioSource.Pause();
                        break;
                    case AudioType.SFX:
                        SFXAudioSource.Pause();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(audioType), audioType, null);
                }
            }
            else
            {
                switch (audioType)
                {
                    case AudioType.AMB:
                        AMBAudioSource.UnPause();
                        break;
                    case AudioType.BGM:
                        BGMAudioSource.UnPause();
                        break;
                    case AudioType.SFX:
                        SFXAudioSource.UnPause();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(audioType), audioType, null);
                }
            }
        }

        /// <summary>
        /// Adjust the Master's volume.
        /// </summary>
        /// <param name="value">0~1사이의 값, 0이면 뮤트됩니다.</param>
        public void SetMasterVolume(float value) => _masterBus.setVolume(value);

        /// <summary>
        /// Adjust the volume of the AMB.
        /// </summary>
        /// <param name="value">0~1사이의 값, 0이면 뮤트됩니다.</param>
        public void SetAMBVolume(float value) => _ambBus.setVolume(value);

        /// <summary>
        /// Adjust the volume of the BGM.
        /// </summary>
        /// <param name="value">0~1사이의 값, 0이면 뮤트됩니다.</param>
        public void SetBGMVolume(float value) => _bgmBus.setVolume(value);

        /// <summary>
        /// Adjusts the volume of SFX.
        /// </summary>
        /// <param name="value">0~1사이의 값, 0이면 뮤트됩니다.</param>
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
        /// <param name="path">재생할 효과음 경로</param>
        /// <param name="position">해당 위치에서 소리를 재생합니다.</param>
        public void PlayOneShot(EventReference path, Vector3 position = default)
        {
            RuntimeManager.PlayOneShot(path, position);
        }

        /// <summary>
        /// 파라미터를 호환하고 인스턴스를 내부에서 만들어서 효과음을 재생하고, 즉시 파괴합니다.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="position"></param>
        public void PlayOneShot(EventReference path, string parameterName, float parameterValue,
            Vector3 position = new Vector3())
        {
            try
            {
                PlayOneShot(path.Guid, parameterName, parameterValue, position);
            }
            catch (EventNotFoundException)
            {
                RuntimeUtils.DebugLogWarning("[FMOD] Event not found: " + path);
            }
        }

        /// <summary>
        /// Parameter compatible, create instance internally, play sound effect, destroy immediately.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="position"></param>
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
        /// <param name="eventReference"></param>
        /// <param name="parameters"></param>
        /// <param name="volumeScale"></param>
        /// <param name="position"></param>
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
        /// <param name="path"></param>
        /// <param name="parameters"></param>
        /// <param name="volumeScale"></param>
        /// <param name="position"></param>
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

        private void PlayOneShot(FMOD.GUID guid, string parameterName, float parameterValue,
            Vector3 position = new Vector3())
        {
            var instance = RuntimeManager.CreateInstance(guid);
            instance.set3DAttributes(position.To3DAttributes());
            instance.setParameterByName(parameterName, parameterValue);
            instance.start();
            instance.release();
        }

        private void PlayOneShot(FMOD.GUID guid, IReadOnlyList<ParamRef> parameters,
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