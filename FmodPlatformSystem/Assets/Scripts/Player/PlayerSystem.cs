using System;
using Data;
using GameplayIngredients;
using Items;
using Managers;
using Settings;
using UnityEngine;

namespace Player
{
    public class PlayerSystem : CharacterController<PlayerCharacterSettings>
    {
        public GameObject DieAnimation;

        protected override void Start()
        {
            base.Start();

            //OnJump += () => Manager.Get<AudioManager>().PlayOneShot(Settings.JumpClip,transform.position);
            //OnLand += () => Manager.Get<AudioManager>().PlayOneShot(Settings.LandClip,transform.position);

            OnJump += () =>
            {
                Manager.Get<AudioManager>().ChangeSFX(Settings.JumpClip);
                Manager.Get<AudioManager>().PlaySFX();
            };
            OnLand += () =>
            {
                Manager.Get<AudioManager>().ChangeSFX(Settings.LandClip);
                Manager.Get<AudioManager>().PlaySFX();
            };
        }

        protected override void Update()
        {
            base.Update();

            if (!Manager.Get<GameManager>().IsFeverMode) 
                OnNormalMode();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            //아이템을 먹었을 때를 처리합니다.
            if (other.CompareTag("Item")) 
                OnEnterItem(other);

            //오케스트라 음악으로 전환합니다.
            if (other.CompareTag("Orchestra"))
                Manager.Get<AudioManager>().ChangeBit(BitType.Orchestra);

            //8비트 음악으로 전환합니다.
            if (other.CompareTag("8Bit"))
                Manager.Get<AudioManager>().ChangeBit(BitType.BIT8);
        }

        /// <summary>
        /// 아이템에 닿았을 때를 처리합니다.
        /// </summary>
        /// <param name="other"></param>
        private void OnEnterItem(Collider2D other)
        {
            Item item = other.GetComponent<Item>();

            if (item)
            {
                ItemType itemType = item.type;

                switch (itemType)
                {
                    case ItemType.Fever:
                        OnFastMove(other.gameObject);
                        break;
                    case ItemType.Poison:
                        OnPoison(other.gameObject);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        private void OnFastMove(GameObject other)
        {
            //이동 속도를 빠르게 합니다.
            _moveSpeed = Settings.FeverMoveSpeed;
        
            //피버 모드를 발동합니다.
            Manager.Get<GameManager>().SetFeverMode(true);

            //아이템 삭제
            Destroy(other);
        }

        private void OnPoison(GameObject other)
        {
            //사망 사운드를 재생합니다.
            Manager.Get<AudioManager>().TriggerDeath();

            //죽는 이펙트 플레이어 생성
            Instantiate(DieAnimation, transform.position, Quaternion.identity);
            
            //아이템 삭제
            Destroy(other);
            
            //자신도 삭제
            Destroy(gameObject);
        }
    

        private void OnNormalMode() =>
            _moveSpeed = Settings.MoveSpeed;
    }
}