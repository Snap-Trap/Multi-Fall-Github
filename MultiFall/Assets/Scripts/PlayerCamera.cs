using Unity.Netcode;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    //public NetworkVariable<float> networkPitch = new NetworkVariable<float>(writePerm: NetworkVariableWritePermission.Owner);
    //public Transform playerBody, playerPivot;

    public Camera playerCamera;
    public Vector2 turn;

    //public float sensitivity = 3f;
    public float pitchMin = -30f;
    public float pitchMax = 30f;
    //public float pitch;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            playerCamera.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        turn.x += Input.GetAxis("Mouse X");
        turn.y += Input.GetAxis("Mouse Y");

        turn.y = Mathf.Clamp(turn.y, pitchMin, pitchMax);
        transform.localRotation = Quaternion.Euler(-turn.y, turn.x, 0);
    }
}

    //[Rpc(SendTo.Server)]
    //public void TurnRpc(float x, float y, RpcParams rpcParams = default)
    //{
    //    turn.x += x;
    //    turn.y += y;

    //    turn.y = Mathf.Clamp(turn.y, pitchMin, pitchMax);

    //    transform.localRotation = Quaternion.Euler(-turn.y, turn.x, 0);
    //}
