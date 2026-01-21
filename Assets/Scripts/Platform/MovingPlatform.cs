using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Type")]
    public MovementType movementType = MovementType.Waypoints;

    [Header("Waypoint Movement")]
    public Transform[] waypoints;
    public float speed = 2f;
    public float waitTime = 1f;
    private int currentWaypointIndex = 0;
    private float waitCounter;
    private bool waiting = false;

    [Header("Horizontal/Vertical Movement")]
    public float moveDistance = 5f;
    public bool startMovingRight = true;
    public bool startMovingUp = true;
    private Vector3 startPosition;
    private bool movingForward = true;

    [Header("Circular Movement")]
    public float circleRadius = 3f;
    public float circleSpeed = 2f;
    private float angle = 0f;

    [Header("Settings")]
    public bool loop = true;
    public bool smoothMovement = true;
    public AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector3 targetPosition;
    private Rigidbody2D rb;

    private Vector3 previousPosition;
    public Vector2 platformVelocity { get; private set; }

    public enum MovementType
    {
        Waypoints,
        Horizontal,
        Vertical,
        Circular
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        startPosition = transform.position;
        previousPosition = transform.position;

        switch (movementType)
        {
            case MovementType.Waypoints:
                if (waypoints.Length > 0)
                    targetPosition = waypoints[0].position;
                break;
            case MovementType.Horizontal:
                movingForward = startMovingRight;
                break;
            case MovementType.Vertical:
                movingForward = startMovingUp;
                break;
        }
    }

    void FixedUpdate()
    {
        previousPosition = transform.position;

        switch (movementType)
        {
            case MovementType.Waypoints:
                MoveAlongWaypoints();
                break;
            case MovementType.Horizontal:
                MoveHorizontal();
                break;
            case MovementType.Vertical:
                MoveVertical();
                break;
            case MovementType.Circular:
                MoveCircular();
                break;
        }

        platformVelocity = (transform.position - previousPosition) / Time.fixedDeltaTime;
    }

    void MoveAlongWaypoints()
    {
        if (waypoints.Length == 0) return;

        if (waiting)
        {
            waitCounter -= Time.fixedDeltaTime;
            if (waitCounter <= 0)
            {
                waiting = false;
                currentWaypointIndex++;

                if (currentWaypointIndex >= waypoints.Length)
                {
                    if (loop)
                        currentWaypointIndex = 0;
                    else
                        currentWaypointIndex = waypoints.Length - 1;
                }

                targetPosition = waypoints[currentWaypointIndex].position;
            }
            return;
        }

        Vector3 newPosition = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPosition);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            waiting = true;
            waitCounter = waitTime;
        }
    }

    void MoveHorizontal()
    {
        Vector3 targetPos;

        if (movingForward)
        {
            targetPos = startPosition + Vector3.right * moveDistance;

            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                if (loop)
                    movingForward = false;
            }
        }
        else
        {
            targetPos = startPosition;

            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                movingForward = true;
            }
        }

        Vector3 newPosition = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPosition);
    }

    void MoveVertical()
    {
        Vector3 targetPos;

        if (movingForward)
        {
            targetPos = startPosition + Vector3.up * moveDistance;

            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                if (loop)
                    movingForward = false;
            }
        }
        else
        {
            targetPos = startPosition;

            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                movingForward = true;
            }
        }

        Vector3 newPosition = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPosition);
    }

    void MoveCircular()
    {
        angle += circleSpeed * Time.fixedDeltaTime;

        float x = Mathf.Cos(angle) * circleRadius;
        float y = Mathf.Sin(angle) * circleRadius;

        Vector3 newPosition = startPosition + new Vector3(x, y, 0);
        rb.MovePosition(newPosition);
    }

    void OnDrawGizmos()
    {
        if (movementType == MovementType.Waypoints && waypoints != null && waypoints.Length > 0)
        {
            Gizmos.color = Color.yellow;

            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] != null)
                {
                    Gizmos.DrawWireSphere(waypoints[i].position, 0.3f);

                    if (i < waypoints.Length - 1 && waypoints[i + 1] != null)
                    {
                        Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                    }
                    else if (loop && i == waypoints.Length - 1 && waypoints[0] != null)
                    {
                        Gizmos.DrawLine(waypoints[i].position, waypoints[0].position);
                    }
                }
            }
        }
        else if (movementType == MovementType.Horizontal)
        {
            Vector3 start = Application.isPlaying ? startPosition : transform.position;
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(start, start + Vector3.right * moveDistance);
            Gizmos.DrawWireSphere(start, 0.2f);
            Gizmos.DrawWireSphere(start + Vector3.right * moveDistance, 0.2f);
        }
        else if (movementType == MovementType.Vertical)
        {
            Vector3 start = Application.isPlaying ? startPosition : transform.position;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(start, start + Vector3.up * moveDistance);
            Gizmos.DrawWireSphere(start, 0.2f);
            Gizmos.DrawWireSphere(start + Vector3.up * moveDistance, 0.2f);
        }
        else if (movementType == MovementType.Circular)
        {
            Vector3 center = Application.isPlaying ? startPosition : transform.position;
            Gizmos.color = Color.magenta;

            int segments = 32;
            float angleStep = 360f / segments;

            for (int i = 0; i < segments; i++)
            {
                float angle1 = Mathf.Deg2Rad * (i * angleStep);
                float angle2 = Mathf.Deg2Rad * ((i + 1) * angleStep);

                Vector3 point1 = center + new Vector3(
                    Mathf.Cos(angle1) * circleRadius,
                    Mathf.Sin(angle1) * circleRadius,
                    0
                );

                Vector3 point2 = center + new Vector3(
                    Mathf.Cos(angle2) * circleRadius,
                    Mathf.Sin(angle2) * circleRadius,
                    0
                );

                Gizmos.DrawLine(point1, point2);
            }
        }
    }
}