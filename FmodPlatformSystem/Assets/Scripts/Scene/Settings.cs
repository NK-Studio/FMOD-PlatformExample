using Data;
using FMODUnity;
using GameplayIngredients;
using Managers;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Scenes
{
    public class Settings : MonoBehaviour
    {
        #region MASTER

        [BoxGroup("MASTER")] public Slider MasterSlider;

        [BoxGroup("MASTER")] public TMP_Text MasterText;

        #endregion

        #region BGM

        [BoxGroup("BGM")] public Slider BGMSlider;

        [BoxGroup("BGM")] public TMP_Text BGMText;

        #endregion

        #region SFX

        [BoxGroup("SFX")] public Slider SFXSlider;

        [BoxGroup("SFX")] public TMP_Text SFXText;

        [BoxGroup("SFX")] public EventReference ExampleSFX;

        #endregion

        #region Toggle

        [BoxGroup("Toggle")] public Toggle StageToggle;

        [BoxGroup("Toggle")] public Toggle PauseToggle;

        #endregion

        private void Start()
        {
            //저장된 볼륨의 값을 가져옵니다.
            MasterSlider.value = DataManager.MasterVolume;
            BGMSlider.value = DataManager.BGMVolume;
            SFXSlider.value = DataManager.SFXVolume;
            PauseToggle.isOn = DataManager.Pause;

            MasterText.text = $"{MasterSlider.value * 100f:N0} %";
            BGMText.text = $"{BGMSlider.value * 100f:N0} %";
            SFXText.text = $"{SFXSlider.value * 100f:N0} %";
        }

        /// <summary>
        /// 마스터 볼륨을 업데이트 합니다.
        /// </summary>
        public void UpdateMasterVolume()
        {
            //화면에 Master 볼륨 퍼센트를 렌더링합니다.
            MasterText.text = $"{MasterSlider.value * 100f:N0} %";

            //Master 슬라이더의 값을 BGM 볼륨에 반영합니다.
            Manager.Get<AudioManager>().SetMasterVolume(MasterSlider.value);

            //저장될 Master 볼륨 데이터를 갱신 합니다.
            DataManager.MasterVolume = MasterSlider.value;
        }

        /// <summary>
        /// BGM 볼륨을 업데이트합니다.
        /// </summary>
        public void UpdateBGMVolume()
        {
            //화면에 BGM 볼륨 퍼센트를 렌더링합니다.
            BGMText.text = $"{BGMSlider.value * 100f:##} %";

            //BGM 슬라이더의 값을 실제 BGM 볼륨에 반영합니다.
            Manager.Get<AudioManager>().SetBGMVolume(BGMSlider.value);

            //실제 저장될 BGM 볼륨 데이터를 갱신 합니다.
            DataManager.BGMVolume = BGMSlider.value;
        }

        /// <summary>
        /// 효과음 볼륨을 업데이트합니다.
        /// </summary>
        public void UpdateSFXVolume()
        {
            //화면에 SFX 볼륨 퍼센트를 렌더링합니다.
            SFXText.text = $"{SFXSlider.value * 100f:##} %";

            //SFX 슬라이더의 값을 실제 BGM 볼륨에 반영합니다.
            Manager.Get<AudioManager>().SetSFXVolume(SFXSlider.value);

            //실제 저장될 SFX 볼륨 데이터를 갱신 합니다.
            DataManager.SFXVolume = SFXSlider.value;
        }

        /// <summary>
        /// 효과음을 재생합니다.
        /// </summary>
        public void PlaySFX()
        {
            Manager.Get<AudioManager>().PlayOneShot(ExampleSFX);
        }

        /// <summary>
        /// 음악 스타일을 변경합니다.
        /// </summary>
        public void ChangeMusicStyle()
        {
            //토글 값을 가져옵니다.
            bool toggle = StageToggle.isOn;

            switch (toggle)
            {
                //오케스트라 음악으로 전환합니다.
                case true:
                    Manager.Get<AudioManager>().ChangeBit(BitType.Orchestra);
                    break;
                //8비트 음악으로 전환합니다.
                case false:
                    Manager.Get<AudioManager>().ChangeBit(BitType.BIT8);
                    break;
            }
        }

        /// <summary>
        /// 특정 씬으로 이동합니다.
        /// </summary>
        /// <param name="sceneName"></param>
        public void GotoStage(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// BGM의 일시 정지 상태를 변경합니다.
        /// </summary>
        public void ChangePause()
        {
            //토글 값을 가져옵니다.
            bool toggle = PauseToggle.isOn;

            //오케스트라 음악으로 전환합니다.
            Manager.Get<AudioManager>().SetPauseBGM(toggle);

            //Pause값을 저장 데이터에 갱신합니다.
            DataManager.Pause = toggle;
        }
    }
}