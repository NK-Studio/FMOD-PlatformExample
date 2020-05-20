using System;
using FMOD.Studio;
using FMODUnity;
using GameplayIngredients;
using UnityEngine.SceneManagement;

public enum BitType
{
    BIT_8,
    ORCHESTRA
}

public enum Stage
{
    Demo1,
    Demo2,
    Demo3,
}

[ManagerDefaultPrefab("AudioManager")]
public class AudioManager : Manager
{
    #region Public

    public EventInstance bgm;

    [EventRef]
    public string[] bgmClip;
    
    #endregion

    #region Private

    private Bus MasterBus;
    private Bus BgmBus;
    private Bus SfxBus;

    #endregion

    private void Awake()
    {
        //볼륨에 대한 Bus를 가져옵니다.
        MasterBus = RuntimeManager.GetBus("bus:/Master");
        BgmBus = RuntimeManager.GetBus("bus:/Master/BGM");
        SfxBus = RuntimeManager.GetBus("bus:/Master/SFX");
    }

    private void Start()
    {
        //씬이 전환될 때, 자동으로 BGM이 바뀌도록 합니다. 
        SceneManager.activeSceneChanged += SceneManagerOnactiveSceneChanged;
        SceneManagerOnactiveSceneChanged(SceneManager.GetSceneByName("NULL"), SceneManager.GetActiveScene());
    }

    private void SceneManagerOnactiveSceneChanged(Scene arg0, Scene arg1)
    {
        if (arg1.name.Equals(Stage.Demo1.ToString())) //현재 씬 이름이 : Demo1라면,
        {
            Change((int) Stage.Demo1);
            Play();
        }
        else if (arg1.name.Equals(Stage.Demo2.ToString())) //현재 씬 이름이 : Demo2라면,
        {
            Change((int) Stage.Demo2);
            Play();
        }
        else if (arg1.name.Equals(Stage.Demo3.ToString())) //현재 씬 이름이 : Demo2라면,
        {
            Change((int) Stage.Demo2);
            Play();
        }
    }

    /// <summary>
    /// BGM 사운드 이벤트를 변경하는데 사용합니다.
    /// </summary>
    public void Change(int index)
    {
        //인스턴스 데이터가 들어있는 경우 릴리즈 해준다.
        notifyRelease();

        //인스턴스 데이터가 해제가 완료되었을 경우
        bgm = RuntimeManager.CreateInstance(bgmClip[index]);
        bgm.setPaused(Get<SaveLoadManager>().SaveData.Pause);
    }

    /// <summary>
    /// 사운드를 재생하게 해줍니다.
    /// </summary>
    public void Play() => bgm.start();

    /// <summary>
    /// 사운드를 정지합니다.
    /// </summary>
    public void Stop(FMOD.Studio.STOP_MODE SM) => bgm.stop(SM);

    /// <summary>
    /// 사운드를 일시정지하거나, 다시 재생합니다.
    /// </summary>
    /// <param name="pause">true면 정지하고, false면 다시 재생합니다.</param>
    public void setPause(bool pause) => bgm.setPaused(pause);

    /// <summary>
    /// BGM을 바꾸려면 먼저 기존에 생성된 인스턴스가 릴리즈 되어야 합니다.
    /// </summary>
    public void notifyRelease()
    {
        if (bgm.isValid())
        {      
            Stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            bgm.release();
            bgm.clearHandle();
        }
    }

    /// <summary>
    /// Master의 볼륨을 조절합니다.
    /// </summary>
    /// <param name="Value">0~1사이의 값, 0이면 뮤트됩니다.</param>
    public void setMasterVolume(float Value) => MasterBus.setVolume(Value);

    /// <summary>
    /// BGM의 볼륨을 조절합니다.
    /// </summary>
    /// <param name="Value">0~1사이의 값, 0이면 뮤트됩니다.</param>
    public void setBGMVolume(float Value) => BgmBus.setVolume(Value);

    /// <summary>
    /// SFX의 볼륨을 조절합니다.
    /// </summary>
    /// <param name="Value">0~1사이의 값, 0이면 뮤트됩니다.</param>
    public void setSFXVolume(float Value) => SfxBus.setVolume(Value);

    /// <summary>
    /// 효과음을 재생하게 해줍니다.
    /// </summary>
    /// <param name="path">재생할 효과음 경로</param>
    public void PlayOneShot(string path)
    {
        //효과음을 재생할 인스턴스를 생성합니다.
        var sfx = RuntimeManager.CreateInstance(path);
        
        //재생
        sfx.start();
        
        //릴리즈
        sfx.release();
    }
    
    #region Demo

    /// <summary>
    /// 음악의 비트를 변경합니다.
    /// </summary>
    public void ChangeBit(BitType bitType) =>
        Get<AudioManager>().bgm.setParameterByName("Stage", bitType == BitType.BIT_8 ? 1 : 0);

    #endregion
}
