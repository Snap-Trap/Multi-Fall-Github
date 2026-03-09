using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    public PlayerCamera playerCamera;

    public NetworkVariable<Vector3> playerPosition = new NetworkVariable<Vector3>(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<Quaternion> playerNetRotation = new NetworkVariable<Quaternion>(writePerm: NetworkVariableWritePermission.Server);

    public InputAction forward, backward, left, right, jump;

    public float maxRayDistance = 1.2f;

    public float speed, jumpForce, playerRotation;

    private Rigidbody rb;

    public LayerMask whatIsGround;

    [SerializeField] private bool isGrounded;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCamera = GetComponentInChildren<PlayerCamera>();
    }

    public override void OnNetworkSpawn()
    {
        gameObject.name = NetworkObjectId.ToString();
        
        if (IsServer)
        {
            playerPosition.Value = transform.position;
            playerNetRotation.Value = transform.rotation;
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        // read input
        float forwardValue = forward.ReadValue<float>();
        float backwardValue = backward.ReadValue<float>();
        float leftValue = left.ReadValue<float>();
        float rightValue = right.ReadValue<float>();
        bool jumpPressed = jump.triggered;
        float rotationX = playerCamera.turn.x;
        float rotationY = playerCamera.turn.y;

        // send input to server
        SendMovementRpc(forwardValue, backwardValue, leftValue, rightValue, rotationY, rotationX, jumpPressed);
    }

    [Rpc(SendTo.Server)]
    private void SendMovementRpc(float forwardValue, float backwardValue, float leftValue, float rightValue, float rotationY, float rotationX, bool jumpPressed, RpcParams rpcParams = default)
    {
        // Makes it so the other script that controls the camera gives the rotation and you can use it here so forward goes forward based on--
        // what the player's forward is instead of the scene's forward
        // Touch this and I'll break your kneecaps
        Debug.Log("Received movement RPC with values: " + forwardValue + ", " + backwardValue + ", " + leftValue + ", " + rightValue + ", " + rotationY + ", " + rotationX + ", " + jumpPressed);

        playerRotation = playerCamera.turn.x;

        Quaternion playerRotationQuaternion = Quaternion.Euler(0, playerRotation, 0);

        Vector3 forwardMovement = playerRotationQuaternion * Vector3.forward;
        Vector3 rightMovement = playerRotationQuaternion * Vector3.right;

        Vector3 movement = Vector3.zero;

        // basic movement shit
        // Uhhhh so this is going to be backwards? Don't ask me.
        if (forwardValue == 1)
        {
            movement += forwardMovement * speed;
        }
        if (backwardValue == 1)
        {
            movement += -forwardMovement * speed;
        }
        if (leftValue == 1)
        {
            movement += -rightMovement * speed;
        }
        if (rightValue == 1)
        {
            movement += rightMovement * speed;
        }

        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);



        // Checks for jump button and if ground is touched
        if (isGrounded && jump.triggered)
        {
            rb.linearVelocity += new Vector3(0, jumpForce, 0);
        }

        // Raycast you bloodgunging watermelon
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out RaycastHit groundHit, maxRayDistance, whatIsGround))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down), Color.red, maxRayDistance);

        playerPosition.Value = transform.position;
        playerNetRotation.Value = transform.rotation;

    }
    // Input System stuff
    private void OnEnable()
    {
        forward.Enable();
        backward.Enable();
        left.Enable();
        right.Enable();
        jump.Enable();
    }

    private void OnDisable()
    {
        forward.Disable();
        backward.Disable();
        left.Disable();
        right.Disable();
        jump.Disable();
    }
}
