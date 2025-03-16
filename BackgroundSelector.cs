using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSelector : MonoBehaviour
{

    [SerializeField] private List<Sprite> bgImages;
    [SerializeField] private SpriteRenderer bg_SP;

    public void ChangeBG()
    {
        int random_Num = Random.Range(0, bgImages.Count - 1);
        bg_SP.sprite = bgImages[random_Num];
    }

}
