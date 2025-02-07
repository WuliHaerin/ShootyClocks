using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SgLib;
using TMPro;

#if EASY_MOBILE
using EasyMobile;
#endif

public class GameplayUI : MonoBehaviour
{
    [HideInInspector]
    public bool stopCountTime;
    [HideInInspector]
    public bool isCompleteWithStar;

    [Header("Object References")]
    public GameManager gameManager;
    public GameObject gameButtons;
    public Button btnShoot;
    public Button btnRetry;
    public Button btnNext;
    public Button btnRetryForStar;
    public Button btnRetryLastShoot;
    //public Button btnShare;
    public Button btnBack;
    public Image imgStar;
    public Image imgCrown;
    public Text txtGetStarTime;
    public Text txtBestScore;
    public Text txtScore;
    public Text txtLevel;
    public TMP_Text txtFinalScoreLabel;
    public Text txtFinalScoreValue;
    public TMP_Text txtLevelComplete;
    public TMP_Text txtRandomText;

    [Header("Sharing-Specific")]
    public GameObject shareUI;
    public Image sharedImage;

    private float getStarTime;
    private bool stopCheck;

    #if EASY_MOBILE
    void OnEnable()
    {
        Advertising.RewardedAdCompleted += Advertising_RewardedAdCompleted;
    }

    void OnDisable()
    {
        Advertising.RewardedAdCompleted -= Advertising_RewardedAdCompleted;
    }
    #endif

    // Use this for initialization
    void Start()
    {
        if (gameManager.gameEnvironment == GameEnvironment.LEVEL_EDITOR)
        {
            StartCoroutine(CountDownGetStarTime());
            btnRetry.gameObject.SetActive(false);
            return;
        }
           
        if (GameManager.gameMode == GameMode.Quest) //On level mode
        {
            //Hide crown image, best score text and score text
            imgCrown.gameObject.SetActive(false);
            txtBestScore.gameObject.SetActive(false);
            txtScore.gameObject.SetActive(false);

            imgStar.gameObject.SetActive(true);
            txtGetStarTime.gameObject.SetActive(true);
            txtLevel.gameObject.SetActive(true);

            //Show level and start count down 
            txtLevel.text = GameManager.levelLoaded.ToString();
            StartCoroutine(CountDownGetStarTime());
        }
        else if (GameManager.gameMode == GameMode.Endless) //On survival mode
        {
            //Show crown image, best score tex and score text
            imgCrown.gameObject.SetActive(true);
            txtBestScore.gameObject.SetActive(true);
            txtScore.gameObject.SetActive(true);

            imgStar.gameObject.SetActive(false);
            txtGetStarTime.gameObject.SetActive(false);
            txtLevel.gameObject.SetActive(false);

            //Hide level text, get start time text and star image
            txtLevel.gameObject.SetActive(false);
            txtGetStarTime.gameObject.SetActive(false);
            imgStar.gameObject.SetActive(false);
       
        }

        gameButtons.SetActive(true);

        //Hide the setting panel, button next, button retry, button retry for star,
        //button setting and the text when game over or pass level
        txtFinalScoreLabel.gameObject.SetActive(false);
        txtFinalScoreValue.gameObject.SetActive(false);
        txtLevelComplete.gameObject.SetActive(false);
        txtRandomText.gameObject.SetActive(false);

        btnNext.gameObject.SetActive(false);
        btnRetry.gameObject.SetActive(false);
        btnRetryForStar.gameObject.SetActive(false);
        btnRetryLastShoot.gameObject.SetActive(false);
        //btnShare.gameObject.SetActive(false);

        shareUI.SetActive(false);
    }
	
