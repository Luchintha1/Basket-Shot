using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BallPower : MonoBehaviour
{
    Touch touch;
    private Rigidbody2D ballRb;
    [SerializeField] private float jumpForce;
    [SerializeField] private Transform basketNet;
    [SerializeField][Range(1, 10)] private int xAsixMoveIncreaser = 1;
    [SerializeField] private LevelManager levelManager;

    void Start()
    {
        ballRb = GetComponent<Rigidbody2D>();
        ballRb.isKinematic = true;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began && (UIManager.Instance.game_Started || !UIManager.Instance.game_Paused) &&
                levelManager.time_Counter > 0)
            {
                Vector2 forceDir = new Vector2((basketNet.transform.position.x * xAsixMoveIncreaser), jumpForce);
                ballRb.velocity = forceDir;

            }
            else if (!UIManager.Instance.game_Started)
            {
                StartCoroutine(resetPosCO());
            }

            if (UIManager.Instance.game_Started && !UIManager.Instance.game_Paused)
            {
                ballRb.isKinematic = false;
            }
            else
            {
                ballRb.velocity = Vector2.zero;
                ballRb.isKinematic = true;
            }
        }

    }

    IEnumerator resetPosCO()
    {
        yield return new WaitForSeconds(1.5f);
        transform.position = Vector2.zero;
        ballRb.velocity = Vector2.zero;
        UIManager.Instance.game_Started = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "ground")
        {
            UIManager.Instance.on_ground = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.collider.tag == "ground")
        {
            UIManager.Instance.on_ground = false;
        }
    }
}
