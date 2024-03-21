using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [field: SerializeField]
    public Transform targetObject;

    [SerializeField]
    private float angleXToTarget;
    [SerializeField]
    private Vector3 offsetToTarget;

    [SerializeField]
    private float speedRotate;

    void Update()
    {
        Move();
        Rotate();
    }

    private void Move()
    {
        int dirOffsetX = (transform.eulerAngles.y >= 0 && transform.eulerAngles.y < 180) 
            ? -1 
            : 1;

        int dirOffsetZ = ((transform.eulerAngles.y >= 0 && transform.eulerAngles.y < 90) || (transform.eulerAngles.y >= 270 && transform.eulerAngles.y < 360))
            ? -1 
            : 1;

        Vector3 offset = new Vector3 (offsetToTarget.x * dirOffsetX, offsetToTarget.y, offsetToTarget.z * dirOffsetZ);
        transform.position = targetObject.position + offset;
    }

    private void Rotate()
    {
        Vector3 dirMovement = targetObject.GetComponent<Moving>().Target - targetObject.position;
        if (dirMovement != Vector3.zero)
        {
            dirMovement.Normalize();
            Quaternion quaternion = Quaternion.LookRotation(dirMovement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, quaternion, 10 * targetObject.GetComponent<Moving>().Speed * Time.deltaTime);
        }
        transform.eulerAngles = new Vector3(angleXToTarget, transform.eulerAngles.y, transform.eulerAngles.z);
    }
}