    // Update is called once per frame
    void Update()
    {
        if (gameManager.gameEnvironment == GameEnvironment.LEVEL_EDITOR)
        {
            if (gameManager.gameOver || gameManager.passLevel)
            {
                stopCountTime = true;
                btnShoot.gameObject.SetActive(false);
                btnRetry.gameObject.SetActive(true);
            }
            return;
        }
            
        //Show score, best score and update music, sound
        txtScore.text = ScoreManager.Instance.Score.ToString();
        txtBestScore.text = ScoreManager.Instance.HighScore.ToString();

        if (gameManager.gameOver && !stopCheck) //Game over
        {
            stopCheck = true;
            stopCountTime = true;

            //Show the text
            txtLevel.gameObject.SetActive(false);
            txtScore.gameObject.SetActive(false);

            if (GameManager.gameMode == GameMode.Endless)
            {
                txtRandomText.gameObject.SetActive(false);
                txtLevelComplete.gameObject.SetActive(false);
                txtFinalScoreLabel.gameObject.SetActive(true);
                txtFinalScoreValue.text = ScoreManager.Instance.Score.ToString();
                txtFinalScoreValue.gameObject.SetActive(true);

                // Only show share button at end of survival mode or when pass a level
                if (gameManager.enablePremiumFeatures)
                {
                    //btnShare.gameObject.SetActive(true);
                }
            }
            else if (GameManager.gameMode == GameMode.Quest)
            {
                txtFinalScoreLabel.gameObject.SetActive(false);
                txtFinalScoreValue.gameObject.SetActive(false);
                txtRandomText.text = gameManager.gameOverTexts[Random.Range(0, gameManager.gameOverTexts.Length - 1)];
                txtLevelComplete.gameObject.SetActive(false);

                // Won't show share button if fail a level
                //btnShare.gameObject.SetActive(false);
            }      

            //Hide buttons
            btnShoot.gameObject.SetActive(false);
            btnNext.gameObject.SetActive(false);

            //Show buttons
            btnRetry.gameObject.SetActive(true);
            btnRetryLastShoot.gameObject.SetActive(true);

            AdManager.ShowInterstitialAd("1lcaf5895d5l1293dc",
    () => {
        Debug.Log("--插屏广告完成--");

    },
    (it, str) => {
        Debug.LogError("Error->" + str);
    });


        }
        else if (gameManager.passLevel && !stopCheck) //Pass level
        {
            stopCheck = true;

            txtLevel.gameObject.SetActive(false);
            txtScore.gameObject.SetActive(false);

            //Random text
            if (isCompleteWithStar)
                txtRandomText.text = gameManager.passLevelWithStarTexts[Random.Range(0, gameManager.passLevelWithStarTexts.Length - 1)];
            else
                txtRandomText.text = gameManager.passLevelTexts[Random.Range(0, gameManager.passLevelTexts.Length - 1)];

            txtLevelComplete.text = " 关卡 " + GameManager.levelLoaded.ToString() + " 完成!!!";

            //Show text when level passed
            txtLevelComplete.gameObject.SetActive(true);
            txtRandomText.gameObject.SetActive(true);
            txtFinalScoreLabel.gameObject.SetActive(false);
            txtFinalScoreValue.gameObject.SetActive(false);

            //Hide buttons
            btnShoot.gameObject.SetActive(false);
            btnRetry.gameObject.SetActive(false);
            btnRetryLastShoot.gameObject.SetActive(false);

            //Show buttons
            btnNext.gameObject.SetActive(true);

            if (gameManager.enablePremiumFeatures)
            {
                //btnShare.gameObject.SetActive(true);
            }

            //Is the level completed without star -> show the retry for star button
            if (!isCompleteWithStar)
                btnRetryForStar.gameObject.SetActive(true);
            AdManager.ShowInterstitialAd("1lcaf5895d5l1293dc",
    () => {
        Debug.Log("--插屏广告完成--");

    },
    (it, str) => {
        Debug.LogError("Error->" + str);
    });
        }
    }

    IEnumerator CountDownGetStarTime()
    {
        //Wait until gameManager load get star time
        while (getStarTime == 0)
        {
            getStarTime = gameManager.getStarTime;
            yield return null;
        }
  
        while (getStarTime > 0)
        {
            getStarTime = getStarTime - 1;
            float milisecond;
            float starMilisecond = 99;
            float endMilisecond = 0;
            float t = 0;
            while (t < 1f)
            {
                if (stopCountTime)
                    yield break;

                t += Time.deltaTime;
                float fraction = t / 1f;
                milisecond = (int)Mathf.Lerp(starMilisecond, endMilisecond, fraction);
                txtGetStarTime.text = getStarTime.ToString() + "." + milisecond.ToString(); 
                yield return null;
            }
        }
        stopCountTime = true;
        yield return new WaitForSeconds(0.5f);
        txtGetStarTime.gameObject.SetActive(false);
        imgStar.gameObject.SetActive(false);
    }


    public void HandleBackButton()
    {
        SceneManager.LoadScene("FirstScene"); 
    }


    void RetryLastShoot()
    {
        btnShoot.gameObject.SetActive(true);
        btnRetry.gameObject.SetActive(false);
        btnRetryLastShoot.gameObject.SetActive(false);
        //btnShare.gameObject.SetActive(false);

        txtRandomText.gameObject.SetActive(false);
        txtLevelComplete.gameObject.SetActive(false);
        txtFinalScoreLabel.gameObject.SetActive(false);
        txtFinalScoreValue.gameObject.SetActive(false);

        if (GameManager.gameMode == GameMode.Endless)
        {
            txtScore.gameObject.SetActive(true);
            txtLevel.gameObject.SetActive(false);
        }
        else if (GameManager.gameMode == GameMode.Quest)
        {
            txtLevel.gameObject.SetActive(true);
            txtScore.gameObject.SetActive(false);
        }

        gameManager.gameOver = false;
        gameManager.passLevel = false;
        stopCheck = false;
        stopCountTime = false;
        gameManager.stopCheck = false;


        foreach (ClockData a in gameManager.listClocksData)
        {
            gameManager.CreateClockByClockData(a);
        }

        gameManager.getStarTime = Mathf.Ceil(float.Parse(txtGetStarTime.text));

        if (GameManager.gameMode == GameMode.Quest)
            StartCoroutine(CountDownGetStarTime());

        gameManager.listClocksData.Clear();
    }

    public void HandleNextButton()
    {
        GameManager.levelLoaded++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void HandleRetryButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowShareUI()
    {

    }

    public void HideShareUI()
    {
        shareUI.SetActive(false);
    }

    public void ShareScreenshot()
    {

    }

    public void ShowRewardedAd()
    {
        AdManager.ShowVideoAd("192if3b93qo6991ed0",
            (bol) => {
                if (bol)
                {
                    RetryLastShoot();

                    AdManager.clickid = "";
                    AdManager.getClickid();
                    AdManager.apiSend("game_addiction", AdManager.clickid);
                    AdManager.apiSend("lt_roi", AdManager.clickid);


                }
                else
                {
                    StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
                }
            },
            (it, str) => {
                Debug.LogError("Error->" + str);
                //AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
            });

    }

}
