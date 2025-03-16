using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFloor : MonoBehaviour
{
    [SerializeField] private float offset;

    void Start()
    {
        Camera cam = Camera.main;
        float screen_Bottom = cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;

        gameObject.transform.position = new Vector2(transform.position.x, screen_Bottom + offset);
    }
}
