using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallDetectorBackwords : MonoBehaviour
{
    [SerializeField] Collider2D hoop_Collider;
    [SerializeField] private float setColliderTimer;
    private float colliderTimerCounter;
    private bool collide;

    [SerializeField] Collider2D ballDetectorTop;

    private void Start()
    {
        collide = false;
        colliderTimerCounter = setColliderTimer;
    }

    private void Update()
    {
        if (collide)
        {
            colliderTimerCounter -= Time.deltaTime;

            if (colliderTimerCounter < 0)
            {
                colliderTimerCounter = setColliderTimer;
                collide = false;

                hoop_Collider.enabled = true;
                ballDetectorTop.enabled = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Ball")
        {
            ballDetectorTop.enabled = false;
            hoop_Collider.enabled = false;
            collide = true;
        }
    }
}
