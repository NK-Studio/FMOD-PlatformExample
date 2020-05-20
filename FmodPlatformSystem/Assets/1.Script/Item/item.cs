using UnityEngine;

public class item : MonoBehaviour
{
    public enum ItemType
    {
        None,
        FEVER,
        POISON
    }
    
    private BoxCollider2D col;
    private Animator animator;

    //아이템 타입
    public ItemType itemType;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        var Check = col.IsTouchingLayers(LayerMask.GetMask("Ground"));

        if (animator)
            animator.speed = Check ? 1f : 0f;
    }
}
