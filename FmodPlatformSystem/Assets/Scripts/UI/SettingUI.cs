using AutoManager;
using Managers;
using UnityEngine;

namespace UI
{
    public class SettingUI : MonoBehaviour
    {
        /// <summary>
        /// 세팅 씬으로 이동합니다.
        /// </summary>
        public void GotoSettings()
        {
            Manager.Get<GameManager>().GotoSettings();
        }
    }
}