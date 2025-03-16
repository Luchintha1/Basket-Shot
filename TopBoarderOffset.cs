using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopBoarderOffset : MonoBehaviour
{
    private float top_spawn_pos;
    [SerializeField] private float spawn_offset;

    void Start()
    {
        Camera cam = Camera.main;
        float top_screen_pos = cam.ViewportToWorldPoint(new Vector3(0, 1f, 0)).y;
        top_spawn_pos = top_screen_pos + spawn_offset;

        this.gameObject.transform.position = new Vector3(0f, top_spawn_pos, 0f);
    }
}
