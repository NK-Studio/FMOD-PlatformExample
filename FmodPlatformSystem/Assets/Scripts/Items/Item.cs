using Data;
using FMODUnity;
using AutoManager;
using Managers;
using UnityEngine;
using SceneUtility = Utility.SceneUtility;

namespace Items
{
    public class Item : MonoBehaviour
    {
        //아이템 타입
        public ItemType type;

        private AudioManager AudioManager => Manager.Get<AudioManager>();

        public FMODParameterSender parameterSender;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                switch (type)
                {
                    case ItemType.Poison:

                        int style = 0;

                        if (SceneUtility.CompareSceneByName("Demo01"))
                            style = 1;
                        else if (SceneUtility.CompareSceneByName("Demo02"))
                            style = 2;

                        switch (style)
                        {
                            case 1:
                                // 데모 01에서는 파라미터를 통해 사운드의 끝을 표현합니다.
                                //AudioManager.BgmAudioSource.SetParameter("Death", 0); old
                                parameterSender.SendValue();
                                break;
                            case 2:
                                // 데모 02에서는 사운드를 페이드하여 정지합니다.
                                AudioManager.StopBGM(true);
                                break;
                        }

                        break;
                    case ItemType.Orchestra:
                        parameterSender.SendValue();
                        // AudioManager.BgmAudioSource.SetParameter("Stage", 0f); Old
                        break;
                    case ItemType.Bit8:
                        parameterSender.SendValue();
                        // AudioManager.BgmAudioSource.SetParameter("Stage", 1f); old
                        break;
                }
            }
        }
    }
}