using GameplayIngredients;
using NaughtyAttributes;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using SceneUtility = NKStudio.Utility.SceneUtility;

namespace Managers
{
    [ManagerDefaultPrefab("GameManager")]
    public class GameManager : Manager
    {
        private BoolReactiveProperty isFeverModeObservable = new();

        public bool IsFeverMode
        {
            get => isFeverModeObservable.Value;
            private set => isFeverModeObservable.Value = value;
        }

        public float FeverTime { get; private set; }

        [BoxGroup("재시작 키 입력")] public InputAction ReStartKey;

        private AudioManager AudioManager => Get<AudioManager>();
        
        private void OnEnable()
        {
            ReStartKey.Enable();
        }

        private void OnDisable()
        {
            ReStartKey.Disable();
        }

        private void Start()
        {
            // Start Fever
            isFeverModeObservable.Where(isMode => isMode)
                .Subscribe(_=> AudioManager.BgmAudioSource.SetParameter("Fast", 1f))
                .AddTo(this);
            
            // End Fever
            isFeverModeObservable.Where(isMode => !isMode)
                .Subscribe(_=> AudioManager.BgmAudioSource.SetParameter("Fast", 0f))
                .AddTo(this);
        }

        private void Update()
        {
            //피버 모드를 처리합니다.
            UpdateFever();

            //캐릭터가 죽은 후 다시 재 시작을 처리합니다.
            OnReStart();

            //ESC를 눌러서 게임 종료를 처리합니다.
            OnInputGameEnd();
        }

        /// <summary>
        /// 피버모드를 설정합니다.
        /// </summary>
        /// <param name="active">active가 true이면 피버를 실행하고, false이면 피버를 종료합니다.</param>
        public void SetFeverMode(bool active)
        {
            IsFeverMode = active;

            if (active)
                FeverTime = 10.0f;
            else
            {
                FeverTime = 0f;
                IsFeverMode = false;
            }
        }

        private void UpdateFever()
        {
            if (IsFeverMode) // 피버모드가 실행되었고,
                if (FeverTime > 0f) // 피버 시간이 0보다 크면,
                    FeverTime -= Time.deltaTime;
                else
                    // 피버 모드를 다시 원래대로 돌립니다.
                    SetFeverMode(false);
        }

        private void OnReStart()
        {
            if (ReStartKey.WasPressedThisFrame())
            {
                //사운드의 상태를 가져옵니다.
                bool isPlay = AudioManager.IsPlayingBGM();

                //사운드가 재생중인 상태라면 return합니다.
                if (isPlay) return;

                //죽은 위치에 따라 씬을 전환합니다.
                string nextScene = SceneUtility.CompareSceneByName("Demo01") ? "Demo02" : "Demo01";
                SceneManager.LoadScene(nextScene);
            }
        }

        /// <summary>
        /// 세팅 씬으로 이동합니다.
        /// </summary>
        public void GotoSettings()
        {
            //세팅으로 이동합니다.
            SceneManager.LoadScene("Setting");
        }

        private void OnInputGameEnd()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }
    }
}