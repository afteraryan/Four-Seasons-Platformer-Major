using UnityEngine;

public class CharacterAnimationController
{
    private bool isRunning = false;
    private bool isIdle = false;
    private Animator animator;
    
    private CharacterMovementState movementState = CharacterMovementState.Idle;
    
    public void SetAnimator(Animator animator)
    {
        this.animator = animator;
    }
    
    public void SetAnimationState(CharacterMovementState newState)
    {
        if (animator == null)
        {
            Debug.LogError("Animator is null");
            return;
        }
        movementState = newState;

        switch (movementState)
        {
            case CharacterMovementState.Idle:
                isIdle = true;
                isRunning = false;
                break;
            case CharacterMovementState.Running:
                isIdle = false;
                isRunning = true;
                break;
            default:
                isIdle = true;
                isRunning = false;
                break;
        }
        
        animator.SetBool("isIdle", isIdle);
        animator.SetBool("isRunning", isRunning);
        
    }
    
}
