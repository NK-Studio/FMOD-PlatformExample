using System;
using FMODUnity;
using GameplayIngredients;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Demo3System : MonoBehaviour
{
    [Header("MASTER")]
    public Slider MasterVolumSlider;

    public Text MasterPercentText;

    [Header("BGM")]
    public Slider BgmVolumSlider;

    public Text BgmPercentText;

    [Header("SFX")]
    public Slider SfxVolumSlider;

    public Text SfxPercentText;

    [Header("Parm")]
    public Toggle StageChangeToggle;

    [Header("Pause")]
    public Toggle PauseChangeToggle;

    private void Start()
    {
        //저장된 볼륨의 값을 가져옵니다.
        MasterVolumSlider.value = Manager.Get<SaveLoadManager>().SaveData.MasterVolume;
        BgmVolumSlider.value = Manager.Get<SaveLoadManager>().SaveData.BgmVolume;
        SfxVolumSlider.value = Manager.Get<SaveLoadManager>().SaveData.SfxVolume;
        PauseChangeToggle.isOn = Manager.Get<SaveLoadManager>().SaveData.Pause;

        //퍼센트 숫자 렌더링
        float value;
        MasterPercentText.text = (value = MasterVolumSlider.value).Equals(0f) ? "0%" : $"{value * 100f:##} %";
        BgmPercentText.text = (value = BgmVolumSlider.value).Equals(0f) ? "0%" : $"{value * 100f:##} %";
        SfxPercentText.text = (value = SfxVolumSlider.value).Equals(0f) ? "0%" : $"{value * 100f:##} %";
    }

    public void ChangeMasterVolum()
    {
        float value;

        //Master 슬라이더의 값을 실제 BGM 볼륨에 반영합니다.
        Manager.Get<AudioManager>().setMasterVolume(MasterVolumSlider.value);

        //실제 저장될 Master 볼륨 데이터를 갱신 합니다.
        Manager.Get<SaveLoadManager>().SaveData.MasterVolume = MasterVolumSlider.value;

        //화면에 Master 볼륨 퍼센트를 렌더링합니다.
        MasterPercentText.text = (value = MasterVolumSlider.value).Equals(0f) ? "0%" : $"{value * 100f:##} %";
    }

    public void ChangeBGMVolum()
    {
        float value;

        //BGM 슬라이더의 값을 실제 BGM 볼륨에 반영합니다.
        Manager.Get<AudioManager>().setBGMVolume(BgmVolumSlider.value);

        //실제 저장될 BGM 볼륨 데이터를 갱신 합니다.
        Manager.Get<SaveLoadManager>().SaveData.BgmVolume = BgmVolumSlider.value;

        //화면에 BGM 볼륨 퍼센트를 렌더링합니다.
        BgmPercentText.text = (value = BgmVolumSlider.value).Equals(0f) ? "0%" : $"{value * 100f:##} %";
    }

    public void ChangeSFXVolum()
    {
        float value;

        //SFX 슬라이더의 값을 실제 BGM 볼륨에 반영합니다.
        Manager.Get<AudioManager>().setSFXVolume(SfxVolumSlider.value);

        //실제 저장될 SFX 볼륨 데이터를 갱신 합니다.
        Manager.Get<SaveLoadManager>().SaveData.SfxVolume = SfxVolumSlider.value;

        //화면에 SFX 볼륨 퍼센트를 렌더링합니다.
        SfxPercentText.text = (value = SfxVolumSlider.value).Equals(0f) ? "0%" : $"{value * 100f:##} %";
    }

    /// <summary>
    /// 효과음을 재생합니다.
    /// </summary>
    public void ListenSFX() => Manager.Get<AudioManager>().PlayOneShot("event:/Jump");

    /// <summary>
    /// 현재 게임 데이터를 저장합니다.
    /// </summary>
    public void SoundSave() => Manager.Get<SaveLoadManager>().Save();

    public void StageBit()
    {
        //토글 값을 가져옵니다.
        var toggle = StageChangeToggle.isOn;

        //오케스트라 음악으로 전환합니다.
        if (toggle)
            Manager.Get<AudioManager>().ChangeBit(BitType.ORCHESTRA);

        //8비트 음악으로 전환합니다.
        if (!toggle)
            Manager.Get<AudioManager>().ChangeBit(BitType.BIT_8);
    }

    /// <summary>
    /// 데모 1 or 2로 이동합니다. 
    /// </summary>
    public void gotoStage(int value)
    {
        //기존에 재생되고 있던 사운드를 릴리즈 합니다.
        Manager.Get<AudioManager>().notifyRelease();

        //파라미터에 따라 데모 1로 이동할지 2로 이동할지 처리합니다.
        var Stage = value == 1 ? 0 : 1;
        SceneManager.LoadScene(Stage);
    }

    public void PauseToggle()
    {
        //토글 값을 가져옵니다.
        var toggle = PauseChangeToggle.isOn;

        //오케스트라 음악으로 전환합니다.
        Manager.Get<AudioManager>().setPause(toggle);

        //Pause값을 저장 데이터에 갱신합니다.
        Manager.Get<SaveLoadManager>().SaveData.Pause = toggle;

        //저장
        SoundSave();
    }
}
