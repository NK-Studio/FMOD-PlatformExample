using System;
using System.IO;
using GameplayIngredients;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class SaveData
{
    public float MasterVolume = 1.0f;
    public float BgmVolume = 0.7f;
    public float SfxVolume = 0.7f;
    public bool Pause = false;
}

[ManagerDefaultPrefab("SaveLoadManager")]
public class SaveLoadManager : Manager
{
    //저장할 세이브 데이터를 담은 클래스
    public SaveData SaveData = new SaveData();

    private void Awake()
    {
        //초기에 로드를 진행합니다.
        Load();

        //로드된 값을 할당합니다.
        Get<AudioManager>().setMasterVolume(SaveData.MasterVolume);
        Get<AudioManager>().setBGMVolume(SaveData.BgmVolume);
        Get<AudioManager>().setSFXVolume(SaveData.SfxVolume);
        Get<AudioManager>().setPause(SaveData.Pause);
    }

    /// <summary>
    /// 데이터를 저장합니다.
    /// </summary>
    public void Save()
    {
        //Json으로 변환하고 저장합니다.
        var json = JsonConvert.SerializeObject(SaveData);
        File.WriteAllText($"{Application.dataPath}/NKStudio.json", json);
    }


    /// <summary>
    /// 데이터를 로드합니다.
    /// </summary>
    /// <returns>불러오기 성공시 true를 반환</returns>
    public bool Load()
    {
        //해당 경로에 저장파일이 있는지 검색합니다.
        if (!File.Exists($"{Application.dataPath}/NKStudio.json")) return false;

        //저장파일이 있다면 게임의 실제 로직에 반영합니다.
        var json = File.ReadAllText($"{Application.dataPath}/NKStudio.json");
        SaveData = JsonConvert.DeserializeObject<SaveData>(json);

        return true;
    }

    /// <summary>
    /// 세이브 데이터를 가지고 있다면 true를 반환합니다.
    /// </summary>
    public bool isHasSaveData() => File.Exists($"{Application.dataPath}/NKStudio.json");
}
