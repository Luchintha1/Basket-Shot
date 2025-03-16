using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // public static LevelManager instance;
    [SerializeField] private UIManager ui_Manager;
    [SerializeField] private float time_score_var;

    [field:SerializeField] public float time_Setter { get; private set; }
    public float time_Counter { get; internal set; }
    public int score { get; internal set; }

    public int best_Score { get; internal set; }


    private void Awake()
    {
        // instance = this;
    }

    private void Start()
    {
        score = 0;
        time_Counter = time_Setter;

        best_Score = PlayerPrefs.GetInt("BestScore", 0);
        // Debug.Log(Time.deltaTime);
    }

    private void Update()
    {
        if ((time_Counter > 0) && (score >= 1))
        {
            time_Counter -= (Time.deltaTime + (score / time_score_var)) * Time.deltaTime;
        }

        // Debug.Log(time_Counter);

        if (score >= best_Score)
        {
            PlayerPrefs.SetInt("BestScore", score);
            best_Score = PlayerPrefs.GetInt("BestScore", 0);
        }
    }

}
