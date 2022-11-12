namespace Managers
{
    //로컬 서버나 클라우드 서버에 저장되지 않고, 게임 플레이 동안만 살아있는 휘발성 데이터 베이스
    public static class DataManager
    {
        //저장할 세이브 데이터를 담은 클래스
        public static float MasterVolume = 1.0f;
        public static float BGMVolume = 0.7f;
        public static float SFXVolume = 0.7f;
        public static bool Pause;
    }
}
