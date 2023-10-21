﻿using System;
using Data;
using AutoManager;
using FMODPlus;
using Managers;
using Settings;
using UnityEngine;

namespace Player
{
    public class PlayerSystem : CharacterController<PlayerCharacterSettings>
    {
        public GameObject DieAnimation;

        private AudioManager AudioManager => Manager.Get<AudioManager>();
        private GameManager GameManager => Manager.Get<GameManager>();

        protected override void Start()
        {
            base.Start();

            OnJump += () => AudioManager.PlayOneShot(Settings.JumpClip, transform.position);
            OnLand += () => AudioManager.PlayOneShot(Settings.LandClip, transform.position);
        }

        protected override void Update()
        {
            base.Update();

            if (!GameManager.IsFeverMode)
                _moveSpeed = Settings.MoveSpeed;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            //아이템을 먹었을 때를 처리합니다.
            if (other.CompareTag("Item"))
                OnEnterItem(other);
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
                ItemType itemType = item.Kind;

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
            if (other.TryGetComponent(out CommandSender eventCommandSender))
            {
                // 피버 BGM 처리
                eventCommandSender.SendCommand();
                
                //이동 속도를 빠르게 합니다.
                _moveSpeed = Settings.FeverMoveSpeed;

                //피버 모드를 발동합니다.
                GameManager.SetFeverMode(true);

                //아이템 삭제
                Destroy(other);
            }
        }

        private void OnPoison(GameObject other)
        {
            if (other.TryGetComponent(out CommandSender eventCommandSender))
            {
                // 사망 BGM 처리
                eventCommandSender.SendCommand();

                //죽는 이펙트 플레이어 생성
                Instantiate(DieAnimation, transform.position, Quaternion.identity);

                //아이템 삭제
                Destroy(other);

                //자신도 삭제
                Destroy(gameObject);
            }
        }
    }
}