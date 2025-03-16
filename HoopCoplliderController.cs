using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoopCoplliderController : MonoBehaviour
{
    [field: SerializeField] public Collider2D hoop_Collider { get; set; }
    [SerializeField] private float setColliderTimer;
    private float colliderTimerCounter;
    private bool collide;

    [field : SerializeField] public Collider2D score_detector { get; set; }

    private void Start()
    {
        collide = false;
        colliderTimerCounter = setColliderTimer;

        score_detector.enabled = false;
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
                score_detector.enabled = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Ball")
        {
            hoop_Collider.enabled = false;
            collide = true;

            // Turn On Score Detector
            score_detector.enabled = true;
        }
    }

    
    // If ball just sit on the hoop
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Ball")
        {
            hoop_Collider.enabled = false;
            collide = true;

            // Turn On Score Detector
            if (!score_detector.enabled)
            {
                score_detector.enabled = true;
            }
        }
    }
}
