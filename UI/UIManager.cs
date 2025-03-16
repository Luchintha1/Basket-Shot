using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public event Action OnResetGame;

    [SerializeField] private Animator mainMenu_Anim;
    [SerializeField] private LevelManager levelManager;

    public bool game_Started { get; set; }
    public bool game_Paused { get; private set; }
    public bool on_ground { get; internal set; }

    // Main Menu
    [SerializeField] private Text bestScore_MainMenu;
    [SerializeField] private GameObject opt_Panel;
    [SerializeField] private List<GameObject> choosenBall;

    [SerializeField] private Text state_Highest_Score, games_Played, total_hoop_Count, avg_Hoop_Count;
    [SerializeField] private String privacyPolicyUrl = "https://sites.google.com/view/basket-shot-privacy-policy/home?authuser=4";

    int games_played = 0;

    // UI
    [SerializeField] private GameObject paused_Menu;
    [SerializeField] private GameObject paused_Button;
    [SerializeField] private Text paused_Text;
    [SerializeField] private Text score_text;
    [SerializeField] private Text score_add_text;
    [SerializeField] private Slider time_slider;
    [SerializeField] private Image time_slider_image;
    [SerializeField] private RectTransform fill_holder;

    [SerializeField] private GameObject replay_Panel;
    [SerializeField] private Text paused_Menu_HS;
    private float pulseTimer = 0f;

    Touch touch;
    public bool first_Touch;
    [SerializeField] private float pulse_Speed = 0.1f;
    [SerializeField] public RectTransform instru_Text;


    // Game Over
    [SerializeField] private GameObject game_Over_Panel;
    [SerializeField] private Text game_Over_Score;
    [SerializeField] private Text gameOver_bestScore;

    // Ball Change
    [SerializeField] private BallLookChange ball_Look_Change;

    // BG Change
    [SerializeField] private BackgroundSelector bg_Selector;


    // In Game Score Anim
    [SerializeField] private Animator in_game_anim;
    [SerializeField] private Animator replay_anim;

    // UI VFX
    [SerializeField] private ParticleSystem fire_Works;

    // UI SFX
    [SerializeField] private Image Sfx_Icon;
    [SerializeField] private Image in_game_Sfx_Icon;
    [SerializeField] private Sprite on_Sfx_Icon, off_Sfx_Icon;

    // Ads
    [SerializeField] private InterstitialAd interstitial_Ads;
    [SerializeField] private RewardedAds reward_Ads;
    [SerializeField] public GameObject rewardAdButton;

    [field: SerializeField] private int adWaitSetter;
    private int adWaitCounter;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        paused_Menu.SetActive(false);
        paused_Button.SetActive(false);
        game_Over_Panel.SetActive(false);
        replay_Panel.SetActive(false);
        opt_Panel.SetActive(false);


        Time.timeScale = 1f;
        game_Started = false;
        game_Paused = false;
        on_ground = false;

        first_Touch = false;
        fire_Works.Stop();

        bestScore_MainMenu.text = PlayerPrefs.GetInt("BestScore", 0).ToString();
        score_text.text = levelManager.score.ToString();
        choosenBallTick();
        soundIcon();

        // Stats Tracker

        games_played = PlayerPrefs.GetInt("TotalGames", 0);

        ScoreDetector.instance.OnGoal += ScoreUpdate;

        // Ad Loader
        StartCoroutine(LoadingAdCo());
    }

    IEnumerator LoadingAdCo()
    {
        yield return new WaitForSeconds(0.5f);
        // Ad Loader
        interstitial_Ads.LoadAd();
        adWaitCounter = adWaitSetter;

        reward_Ads.LoadAd();
    }

    private void OnDestroy()
    {
        ScoreDetector.instance.OnGoal -= ScoreUpdate;
    }

    private void Update()
    {
        if (!first_Touch)
        {
            instructionAnim();

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began && game_Started)
                {
                    first_Touch = true;
                    instru_Text.gameObject.SetActive(false);
                }
            }
        }

        time_slider.value = levelManager.time_Counter;

        // Changing the color of the Slider bar

        float normalizedTimer = Mathf.Clamp01(levelManager.time_Counter / levelManager.time_Setter);

        Color startColor = Color.green;
        Color midColor = Color.yellow;
        Color endColor = Color.red;

        if(normalizedTimer > 0.5)
        {
            float t = (normalizedTimer - 0.5f) * 2;
            time_slider_image.color = Color.Lerp(midColor, startColor, t); // green to yellow
        }
        else
        {
            float t = normalizedTimer * 2;
            time_slider_image.color = Color.Lerp(endColor, midColor, t); // yellow to red
        }

        // Pulsing Effect (Heartbeat Animation)

        if (fill_holder != null)
        {
            float baseSize = 1f;  // Normal scale size
            float pulseAmount = 0.2f; // How much it grows/shrinks
            float minPulseSpeed = 2f;  // Slow pulsing when full
            float maxPulseSpeed = 8f; // Fast pulsing when time is low

            pulseTimer += Time.deltaTime;
            if (pulseTimer > Mathf.PI * 2f) pulseTimer = 0f; // Reset to avoid precision loss
            float pulseSpeed = Mathf.Lerp(minPulseSpeed, maxPulseSpeed, 1 - normalizedTimer);
            float pulseEffect = Mathf.Sin(pulseTimer * pulseSpeed) * pulseAmount;
            fill_holder.localScale = new Vector3(1f, baseSize + pulseEffect, 1f);
        }

        if (levelManager.time_Counter <= 0 && on_ground)
        {
            gameOverPanelOn();
        }
    }

    private void InterstitialAdCount()
    {
        if (adWaitCounter <= 0)
        {
            interstitial_Ads.ShowAd();
            adWaitCounter = adWaitSetter;
        }
        else
        {
            adWaitCounter--;
            Debug.Log(adWaitCounter);
        }
    }

    public void OnRewardAds()
    {
        soundController(0, 0.8f);
        fire_Works.Stop();
        levelManager.time_Counter = levelManager.time_Setter;
        paused_Button.SetActive(true);
        game_Over_Panel.SetActive(false);
        paused_Menu.SetActive(false);

        levelManager.time_Counter = levelManager.time_Setter;
        time_slider.gameObject.SetActive(true);
        score_text.gameObject.SetActive(true);

        game_Started = true;
        game_Paused = false;

        Time.timeScale = 1f;
    }

    private void ScoreUpdate()
    {
        if (!ScoreDecider.instance.isHitBar)
        {
            score_add_text.text = "+ 2";
        }
        else
        {
            score_add_text.text = "+ 1";
        }

        in_game_anim.SetTrigger("OnScore");
        score_text.text = levelManager.score.ToString();
    }

    private void instructionAnim()
    {
        if (instru_Text != null)
        {
            float baseSize = 1f;  // Normal scale size
            float pulseAmount = 0.2f; // How much it grows/shrinks

            pulseTimer += Time.deltaTime;
            if (pulseTimer > Mathf.PI * 2f) pulseTimer = 0f;
            float pulseEffect = Mathf.Sin(pulseTimer * pulse_Speed) * pulseAmount;
            instru_Text.localScale = new Vector3(baseSize + pulseEffect, baseSize + pulseEffect, 1f);
        }
    }

    private void gameOverPanelOn()
    {
        gameOver_bestScore.text = PlayerPrefs.GetInt("BestScore", 0).ToString();
        Time.timeScale = 0f;
        game_Paused = true;
        game_Over_Score.text = levelManager.score.ToString();

        if (levelManager.score >= PlayerPrefs.GetInt("BestScore", 0))
        {
            fire_Works.Play();
        }
        else
        {
            fire_Works.Stop();
        }

        game_Over_Panel.SetActive(true);

        paused_Button.SetActive(false);
        time_slider.gameObject.SetActive(false);
        score_text.gameObject.SetActive(false);
    }

    private void gamePausedOverMenu()
    {
        paused_Button.SetActive(false);
        time_slider.gameObject.SetActive(false);
        score_text.gameObject.SetActive(false);
    }

    private void gameStartMenu()
    {
        paused_Button.SetActive(true);
        time_slider.gameObject.SetActive(true);
        score_text.gameObject.SetActive(true);
    }

    public void playGame()
    {
        soundController(0, 0.8f);

        bg_Selector.ChangeBG();

        games_played++;
        PlayerPrefs.SetInt("TotalGames", games_played);

        mainMenu_Anim.SetBool("inToGame", true);
        game_Over_Panel.SetActive(false);
        rewardAdButton.SetActive(true);

        gameStartMenu();

        // Reset Ads

        Time.timeScale = 1f;
        StartCoroutine(reset_netCO());
        StartCoroutine(grace_TimeCo());
    }

    public void pausedButton()
    {
        soundController(0, 1f);

        paused_Menu.SetActive(true);
        game_Over_Panel.SetActive(false);

        gamePausedOverMenu();

        paused_Text.text = levelManager.score.ToString();
        paused_Menu_HS.text = PlayerPrefs.GetInt("BestScore", 0).ToString();

        Time.timeScale = 0f;
        game_Paused = true;
    }

    public void resumeGame()
    {
        soundController(0, 1f);

        paused_Menu.SetActive(false);
        paused_Button.SetActive(true);
        game_Over_Panel.SetActive(false);
        gameStartMenu();

        Time.timeScale = 1f;
        game_Paused = false;
    }

    public void replayGame()
    {
        soundController(0, 1f);

        fire_Works.Stop();

        InterstitialAdCount();
        replay_anim.SetTrigger("change");

        // Reset Ads
        rewardAdButton.SetActive(true);

        replay_Panel.SetActive(true);
        game_Paused = true;
        game_Started = false;

        games_played++;
        PlayerPrefs.SetInt("TotalGames", games_played);
        ScoreDetector.instance.hoopVFX.Stop();

        Time.timeScale = 1f;
        StartCoroutine(reset_netCO());
        StartCoroutine(grace_TimeCo());
        gameStartMenu();
        StartCoroutine(resetCO());
    }

    public void mainMenu()
    {
        soundController(0, 1f);

        fire_Works.Stop();

        InterstitialAdCount();

        mainMenu_Anim.SetBool("inToGame", false);
        gamePausedOverMenu();


        game_Started = false;
        bestScore_MainMenu.text = PlayerPrefs.GetInt("BestScore", 0).ToString();
        ScoreDetector.instance.hoopVFX.Stop();

        // Reset Ads
        rewardAdButton.SetActive(true);

        StartCoroutine(resetMainCO());
        Time.timeScale = 1f;
    }

    IEnumerator reset_netCO()
    {
        yield return new WaitForSeconds(1f);
        OnResetGame?.Invoke();
    }

    /*IEnumerator replay_PanelOpenCloaseCo()
    {
        yield return new WaitForSeconds(3f);
        replay_Panel.SetActive(false);
    }*/

    IEnumerator grace_TimeCo()
    {
        // StartCoroutine(replay_PanelOpenCloaseCo());

        yield return new WaitForSeconds(1f);

        game_Started = true;
        game_Paused = false;
    }

    IEnumerator resetMainCO()
    {
        levelManager.time_Counter = levelManager.time_Setter;
        paused_Button.SetActive(true);
        game_Over_Panel.SetActive(false);
        paused_Menu.SetActive(false);

        yield return new WaitForSeconds(1f);
        levelManager.score = 0;
        score_text.text = "0";
        levelManager.time_Counter = levelManager.time_Setter;
    }

    IEnumerator resetCO()
    {
        levelManager.time_Counter = levelManager.time_Setter;
        paused_Button.SetActive(true);
        game_Over_Panel.SetActive(false);
        paused_Menu.SetActive(false);


        yield return new WaitForSeconds(1f);
        bg_Selector.ChangeBG();
        score_text.text = "0";

        yield return new WaitForSeconds(1f);
        levelManager.score = 0;
        levelManager.time_Counter = levelManager.time_Setter;
    }

    // Main Menu Option

    IEnumerator skinMenuLoadCo()
    {
        yield return new WaitForSeconds(0.5f);
        mainMenu_Anim.SetBool("SkinMenuOn", true);
    }

    public void optionButton()
    {
        soundController(0, 1f);

        opt_Panel.SetActive(true);
        mainMenu_Anim.SetBool("OptMenuOn", true);

        StartCoroutine(skinMenuLoadCo());
    }

    IEnumerator skinButtonCo()
    {
        yield return new WaitForSeconds(0.5f);
        mainMenu_Anim.SetBool("SkinMenuOn", true);
    }

    public void skinButton()
    {
        soundController(0, 1f);

        mainMenu_Anim.SetBool("StatMenuOn", false);
        StartCoroutine(skinButtonCo());
    }

    public void choosenBallTick()
    {
        foreach(GameObject go in choosenBall) { go.SetActive(false); }

        int ball_look = PlayerPrefs.GetInt("ball_look", 0);

        switch (ball_look)
        {
            case 0:
                choosenBall[0].SetActive(true);
                break;

            case 1:
                choosenBall[1].SetActive(true);
                break;

            case 2:
                choosenBall[2].SetActive(true);
                break;

            case 3:
                choosenBall[3].SetActive(true);
                break;

            case 4:
                choosenBall[4].SetActive(true);
                break;

            case 5:
                choosenBall[5].SetActive(true);
                break;

            default:
                choosenBall[0].SetActive(true);
                break;

        }
    }

    public void orangeBallButton()
    {
        soundController(0, 1f);

        PlayerPrefs.SetInt("ball_look", 0);
        
        foreach(GameObject go in choosenBall)
        {
            go.SetActive(false);
        }

        choosenBall[0].SetActive(true);

        // Change the ball
        ball_Look_Change.BallSelection();
    }

    public void pinkBallButton()
    {
        soundController(0, 1f);
        PlayerPrefs.SetInt("ball_look", 1);

        foreach (GameObject go in choosenBall)
        {
            go.SetActive(false);
        }

        choosenBall[1].SetActive(true);

        // Change the ball
        ball_Look_Change.BallSelection();
    }

    public void redBallButton()
    {
        soundController(0, 1f);
        PlayerPrefs.SetInt("ball_look", 2);

        foreach (GameObject go in choosenBall)
        {
            go.SetActive(false);
        }

        choosenBall[2].SetActive(true);

        // Change the ball
        ball_Look_Change.BallSelection();
    }

    public void blueBallButton()
    {
        soundController(0, 1f);
        PlayerPrefs.SetInt("ball_look", 3);

        foreach (GameObject go in choosenBall)
        {
            go.SetActive(false);
        }

        choosenBall[3].SetActive(true);

        // Change the ball
        ball_Look_Change.BallSelection();
    }

    public void greenBallButton()
    {
        soundController(0, 1f);
        PlayerPrefs.SetInt("ball_look", 4);

        foreach (GameObject go in choosenBall)
        {
            go.SetActive(false);
        }

        choosenBall[4].SetActive(true);

        // Change the ball
        ball_Look_Change.BallSelection();
    }

    public void footBallButton()
    {
        soundController(0, 1f);
        PlayerPrefs.SetInt("ball_look", 5);

        foreach (GameObject go in choosenBall)
        {
            go.SetActive(false);
        }

        choosenBall[5].SetActive(true);

        // Change the ball
        ball_Look_Change.BallSelection();
    }

    IEnumerator statButtonCo()
    {
        yield return new WaitForSeconds(0.5f);
        mainMenu_Anim.SetBool("StatMenuOn", true);
    }

    public void stateMenuDetails()
    {
        state_Highest_Score.text = PlayerPrefs.GetInt("BestScore", 0).ToString();
        games_Played.text = PlayerPrefs.GetInt("TotalGames", 0).ToString();
        total_hoop_Count.text = PlayerPrefs.GetInt("totalHoops", 0).ToString();

        int avg_hoop = 0;

        try
        {
            avg_hoop = PlayerPrefs.GetInt("totalHoops", 0) / PlayerPrefs.GetInt("TotalGames", 0);
        }
        catch(DivideByZeroException e)
        {
            Debug.Log(e.ToString());
            avg_hoop = 0;
        }

        avg_Hoop_Count.text = avg_hoop.ToString();
    }

    public void statButton()
    {
        soundController(0, 1f);
        mainMenu_Anim.SetBool("SkinMenuOn", false);
        StartCoroutine(statButtonCo());
    }

    public void privacyPolicyButton()
    {
        soundController(0, 1f);

        Application.OpenURL(privacyPolicyUrl);
    }


    IEnumerator optionMenuExitCo()
    {
        mainMenu_Anim.SetBool("SkinMenuOn", false);
        mainMenu_Anim.SetBool("StatMenuOn", false);
        yield return new WaitForSeconds(0.5f);

        mainMenu_Anim.SetBool("OptMenuOn", false);
        yield return new WaitForSeconds(1f);
        opt_Panel.SetActive(false);
    }

    public void exitOption()
    {
        soundController(0, 1f);
        StartCoroutine(optionMenuExitCo());
    }

    private void soundController(int index, float vol)
    {
        if (AudioManager.instance.isSoundOn == 1) // If sound is On
        {
            AudioManager.instance.audioSources[index].Stop();

            AudioManager.instance.audioSources[index].pitch = UnityEngine.Random.Range(0.9f, 1f);
            AudioManager.instance.audioSources[index].volume = vol;
            AudioManager.instance.audioSources[index].Play();
        }
        else // If sound is Off
        {
            AudioManager.instance.audioSources[index].volume = 0f;
            AudioManager.instance.audioSources[index].Stop();
        }
    }

    private void soundIcon()
    {
        if (AudioManager.instance.isSoundOn == 1) // If sound is On
        {
            Sfx_Icon.sprite = on_Sfx_Icon;
        }
        else // If sound is Off
        {
            Sfx_Icon.sprite = off_Sfx_Icon;
        }
    }

    public void buttonClickSoundIcon()
    {
        soundController(0, 1f);

        if (AudioManager.instance.isSoundOn == 1) // If sound is On
        {
            // Turn Off Sound
            AudioManager.instance.music.Stop();

            PlayerPrefs.SetInt("Sound", 0);
            AudioManager.instance.isSoundOn = PlayerPrefs.GetInt("Sound", 0);

            Sfx_Icon.sprite = off_Sfx_Icon;
            in_game_Sfx_Icon.sprite = off_Sfx_Icon;
        }
        else // If sound is Off
        {
            // Turn On Sound
            AudioManager.instance.music.volume = 0.2f;
            AudioManager.instance.music.Play();

            PlayerPrefs.SetInt("Sound", 1);
            AudioManager.instance.isSoundOn = PlayerPrefs.GetInt("Sound", 1);

            Sfx_Icon.sprite = on_Sfx_Icon;
            in_game_Sfx_Icon.sprite = on_Sfx_Icon;
        }
    }
}
