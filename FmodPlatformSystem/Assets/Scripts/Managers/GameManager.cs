using Data;
using GameplayIngredients;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Managers
{
    [ManagerDefaultPrefab("GameManager")]
    public class GameManager : Manager
    {
        public bool IsFeverMode { get; private set; }

        public float FeverTime { get; private set; }

        [BoxGroup("재시작 키 입력")]
        public InputAction ReStartKey;

        private void OnEnable()
        {
            ReStartKey.Enable();
        }

        private void OnDisable()
        {
            ReStartKey.Disable();
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
            {
                FeverTime = 10.0f; 
                Manager.Get<AudioManager>().ChangeBPM(BPMType.Fast);
            }
            else
            {
                FeverTime = 0f;
                IsFeverMode = false;
                Manager.Get<AudioManager>().ChangeBPM(BPMType.Normal);
            }
        }

        /// <summary>
        /// 현재 씬이 인자와 같은 씬인지 확인합니다.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public bool CompareSceneName(string sceneName)
        {
            return SceneManager.GetActiveScene().name.Equals(sceneName);
        }

        private void UpdateFever()
        {
            if (IsFeverMode)//피버모드가 실행되었고,
                if (FeverTime > 0f) //피버 시간이 0보다 크면,
                    FeverTime -= Time.deltaTime;
                else
                    //음악의 속도 파라미터를 다시 0으로 되돌립니다.
                    SetFeverMode(false);
        }

        private void OnReStart()
        {
            if (ReStartKey.WasPressedThisFrame())
            {
                //사운드의 상태를 가져옵니다.
                bool isPlay = Manager.Get<AudioManager>().IsPlayingBGM();

                //사운드가 재생중인 상태라면 return합니다.
                if (isPlay) return;

                //초기화
                Manager.Get<AudioManager>().Reset();
                
                //죽은 위치에 따라 씬을 전환합니다.
                string nextScene = CompareSceneName("Demo01") ? "Demo02" : "Demo01";
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