using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class CubeRpc : NetworkBehaviour
{
    public Vector3 cubeTransform;

    public void Start()
    {
        cubeTransform = GetComponent<Transform>().localScale;
    }

    //public void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.CompareTag("Player"))
    //    {
    //        CubeGrowSizeRpc();
    //    }
    //}

    //[Rpc(SendTo.Everyone)]
    //public void CubeGrowSizeRpc()
    //{
    //    cubeTransform *= 1.5f;
    //}

    
    //public void SubmitCubeSizeRpc(Vector3 newCubeTransform, RpcParams rpcParams = default)
    //{
    //    cubeTransform = newCubeTransform;
    //    GetComponent<Transform>().localScale = cubeTransform;
    //}
}
