using Data;
using FMOD.Studio;
using FMODUnity;
using FMODUtility;
using GameplayIngredients;
using NaughtyAttributes;
using UnityEngine;

namespace Managers
{
    [ManagerDefaultPrefab("AudioManager")]
    public class AudioManager : Manager
    {
        #region Public

        [BoxGroup("Audio Emitter")] public FMODAudioSource BgmAudioSource;

        [BoxGroup("Music")] public AudioPathByString Clip;

        [BoxGroup("Bank")] public string[] Bank;

        #endregion

        #region Private

        private Bus _masterBus;
        private Bus _bgmBus;
        private Bus _sfxBus;

        #endregion

        private void Awake()
        {
            //볼륨에 대한 Bus를 가져옵니다.
            _masterBus = RuntimeManager.GetBus(Bank[0]);
            _bgmBus = RuntimeManager.GetBus(Bank[1]);
            _sfxBus = RuntimeManager.GetBus(Bank[2]);
        }
        
        /// <summary>
        /// 사운드를 재생하게 해줍니다.
        /// </summary>
        public void PlayBGM() => BgmAudioSource.Play();
        
        /// <summary>
        /// 배경음악이 일시정지 되었는지 반환합니다.
        /// </summary>
        /// <returns></returns>
        public bool IsPlayingBGM() => BgmAudioSource.IsPlaying();

        /// <summary>
        /// 사운드를 정지합니다.
        /// </summary>
        /// <param name="fadeOut">true이면 페이드를 합니다.</param>
        public void StopBGM(bool fadeOut = false)
        {
            BgmAudioSource.AllowFadeout = fadeOut;
            BgmAudioSource.Stop();
        }

        /// <summary>
        /// 사운드를 일시정지하거나, 다시 재생합니다.
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
        /// Master의 볼륨을 조절합니다.
        /// </summary>
        /// <param name="value">0~1사이의 값, 0이면 뮤트됩니다.</param>
        public void SetMasterVolume(float value) => _masterBus.setVolume(value);

        /// <summary>
        /// BGM의 볼륨을 조절합니다.
        /// </summary>
        /// <param name="value">0~1사이의 값, 0이면 뮤트됩니다.</param>
        public void SetBGMVolume(float value) => _bgmBus.setVolume(value);

        /// <summary>
        /// SFX의 볼륨을 조절합니다.
        /// </summary>
        /// <param name="value">0~1사이의 값, 0이면 뮤트됩니다.</param>
        public void SetSFXVolume(float value) => _sfxBus.setVolume(value);

        /// <summary>
        /// 인스턴스를 내부에서 만들어서 효과음을 재생하고, 즉시 파괴합니다.
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
        public static void PlayOneShot(EventReference path, string parameterName, float parameterValue,
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
        /// 파라미터를 호환하고 인스턴스를 내부에서 만들어서 효과음을 재생하고, 즉시 파괴합니다.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="position"></param>
        public static void PlayOneShot(string path, string parameterName, float parameterValue,
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

        private static void PlayOneShot(FMOD.GUID guid, string parameterName, float parameterValue,
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