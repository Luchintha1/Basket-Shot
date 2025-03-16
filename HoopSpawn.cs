using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoopSpawn : MonoBehaviour
{
    [SerializeField] private ScoreDetector score_detector;
    [SerializeField] private UIManager ui_manager;

    [SerializeField] private float minYValue, maxYValue;

    private float left_x_startpos, right_x_startpos;
    private float left_x_goalpos, right_x_goalpos;

    private float left_turn = 180f, right_turn = 0f;
    [SerializeField] private float move_speed;
    [SerializeField] private float goal_pos_offset, spawn_pos_offset;

    [SerializeField] private HoopCoplliderController hoop_controller;

    private bool isRight;
    private bool inPos;

    private void Start()
    {
        isRight = true;
        inPos = false;

        CalculateScreenBounds();
        //Spawn();
        score_detector.OnGoal += Spawn;
        ui_manager.OnResetGame += Spawn;
    }

    private void OnDestroy()
    {
        score_detector.OnGoal -= Spawn;
        ui_manager.OnResetGame -= Spawn;
    }

    private void FixedUpdate()
    {
        if (isRight && !inPos)
        {
            // Debug.Log("Move To Right");
            MoveToPos(right_x_goalpos);
        }
        else if (!isRight && !inPos)
        {
            // Debug.Log("Move To Left");
            MoveToPos(left_x_goalpos);
        }
    }

    private void CalculateScreenBounds()
    {
        Camera cam = Camera.main;

        float screen_Edge_Left = cam.ViewportToWorldPoint(new Vector3(0, 0.5f, 0)).x;
        float screen_Edge_Right = cam.ViewportToWorldPoint(new Vector3(1, 0.5f, 0)).x;

        left_x_startpos = screen_Edge_Left - spawn_pos_offset;
        right_x_startpos = screen_Edge_Right + spawn_pos_offset;

        left_x_goalpos = screen_Edge_Left + goal_pos_offset;
        right_x_goalpos = screen_Edge_Right - goal_pos_offset;
    }

    private void MoveToPos(float new_x_pos)
    {
        ScoreDecider.instance.isHitBar = false;
        hoop_controller.score_detector.enabled = false;
        hoop_controller.hoop_Collider.enabled = true;

        gameObject.transform.position = Vector2.MoveTowards((Vector2)gameObject.transform.position,
                new Vector2(new_x_pos, transform.position.y), move_speed * Time.deltaTime);

        if(!inPos && isRight && gameObject.transform.position.x >= left_x_goalpos)
        {
            inPos = true;
        }
        else if(!inPos && !isRight && gameObject.transform.position.x <= right_x_goalpos)
        {
            inPos = true;
        }

    }

    private void Spawn()
    {
        ScoreDecider.instance.isHitBar = false;

        if (isRight)
        {
            gameObject.transform.position = new Vector2(right_x_startpos, Random.Range(minYValue, maxYValue));
            gameObject.transform.eulerAngles = new Vector2(gameObject.transform.rotation.x, right_turn);
            inPos = false;
            isRight = false;
        }
        else
        {
            gameObject.transform.position = new Vector2(left_x_startpos, Random.Range(minYValue, maxYValue));
            gameObject.transform.eulerAngles = new Vector2(gameObject.transform.rotation.x, left_turn);
            inPos = false;
            isRight = true;
        }
    }

}
