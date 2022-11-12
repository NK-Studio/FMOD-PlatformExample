using UnityEngine;

namespace Player
{
    public class CharacterAnimationHandler : MonoBehaviour
    {
        private Animator _animator;
        private const float SmoothingFactor = 40f;
        private Vector3 _oldMovementVelocity = Vector3.zero;
    
        #region AnimationID

        private static readonly int IDHorizontal = Animator.StringToHash("Horizontal");
        private static readonly int IDVertical = Animator.StringToHash("Vertical");
        private static readonly int IDIsGround = Animator.StringToHash("IsGround");
        private static readonly int IDOnJump = Animator.StringToHash("OnJump");

        #endregion

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
        }

        public void OnUpdateMoveAnimation(Vector2 velocity, bool isGrounded)
        {
            Vector3 horizontalVelocity = new Vector3(Mathf.Abs(velocity.x), 0, 0);

            horizontalVelocity =
                Vector3.Lerp(_oldMovementVelocity, horizontalVelocity, SmoothingFactor * Time.deltaTime);
            _oldMovementVelocity = horizontalVelocity;
        
            _animator.SetFloat(IDHorizontal, horizontalVelocity.magnitude);
        
            _animator.SetFloat(IDVertical,velocity.y);
        
            _animator.SetBool(IDIsGround, isGrounded);
        }

        public void OnJumpAnimation()
        {
            _animator.SetTrigger(IDOnJump);
        }
    }
}