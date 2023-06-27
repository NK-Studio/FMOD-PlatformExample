using System;
using GameplayIngredients;
using Managers;
using UnityEngine;

namespace Player
{
    public class DieAnimation : MonoBehaviour
    {
        private Rigidbody2D rig;

        private void Awake() => 
            rig = GetComponent<Rigidbody2D>();

        private void Start() => 
            rig.AddForce(Vector2.up * 400f);

        private void Update()
        {
            //밖으로 나갔다면 삭제한다.
            if (transform.position.y < -20f) Destroy(gameObject);
        }
    }
}
