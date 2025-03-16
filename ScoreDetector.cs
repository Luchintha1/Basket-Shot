using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDetector : MonoBehaviour
{
    public static ScoreDetector instance;

    [SerializeField] private LevelManager levelManager;
    [SerializeField] private HoopSpawn hoop;
    [field : SerializeField] public ParticleSystem hoopVFX {get; set;}

    public event Action OnGoal;
    private int total_Hoops = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        total_Hoops = PlayerPrefs.GetInt("totalHoops", 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Ball")
        {

            if (!ScoreDecider.instance.isHitBar)
            {
                // When the ball goes in without touching the bar
                hoopVFX.Play();
                total_Hoops += 2;
                levelManager.score += 2;
            }
            else
            {
                // When the ball goes in after touching the bar
                total_Hoops++;
                levelManager.score++;
            }

            // OnScore?.Invoke();
            PlayerPrefs.SetInt("totalHoops", total_Hoops);
            levelManager.time_Counter = levelManager.time_Setter;

            OnGoal?.Invoke();
        }
    }
}
