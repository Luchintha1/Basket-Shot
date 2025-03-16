using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallLookChange : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRen;
    [SerializeField] private List<Sprite> ballLooks;

    
    public void BallSelection()
    {
        int ball_look = PlayerPrefs.GetInt("ball_look", 0);

        switch (ball_look)
        {
            case 0:
                spriteRen.sprite = ballLooks[0];
                break;

            case 1:
                spriteRen.sprite = ballLooks[1];
                break;

            case 2:
                spriteRen.sprite = ballLooks[2];
                break;

            case 3:
                spriteRen.sprite = ballLooks[3];
                break;

            case 4:
                spriteRen.sprite = ballLooks[4];
                break;

            case 5:
                spriteRen.sprite = ballLooks[5];
                break;

            default:
                PlayerPrefs.SetInt("ball_look", 0);
                break;

        }
    }
    
    private void Start()
    {
        BallSelection();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
