using UnityEngine;

namespace Player
{
    public class Mover : MonoBehaviour
    {
        [Tooltip("땅 체크 마스크")] public LayerMask GroundMask;

        [Tooltip("땅 체크시 서클 사이즈")] public float Radius = 0.025f;
    
        //박스콜라이더 참조 변수
        private BoxCollider2D _bodyCollider;

        //리지드바디2D 참조 변수
        private Rigidbody2D _rigidbody;
    
        public bool IsGrounded { get; private set; }
    
        private void Awake()
        {
            _bodyCollider = GetComponent<BoxCollider2D>();
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        public bool IsGround()
        {
            //플레이어의 콜라이더에서 CenterBottom의 위치를 담습니다.
            Vector2 point = Vector2.zero;
            Bounds bounds = _bodyCollider.bounds;
            point.x = bounds.center.x;
            point.y = bounds.min.y;

            IsGrounded = Physics2D.OverlapCircle(point, Radius, GroundMask);
        
            //땅 체크
            return IsGrounded;
        }
    
        public void SetGravityScale(float scale)
        {
            _rigidbody.gravityScale = scale;
        }
    
        public Vector2 GetVelocity()
        {
            return _rigidbody.velocity;
        }

        public void SetVelocity(Vector2 velocity)
        {
            _rigidbody.velocity = velocity;
        }

        public void AddForce(Vector2 settingsJumpPower, ForceMode2D impulse)
        {
            _rigidbody.AddForce(settingsJumpPower, impulse);
        }
    }
}