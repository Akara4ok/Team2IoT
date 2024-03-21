using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform targetObject;
    private Vector3 initalOffset;
    private Vector3 cameraPosition;

    void Start()
    {
        initalOffset = transform.position - targetObject.position;
    }

    void Update()
    {
        cameraPosition = targetObject.position + initalOffset;
        transform.position = cameraPosition;
    }
}