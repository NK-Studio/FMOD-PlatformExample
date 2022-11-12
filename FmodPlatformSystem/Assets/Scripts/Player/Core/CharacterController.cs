using Settings;
using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    public class CharacterController<T> : MonoBehaviour where T : CharacterSettings
    {
        [SerializeField] protected T Settings;

        protected Mover _mover;

        protected InputHandler _inputHandler;

        protected CharacterAnimationHandler CharacterAnimationHandler;

        //이동 제어를 하는 변수
        protected bool _isStop;

        //좌우 이동에 대한 변수
        protected float _horizontal;

        protected float _moveSpeed;

        private bool isAir;

        protected Vector2 _velocity = Vector2.zero;

        protected UnityAction OnJump;
        protected UnityAction OnLand;

        protected virtual void Awake()
        {
            //초기화
            _mover = GetComponent<Mover>();
            _inputHandler = GetComponent<InputHandler>();
            CharacterAnimationHandler = GetComponent<CharacterAnimationHandler>();
        }

        protected virtual void Start()
        {
            _moveSpeed = Settings.MoveSpeed;
        }

        protected virtual void Update()
        {
            //방향키 입력
            _horizontal = _inputHandler.Horizontal;
        }

        protected virtual void FixedUpdate()
        {
            //이동
            UpdateMove(_horizontal);

            //좌우 전환
            UpdateFlip(_horizontal);

            //점프, 낙하 중력 변환
            UpdateGravityScale();

            //점프
            UpdateJump();

            CharacterAnimationHandler.OnUpdateMoveAnimation(_velocity, _mover.IsGrounded);
        }

        #region playerUpadate

        protected virtual void UpdateGravityScale()
        {
            _mover.SetGravityScale(Settings.Gravity);

            Vector2 velocity = _mover.GetVelocity();

            //위로 상승하면 jump, 하강이면 fall 중력으로 변경한다.
            if (velocity.y > 0f)
                _mover.SetGravityScale(Settings.JumpGravity);
            else if (velocity.y < 0f)
                _mover.SetGravityScale(Settings.FallGravity);

            bool isGround = _mover.IsGround();

            if (!isGround)
            {
                _inputHandler.SetJump(false);
                isAir = true;
            }
            else
            {
                if (isAir)
                {
                    OnLand?.Invoke();
                    isAir = false;
                }
            }

            float vertical = isGround ? 0f : velocity.y;
            _velocity.y = vertical;
        }

        protected virtual void UpdateFlip(float axis)
        {
            //h가 0.0f가 아니라면, (h != 0f 이렇게 조건하는 것은 효율이 좋지 않습니다.)
            if (!Mathf.Approximately(axis, 0f))
                transform.localScale = new Vector2(axis, 1f);
        }

        protected virtual void UpdateMove(float axis)
        {
            //이동 처리
            Vector2 velocity = _mover.GetVelocity();

            //이동 가능시에만 값 변경
            if (!_isStop)
                velocity.x = axis * (_moveSpeed * 100) * Time.deltaTime;
            else
                velocity.x = 0f;

            //적용
            _mover.SetVelocity(velocity);
            _velocity.x = axis * _moveSpeed;
        }

        protected virtual void UpdateJump()
        {
            if (_inputHandler.Jump)
            {
                //Y의 힘을 초기화 함.
                Vector2 resultVelocity = _mover.GetVelocity();
                resultVelocity.y = 0f;
                _mover.SetVelocity(resultVelocity);

                //실제 액터를 위로 점프 시킴
                _mover.AddForce(Vector2.up * Settings.JumpPower, ForceMode2D.Impulse);
                CharacterAnimationHandler.OnJumpAnimation();
                OnJump?.Invoke();
            }
        }

        #endregion
    }
}