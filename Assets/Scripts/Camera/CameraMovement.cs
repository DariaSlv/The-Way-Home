using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    public float orthoSize = 6f;
    public float yDeadZone = 1f;
    public float minY = -2f;
    public float maxY = 2f;

    public float ySmoothSpeed = 0.05f;

    void Start()
    {
        Camera cam = GetComponent<Camera>();
        if (cam != null)
            cam.orthographicSize = orthoSize;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        float yDiff = desiredPosition.y - transform.position.y;
        if (Mathf.Abs(yDiff) < yDeadZone)
            desiredPosition.y = transform.position.y;

        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, new Vector3(desiredPosition.x, transform.position.y, desiredPosition.z), smoothSpeed);

        float smoothY = Mathf.Lerp(transform.position.y, desiredPosition.y, ySmoothSpeed);
        smoothedPosition.y = smoothY;

        transform.position = smoothedPosition;
    }
}