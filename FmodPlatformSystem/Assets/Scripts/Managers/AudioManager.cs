using Data;
using FMOD.Studio;
using FMODUnity;
using GameplayIngredients;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    [ManagerDefaultPrefab("AudioManager")]
    public class AudioManager : Manager
    {
        #region Public

        [BoxGroup("Audio Emitter")] public StudioEventEmitter BGMEmitter;

        [BoxGroup("Audio Emitter")] public StudioEventEmitter SFXEmitter;

        [BoxGroup("Music")] public EventReference[] bgmClip;

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

        private void Start()
        {
            //씬이 전환될 때, 자동으로 BGM이 바뀌도록 합니다. 
            SceneManager.activeSceneChanged += SceneManagerOnactiveSceneChanged;
            SceneManagerOnactiveSceneChanged(SceneManager.GetSceneByName("NULL"), SceneManager.GetActiveScene());
        }

        //씬이 변경되면 감지해서 알맞게 사운드를 재생한다.
        private void SceneManagerOnactiveSceneChanged(Scene arg0, Scene arg1)
        {
            //현재 씬 이름이 : Demo1이라면,
            if (CompareScene(arg1, "Demo01"))
                ChangeBGM(Stage.Demo1);

            //현재 씬 이름이 : Demo2이라면,
            else if (CompareScene(arg1, "Demo02"))
                ChangeBGM(Stage.Demo2);

            //현재 씬 이름이 : Setting이라면, 데모01 BGM을 재생합니다.
            else if (CompareScene(arg1, "Setting"))
                ChangeBGM(Stage.Demo2);

            //사운드를 재생한다.
            PlayBGM();

            //일시정지 상태가 아니라면 재생한다.
            bool isPause = DataManager.Pause;
            SetPauseBGM(isPause);
        }

        public bool CompareScene(Scene scene, string sceneName)
        {
            return scene.name.Equals(sceneName);
        }

        /// <summary>
        /// BGM 사운드 이벤트를 변경하는데 사용합니다.
        /// </summary>
        public void ChangeBGM(Stage stage)
        {
            int index = (int)stage;
            BGMEmitter.ChangeEvent(bgmClip[index]);
        }
        
        /// <summary>
        /// 효과음을 변경합니다.
        /// </summary>
        /// <param name="path">재생할 효과음 경로</param>
        public void ChangeSFX(EventReference path) => SFXEmitter.ChangeEvent(path);

        /// <summary>
        /// 사운드를 재생하게 해줍니다.
        /// </summary>
        public void PlayBGM() => BGMEmitter.Play();

        /// <summary>
        /// 효과음을 재생하게 해줍니다.
        /// </summary>
        public void PlaySFX() => SFXEmitter.Play();
        
        /// <summary>
        /// 배경음악이 일시정지 되었는지 반환합니다.
        /// </summary>
        /// <returns></returns>
        public bool IsPlayingBGM() => BGMEmitter.IsPlaying();

        /// <summary>
        /// 사운드를 정지합니다.
        /// </summary>
        /// <param name="fadeOut">true이면 페이드를 합니다.</param>
        public void StopBGM(bool fadeOut = false)
        {
            BGMEmitter.AllowFadeout = fadeOut;
            BGMEmitter.Stop();
        }

        /// <summary>
        /// 사운드를 일시정지하거나, 다시 재생합니다.
        /// </summary>
        /// <param name="pause">true면 정지하고, false면 다시 재생합니다.</param>
        public void SetPauseBGM(bool pause) => BGMEmitter.SetPause(pause);

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
        public void PlayOneShot(EventReference path,Vector3 position = default)
        {
            RuntimeManager.PlayOneShot(path, position);
        }
        
        #region Parameter

        public void Reset()
        {
            BGMEmitter.SetParameter("Fast", 0);
            BGMEmitter.SetParameter("Death", 1);
            BGMEmitter.SetParameter("Stage", 0);
        }

        /// <summary>
        /// 베경 음악을 종료합니다.
        /// </summary>
        public void TriggerDeath()
        {
            bool isDemo01Scene = Manager.Get<GameManager>().CompareSceneName("Demo01");
            bool isDemo02Scene = Manager.Get<GameManager>().CompareSceneName("Demo02");

            //데모 01에서는 파라미터를 통해 사운드의 끝을 표현합니다.
            if (isDemo01Scene)
                BGMEmitter.SetParameter("Death", 0);

            //데모 02에서는 사운드를 페이드하여 정지합니다.
            if (isDemo02Scene)
                StopBGM(true);
        }

        /// <summary>
        /// 음악의 속도를 변경합니다.
        /// </summary>
        public void ChangeBPM(BPMType bpmType) => BGMEmitter.SetParameter("Fast", bpmType == BPMType.Fast ? 1 : 0);

        /// <summary>
        /// 음악의 비트를 변경합니다.
        /// </summary>
        public void ChangeBit(BitType bitType) => BGMEmitter.SetParameter("Stage", bitType == BitType.BIT8 ? 1 : 0);

        #endregion
    }
}