using UnityEngine;

public class CharacterAnimationController
{
    private bool isRunning = false;
    private bool isIdle = false;
    private Animator animator;
    
    private CharacterMovementState movementState = CharacterMovementState.Idle;
    private static readonly int TriggerJump01Prepare = Animator.StringToHash("trigger_Jump_01_Prepare");
    private static readonly int TriggerJump02Launch = Animator.StringToHash("trigger_Jump_02_Launch");
    private static readonly int TriggerJump03Fall = Animator.StringToHash("trigger_Jump_03_Fall");
    private static readonly int TriggerJump04Land = Animator.StringToHash("trigger_Jump_04_Land");

    public void SetAnimator(Animator animator)
    {
        this.animator = animator;
    }
    
    public void SetAnimationState(CharacterMovementState newState)
    {
        if (animator == null)
        {
            Debug.LogError("Animator is null");
            //ihgu
            return;
        }
        movementState = newState;
        isIdle = false;
        isRunning = false;
        switch (movementState)
        {
            case CharacterMovementState.Idle:
                isIdle = true;
                break;
            case CharacterMovementState.Running:
                isRunning = true;
                break;
            case CharacterMovementState.Jumping:
                animator.SetTrigger("isJumping");
                break;
            case CharacterMovementState.Jump01Prepare:
                animator.SetTrigger(TriggerJump01Prepare);
                break;
            case CharacterMovementState.Jump02Launch:
                animator.SetTrigger(TriggerJump02Launch);
                break;
            case CharacterMovementState.Jump03Fall:
                animator.SetTrigger(TriggerJump03Fall);
                break;
            case CharacterMovementState.Jump04Land:
                animator.SetTrigger(TriggerJump04Land);
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
