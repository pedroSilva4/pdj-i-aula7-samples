
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerStats))]
public class ThirdPersonMovement : MonoBehaviour
{


    [SerializeField] private float Gravity  = -9.81f;
    private PlayerStats Stats;


    [SerializeField] private Transform CameraTransform;

    private CharacterController _CharacterController;
    private Vector2 _MoveInput;
    private float _VerticalVelocity;

    void Start()
    {
        _CharacterController = GetComponent<CharacterController>();
        Stats = GetComponent<PlayerStats>();

        if (CameraTransform == null && Camera.main != null)
            CameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (_CharacterController.isGrounded && _VerticalVelocity < 0)
            _VerticalVelocity = -2f;

        _VerticalVelocity += Gravity * Time.deltaTime;

        Vector3 moveDirection = Vector3.zero;

        if (_MoveInput.magnitude > 0.1f)
        {
            Vector3 input = new Vector3(_MoveInput.x, 0f, _MoveInput.y).normalized;

            //if the movement is a step backwards, do not rotate the character, just move backwards
            if (input.z < 0)
            {
                if (CameraTransform != null)
                {
                    Vector3 camRight = Vector3.Scale(CameraTransform.right, new Vector3(1, 0, 1)).normalized;
                    moveDirection = -transform.forward * Mathf.Abs(input.z) + camRight * input.x;
                }
                else
                {
                    moveDirection = new Vector3(input.x, 0f, -Mathf.Abs(input.z));
                }
            }
            else if (CameraTransform != null)
            {
                Vector3 camForward = Vector3.Scale(CameraTransform.forward, new Vector3(1, 0, 1)).normalized;
                Vector3 camRight   = Vector3.Scale(CameraTransform.right,   new Vector3(1, 0, 1)).normalized;
                moveDirection = camForward * input.z + camRight * input.x;
            }
            else
            {
                moveDirection = input;
            }

            if (input.z >= 0)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Stats.RotationSpeed * Time.deltaTime);
            }
        }

        Vector3 velocity = moveDirection * Stats.Speed;
        velocity.y = _VerticalVelocity;
        _CharacterController.Move(velocity * Time.deltaTime);
    }

    public void OnMove(InputValue value)
    {
        _MoveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (_CharacterController.isGrounded)
            _VerticalVelocity = Stats.JumpForce;
    }
}
