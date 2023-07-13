using FMOD.Studio;
using FMODUnity;
using AutoManager;
using FMODPlus;
using NaughtyAttributes;
using UnityEngine;

namespace Managers
{
    [ManagerDefaultPrefab("AudioManager")]
    public class AudioManager : Manager
    {
        #region Public

        [BoxGroup("Audio Emitter")] public FMODAudioSource BgmAudioSource;

        [field: SerializeField, BoxGroup("Clip")]
        public RegisterEventClip RegisterEvent { get; private set; }

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
        public void PlayBGM() => BgmAudioSource.Play();

        /// <summary>
        /// Returns whether the background music is paused.
        /// </summary>
        /// <returns></returns>
        public bool IsPlayingBGM() => BgmAudioSource.IsPlaying();

        /// <summary>
        /// Stop the sound.
        /// </summary>
        /// <param name="fadeOut">true이면 페이드를 합니다.</param>
        public void StopBGM(bool fadeOut = false)
        {
            BgmAudioSource.AllowFadeout = fadeOut;
            BgmAudioSource.Stop();
        }

        /// <summary>
        /// Pause or resume playing the sound.
        /// </summary>
        /// <param name="pause">true면 정지하고, false면 다시 재생합니다.</param>
        public void SetPauseBGM(bool pause)
        {
            if (pause)
                BgmAudioSource.Pause();
            else
                BgmAudioSource.UnPause();
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
            BgmAudioSource.EventInstance.keyOff();
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

        private void PlayOneShot(FMOD.GUID guid, string parameterName, float parameterValue,
            Vector3 position = new Vector3())
        {
            var instance = RuntimeManager.CreateInstance(guid);
            instance.set3DAttributes(position.To3DAttributes());
            instance.setParameterByName(parameterName, parameterValue);
            instance.start();
            instance.release();
        }
    }
}