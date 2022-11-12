using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class InputHandler : MonoBehaviour
    {
        public float Horizontal { get; private set; }
        public bool Jump { get; private set; }
    
        public void InputMove(InputAction.CallbackContext context)
        {
            Horizontal = context.ReadValue<float>();
        }
    
        public void InputJump(InputAction.CallbackContext context)
        {
            Jump = context.action.IsPressed();
        }

        public void SetJump(bool value)
        {
            Jump = value;
        }
    }
}
