using System;
using FMOD.Studio;
using GameplayIngredients;
using UnityEngine;
using UnityEngine.SceneManagement;

[ManagerDefaultPrefab("GameManager")]
public class GameManager : Manager
{
    private bool isFeverMode;
    public float FeverTime { get; private set; }

    public float HP { get; private set; } = 100;

    /// <summary>
    /// 피버모드를 발동할지 말지 처리합니다.
    /// </summary>
    public void FeverMode(bool Fever)
    {
        isFeverMode = Fever;

        if (Fever)
        {
            FeverTime = 10.0f;
            Get<AudioManager>().bgm.setParameterByName("Fast", 1);
        }
        else
        {
            Get<AudioManager>().bgm.setParameterByName("Fast", 0);
            FeverTime = 0f;
        }
    }
    
    public void DiePlayer()
    {
        HP = 0f;
        Get<AudioManager>().bgm.setParameterByName("Death", 0f);

        if (SceneManager.GetActiveScene().name.Equals("Demo2"))
            Get<AudioManager>().Stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private void Update()
    {
        //피버 모드를 처리합니다.
        updateFever();

        //캐릭터가 죽은 후 다시 재 시작을 처리합니다.
        reStart();
        
        //ESC를 눌러서 게임 종료를 처리합니다.
        GameEnd();
    }

    private void updateFever()
    {
        if (isFeverMode)
            if (FeverTime > 0f) //피버 모드면 시간을 계속 감속시킵니다.
                FeverTime -= Time.deltaTime;
            else
            {
                //플레이어 속도를 원래대로 되돌립니다.
                Messager.Send("NormalSpeed");

                //음악의 속도 파라미터를 다시 0으로 되돌립니다.
                Get<AudioManager>().bgm.setParameterByName("Fast", 0);

                //피버모드 초기화
                FeverTime = 0f;
                isFeverMode = false;
            }
    }

    private void reStart()
    {
        if (Input.GetKeyDown(KeyCode.R))
            if (Mathf.Approximately(HP, 0f))
            {
                //사운드의 상태를 가져옵니다.
                Get<AudioManager>().bgm.getPlaybackState(out var playbackState);
                
                //서스테인 상태가 아니라면 : return
                if (playbackState != PLAYBACK_STATE.SUSTAINING && playbackState != PLAYBACK_STATE.STOPPED) return;

                //초기화
                HP = 100f;
                Get<AudioManager>().bgm.setParameterByName("Fast", 0);
                Get<AudioManager>().bgm.setParameterByName("Death", 1);
                Get<AudioManager>().bgm.setParameterByName("Stage", 0);

                //IMMEDIATE를 통해 바로 사운드를 Stop해야하며, ALLOWFADEOUT를 사용시 볼륨 페이드가 발생하여
                //자칫 사운드 겹칩이 생길 수 있습니다.
                //ALLOWFADEOUT를 사용할 경우 충분히 사운드가 모두 재생이 완료된 후 Change함수와 Play함수를 실행하면 됩니다.
                Get<AudioManager>().Stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

                //죽은 위치에 따라 씬을 전환합니다.
                Debug.Log(SceneManager.GetActiveScene().name);
                SceneManager.LoadScene(SceneManager.GetActiveScene().name.Equals("Demo1") ? "Demo2" : "Demo1");
            }
    }

    private void GameEnd()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
}
