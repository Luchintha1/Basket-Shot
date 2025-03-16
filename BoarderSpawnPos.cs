using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarderSpawnPos : MonoBehaviour
{
    private float left_spawn_pos, right_spawn_pos;
    [SerializeField] private bool isRight;
    [SerializeField] private float spawn_offset;

    void Start()
    {
        Camera cam = Camera.main;
        float left_screen_pos = cam.ViewportToWorldPoint(new Vector3(0, 0.5f, 0)).x;
        float right_screen_pos = cam.ViewportToWorldPoint(new Vector3(1f, 0.5f, 0)).x;

        left_spawn_pos = left_screen_pos - spawn_offset;
        right_spawn_pos = right_screen_pos + spawn_offset;

        if (isRight)
        {
            this.gameObject.transform.position = new Vector3(right_spawn_pos, 0f, 0f);
        }
        else
        {
            this.gameObject.transform.position = new Vector3(left_spawn_pos, 0f, 0f);
        }
    }
}
