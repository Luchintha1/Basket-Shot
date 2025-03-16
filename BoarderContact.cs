using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarderContact : MonoBehaviour
{
    [SerializeField] private Transform leftSpawnX;
    [SerializeField] private Transform rightSpawnX;
    [SerializeField] private float offset;
    [SerializeField] private float xVelocity;

    private Rigidbody2D ballRb;

    // Ball VFX control
    [SerializeField] private GameObject[] trailParticles;
    [SerializeField] private float trailWaitTime;

    private void Start()
    {
        ballRb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "LeftBoarder")
        {
            TrailsOff();
            transform.position = new Vector3(rightSpawnX.position.x - offset, transform.position.y, transform.position.z);
            ballRb.velocity = new Vector2(-xVelocity, ballRb.velocity.y);
            TrailsOn();
        }
        else if (other.tag == "RightBoarder")
        {
            TrailsOff();
            transform.position = new Vector3(leftSpawnX.position.x + offset, transform.position.y, transform.position.z);
            ballRb.velocity = new Vector2(xVelocity, ballRb.velocity.y);
            TrailsOn();
        }
    }

    private void TrailsOff()
    {
        foreach (var trail in trailParticles)
        {
            trail.SetActive(false);
        }
    }

    private IEnumerator TrailOnCo()
    {
        yield return new WaitForSeconds(trailWaitTime);
        foreach (var trail in trailParticles)
        {
            trail.SetActive(true);
        }
    }
    private void TrailsOn()
    {
        StartCoroutine(TrailOnCo());
    }
}
