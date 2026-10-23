
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;

    private CharacterController controller;
    private Transform cameraTransform;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
            return;

        float horizontal = 0f;
        float vertical = 0f;

        if (keyboard.aKey.isPressed) horizontal -= 1f;
        if (keyboard.dKey.isPressed) horizontal += 1f;

        if (keyboard.wKey.isPressed) vertical += 1f;
        if (keyboard.sKey.isPressed) vertical -= 1f;

        Vector3 direction = new Vector3(horizontal, 0, vertical);

        if (cameraTransform != null)
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            direction = forward * vertical + right * horizontal;
        }

        direction = Vector3.ClampMagnitude(direction, 1f);

        controller.SimpleMove(direction * moveSpeed);
    }
}
