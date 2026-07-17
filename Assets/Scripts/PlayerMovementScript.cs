using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PlayerMovementScript : MonoBehaviour
{
    private static readonly int IsRunningHash = Animator.StringToHash("isRunning");
    private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");

    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private OutlineSelectorScript outlineSelectorScript;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 1.5f;
    [SerializeField] private float runSpeed = 4f;
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private float gravity = -20f;

    [Header("Combat")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackLayerBlendSpeed = 8f;

    private Animator animator;
    private CharacterController characterController;

    private float verticalVelocity;

    private float targetLayer1Weight;
    private float targetLayer2Weight;

    private bool isAttacking;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleMovement();
        HandleMovementAnimation();
        HandleAttack();
        UpdateAttackLayers();
    }

    #region Movement

    private void HandleMovement()
    {
        float x = 0;
        float z = 0;

        if (Keyboard.current.aKey.isPressed) x = -1;
        if (Keyboard.current.dKey.isPressed) x = 1;
        if (Keyboard.current.sKey.isPressed) z = -1;
        if (Keyboard.current.wKey.isPressed) z = 1;

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camForward * z + camRight * x;

        float speed = Keyboard.current.leftShiftKey.isPressed ? runSpeed : walkSpeed;

        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime);
        }

        if (characterController.isGrounded)
        {
            if (verticalVelocity < 0)
                verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
            speed = 0;
        }

        Vector3 finalMove =
            move.normalized * speed +
            Vector3.up * verticalVelocity;

        characterController.Move(finalMove * Time.deltaTime);
    }

    private void HandleMovementAnimation()
    {
        bool isMoving =
            Keyboard.current.wKey.isPressed ||
            Keyboard.current.aKey.isPressed ||
            Keyboard.current.sKey.isPressed ||
            Keyboard.current.dKey.isPressed;

        bool isRunning =
            isMoving &&
            Keyboard.current.leftShiftKey.isPressed &&
            characterController.isGrounded;

        animator.SetBool(IsWalkingHash, isMoving && characterController.isGrounded);
        animator.SetBool(IsRunningHash, isRunning);
    }

    #endregion

    #region Combat

    private void HandleAttack()
    {
        if (isAttacking)
            return;

        if (!outlineSelectorScript.TryGetHoveredSelectable(out RaycastHit hit))
        {
            StopAttack();
            return;
        }

        float distance = Vector3.Distance(
            transform.position,
            hit.collider.ClosestPoint(transform.position));

        if (distance > attackRange)
        {
            StopAttack();
            return;
        }

        RotateTowards(hit.transform.position);

        if (Mouse.current.leftButton.isPressed)
        {
            StartVerticalAttack();
        }
        else if (Mouse.current.rightButton.isPressed)
        {
            StartHorizontalAttack();
        }
    }

    private void StartVerticalAttack()
    {
        targetLayer1Weight = 1f;
        targetLayer2Weight = 0f;
        isAttacking = true;
    }

    private void StartHorizontalAttack()
    {
        targetLayer1Weight = 0f;
        targetLayer2Weight = 1f;
        isAttacking = true;
    }

    private void StopAttack()
    {
        targetLayer1Weight = 0f;
        targetLayer2Weight = 0f;
    }

    private void UpdateAttackLayers()
    {
        animator.SetLayerWeight(
            1,
            Mathf.MoveTowards(
                animator.GetLayerWeight(1),
                targetLayer1Weight,
                attackLayerBlendSpeed * Time.deltaTime));

        animator.SetLayerWeight(
            2,
            Mathf.MoveTowards(
                animator.GetLayerWeight(2),
                targetLayer2Weight,
                attackLayerBlendSpeed * Time.deltaTime));
    }

    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
        StopAttack();
    }

    #endregion

    #region Helpers

    private void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime);
    }

    #endregion
}