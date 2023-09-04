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

        [BoxGroup("Audio Emitter")] public FMODAudioSource BGMAudioSource;
        [BoxGroup("Audio Emitter")] public FMODAudioSource AMBAudioSource;

        [BoxGroup("Bus")] public string[] Buses;

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

        public void ChangeParameter(AudioType audioType, ParamRef paramRef)
        {
            switch (audioType)
            {
                case AudioType.AMB:
                    AMBAudioSource.SetParameter(paramRef.Name,paramRef.Value);
                    break;
                case AudioType.BGM:
                    BGMAudioSource.ApplyParameter(paramRefs);
                    break;
                case AudioType.SFX:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(audioType), audioType, null);
            }
        }
        
        public void ChangeParameter(AudioType audioType, ParamRef[] paramRefs)
        {
            switch (audioType)
            {
                case AudioType.AMB:
                    AMBAudioSource.ApplyParameter(paramRefs);
                    break;
                case AudioType.BGM:
                    BGMAudioSource.ApplyParameter(paramRefs);
                    break;
                case AudioType.SFX:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(audioType), audioType, null);
            }
        }
        
        /// <summary>
        /// 다른 사운드로 변경합니다.
        /// </summary>
        /// <param name="audioType">오디오 타입</param>
        /// <param name="clip">변경할 이벤트 레퍼런스 클립</param>
        public void ChangeClip(AudioType audioType,EventReference clip)
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
        /// 다른 사운드로 변경합니다.
        /// </summary>
        /// <param name="audioType">오디오 타입</param>
        /// <param name="clip">변경할 이벤트 레퍼런스 클립</param>
        public void ChangeClip(AudioType audioType,EventReference clip,ParamRef paramRef)
        {
            switch (audioType)
            {
                case AudioType.AMB:
                    AMBAudioSource.SetParameter(paramRef.Name,paramRef.Value);
                    AMBAudioSource.Clip = clip;
                    break;
                case AudioType.BGM:
                    BGMAudioSource.SetParameter(paramRef.Name,paramRef.Value);
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
        /// 다른 사운드로 변경합니다.
        /// </summary>
        /// <param name="audioType">오디오 타입</param>
        /// <param name="clip">변경할 이벤트 레퍼런스 클립</param>
        public void ChangeClip(AudioType audioType,EventReference clip,ParamRef[] paramRefs)
        {
            switch (audioType)
            {
                case AudioType.AMB:
                    AMBAudioSource.ApplyParameter(paramRefs);
                    AMBAudioSource.Clip = clip;
                    break;
                case AudioType.BGM:
                    BGMAudioSource.ApplyParameter(paramRefs);
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
                    Debug.LogWarning("SFX는 아직 지원하지 않습니다.");
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
        /// <param name="value">0~1사이의 값, 0이면 뮤트됩니다.</param>
        public void SetMasterVolume(float value) => _masterBus.setVolume(value);

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