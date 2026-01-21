using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextFollowsPlayer : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 2, 0);

    void LateUpdate()
    {
        if (player != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(player.position + offset);
        }
    }
}