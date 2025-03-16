using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDecider : MonoBehaviour
{
    public static ScoreDecider instance;

    [SerializeField] private ScoreDetector score_dec;

    public bool isHitBar { get; set; }

    private void Awake()
    {
        instance = this;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.collider.tag == "Ball")
        {
            Debug.Log("Hit");
            instance.isHitBar = true;

            score_dec.hoopVFX.Stop();
        }
        
    }
}
