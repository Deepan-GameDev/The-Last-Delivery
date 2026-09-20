using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private Transform cameraTransform;

    private CharacterController controller;
    private Vector3 velocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection =
            cameraForward * vertical +
            cameraRight * horizontal;

        if (moveDirection.magnitude > 1f)
            moveDirection.Normalize();

        // Player moves ONLY when WASD is pressed
        if (moveDirection.magnitude > 0.1f)
        {
            if (Mathf.Abs(vertical) > 0.1f)
            {
                Vector3 faceDirection = cameraForward * vertical;

                Quaternion targetRotation =
                    Quaternion.LookRotation(faceDirection) *
                    Quaternion.Euler(0f, 180f, 0f);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
        }

        // Gravity
        if (controller.isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;

        // Movement + Gravity in ONE Move call
        Vector3 finalMovement =
            moveDirection * moveSpeed;

        finalMovement.y = velocity.y;

        controller.Move(finalMovement * Time.deltaTime);
    }
}