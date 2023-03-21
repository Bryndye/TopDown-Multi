using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerCamera : MonoBehaviour
{
    public Transform Player;
    [SerializeField] private float speedCam = 200;

    void FixedUpdate()
    {
        Vector3 direction = ((Vector2)Player.position - (Vector2)transform.position);

        transform.position += direction / speedCam;
    }
}
