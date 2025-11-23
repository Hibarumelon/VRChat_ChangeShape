
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CameraMover : UdonSharpBehaviour
{
    [SerializeField] private Transform targetTransform;
    private Vector3 offset;

    private void Start()
    {
        offset = this.transform.position - targetTransform.position;
        offset.y = 0f;
    }
    
    private void LateUpdate()
    {
        Vector3 tempPos = targetTransform.position + offset;
        tempPos.y = this.transform.position.y;
        this.transform.position = tempPos;
    }
}
