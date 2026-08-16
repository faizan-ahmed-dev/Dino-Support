using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Audio Clips")]
    public AudioClip ticketClickSound;
    public AudioClip enterDinoTransitionSound;
    public AudioClip returnToCitySound;
    public AudioClip strikeGainedSound;
    public AudioClip meterDangerSound;
    public AudioClip gameOverSound;
    public AudioClip winSound;

    private bool dangerSoundPlayed = false;

    [Header("Setup")]
    public HouseSlot[] houseSlots;
    public float shiftDuration = 90f;

    [Header("World Roots - drag the parent GameObjects here")]
    public GameObject cityWorldRoot;
    public GameObject dinoWorldRoot;
    public DinoRunManager dinoRunManager;

    [Header("Intro")]
    public GameObject introPanel;

    [Header("Player Avatar")]
    public PlayerAvatarUI playerAvatar;

    [Header("UI - City")]
    public TMP_Text shiftTimerText;
    public TMP_Text offlineMeterText;
    public UnityEngine.UI.Image offlineMeterBar;

    [Header("Offline Meter")]
    [Range(0f, 1f)] public float offlineMeter = 0.2f;
    public float meterGainOnFail = 0.18f;
    public float meterDropOnFix = 0.06f;
    public float meterDropAfterSurvivingDino = 0.5f;
    public float meterPenaltyAfterDinoDeath = 0.05f;

    [Header("Meter Colors")]
    public Color meterSafeColor = new Color(0.373f, 0.851f, 0.478f);
    public Color meterWarningColor = new Color(1f, 0.788f, 0.235f);
    public Color meterDangerColor = new Color(1f, 0.353f, 0.322f);

    [Header("Difficulty Scaling")]
    public float difficultyRampInterval = 20f;
    public float timeLimitMultiplier = 1f;
    public float timeLimitMultiplierStep = 0.9f;

    [Header("Travel / Prioritization")]
    public float travelDuration = 1.5f;
    public TMP_Text travelStatusText;

    [Header("Strikes")]
    public int maxStrikes = 3;
    public TMP_Text strikesText;
    public UnityEngine.UI.Image[] strikeIcons;

    [Header("Running Stats")]
    public TMP_Text statsText;

    [Header("Win/Lose Screens")]
    public GameObject winPanel;
    public GameObject losePanel;
    public TMP_Text summaryText;
    public TMP_Text loseSummaryText;

    private float shiftTimeRemaining;
    private float difficultyTimer;
    private bool gameOver = false;
    private bool inDinoMode = false;
    private bool shiftStarted = false;
    private bool isTraveling = false;
    private int customersServed = 0;
    private int customersFailed = 0;
    private int strikes = 0;
    public bool IsGamePaused => inDinoMode || gameOver || !shiftStarted;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        shiftTimeRemaining = shiftDuration;
        difficultyTimer = 0f;
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        dinoWorldRoot.SetActive(false);
        cityWorldRoot.SetActive(true);
        introPanel.SetActive(true);

        if (travelStatusText != null) travelStatusText.text = "";
        UpdateMeterUI();
        UpdateStrikesUI();
        UpdateStatsUI();
    }

    public void StartShift()
    {
        introPanel.SetActive(false);
        shiftStarted = true;
        shiftTimeRemaining = shiftDuration;

        foreach (var slot in houseSlots)
        {
            AssignNewProblem(slot);
        }
    }

    public void StartShift_Easy()
    {
        travelDuration = 0.5f;
        shiftDuration = 60f;
        StartShift();
    }

    public void StartShift_Medium()
    {
        travelDuration = 1.0f;
        shiftDuration = 90f;
        StartShift();
    }

    public void StartShift_Hard()
    {
        travelDuration = 1.5f;
        shiftDuration = 120f;
        StartShift();
    }

    void Update()
    {
        if (gameOver || inDinoMode || !shiftStarted) return;

        shiftTimeRemaining -= Time.deltaTime;
        int secondsLeft = Mathf.Max(0, Mathf.CeilToInt(shiftTimeRemaining));
        shiftTimerText.text = $"Shift ends in: {secondsLeft}s";

        difficultyTimer += Time.deltaTime;
        if (difficultyTimer >= difficultyRampInterval)
        {
            difficultyTimer = 0f;
            timeLimitMultiplier *= timeLimitMultiplierStep;
        }

        if (shiftTimeRemaining <= 0f)
        {
            WinShift();
        }
    }

    private void AssignNewProblem(HouseSlot slot)
    {
        var problem = ProblemDatabase.AllProblems[Random.Range(0, ProblemDatabase.AllProblems.Length)];
        string houseName = "House #" + Random.Range(1, 99);
        slot.Setup(houseName, problem, timeLimitMultiplier);
    }

    // ---- Travel / Prioritization ----

    public void RequestTravel(HouseSlot slot)
    {
        if (isTraveling || gameOver || inDinoMode || !shiftStarted) return;
        AudioManager.Instance.PlaySFX(ticketClickSound);
        StartCoroutine(TravelRoutine(slot));
    }

    private IEnumerator TravelRoutine(HouseSlot slot)
    {
        isTraveling = true;
        foreach (var s in houseSlots) s.SetInteractable(false);
        if (travelStatusText != null) travelStatusText.text = "On the way...";

        yield return new WaitForSeconds(travelDuration);

        isTraveling = false;
        foreach (var s in houseSlots) s.SetInteractable(true);
        if (travelStatusText != null) travelStatusText.text = "";

        if (gameOver || inDinoMode) yield break; // got pulled into the Dino World mid-travel - don't open a popup in the background

        slot.ShowPopupNow();
    }

    public void OnHouseResolved(HouseSlot slot, bool wasCorrect)
    {
        if (gameOver || inDinoMode) return;

        if (wasCorrect)
        {
            customersServed++;
            ChangeMeter(-meterDropOnFix);
        }
        else
        {
            customersFailed++;
            ChangeMeter(meterGainOnFail);
        }

        UpdateStatsUI();
        RefreshSlot(slot);
    }

    public void OnHouseTimedOut(HouseSlot slot)
    {
        if (gameOver || inDinoMode) return;

        customersFailed++;
        ChangeMeter(meterGainOnFail);
        UpdateStatsUI();
        RefreshSlot(slot);
    }

    private void RefreshSlot(HouseSlot slot)
    {
        if (gameOver || inDinoMode) return;
        AssignNewProblem(slot);
    }

    private void ChangeMeter(float amount)
    {
        offlineMeter = Mathf.Clamp01(offlineMeter + amount);
        UpdateMeterUI();

        if (offlineMeter >= 1f)
        {
            EnterDinoMode();
        }
    }

    private void UpdateMeterUI()
    {
        offlineMeterBar.fillAmount = offlineMeter;

        if (offlineMeter < 0.6f)
            offlineMeterBar.color = Color.Lerp(meterSafeColor, meterWarningColor, offlineMeter / 0.6f);
        else
            offlineMeterBar.color = Color.Lerp(meterWarningColor, meterDangerColor, (offlineMeter - 0.6f) / 0.4f);

        offlineMeterText.text = $"Offline Meter: {Mathf.RoundToInt(offlineMeter * 100)}%";

        if (playerAvatar != null) playerAvatar.UpdateAvatar(offlineMeter);

        if (offlineMeter >= 0.6f && !dangerSoundPlayed)
        {
            AudioManager.Instance.PlaySFX(meterDangerSound);
            dangerSoundPlayed = true;
        }
        else if (offlineMeter < 0.6f)
        {
            dangerSoundPlayed = false;
        }
    }

    private void UpdateStrikesUI()
    {
        if (strikesText != null) strikesText.text = $"Strikes: {strikes}/{maxStrikes}";

        if (strikeIcons != null)
        {
            for (int i = 0; i < strikeIcons.Length; i++)
            {
                if (strikeIcons[i] == null) continue;
                strikeIcons[i].color = i < strikes ? meterDangerColor : new Color(1f, 1f, 1f, 0.4f);
            }
        }
    }

    private void UpdateStatsUI()
    {
        if (statsText != null) statsText.text = $"Served: {customersServed}\nFailed: {customersFailed}";
    }

    // ---- Switching between City and Dino World ----

    private void EnterDinoMode()
    {
        if (inDinoMode) return;
        AudioManager.Instance.PlaySFX(enterDinoTransitionSound);
        AudioManager.Instance.PlayDinoMusic();
        inDinoMode = true;
        cityWorldRoot.SetActive(false);
        dinoWorldRoot.SetActive(true);
        dinoRunManager.BeginRun();
    }

    // Called by DinoRunManager when the run ends. timeSurvivedThisRun/surviveDuration
    // let us give continuous partial credit instead of a flat survive-vs-die bonus.
    public void ExitDinoMode(bool survived, float timeSurvivedThisRun, float surviveDuration)
    {
        inDinoMode = false;
        dinoWorldRoot.SetActive(false);
        cityWorldRoot.SetActive(true);
        AudioManager.Instance.PlaySFX(returnToCitySound);
        AudioManager.Instance.PlayCityMusic();

        if (survived)
        {
            offlineMeter = Mathf.Clamp01(offlineMeter - meterDropAfterSurvivingDino);
            UpdateMeterUI();
            return;
        }

        strikes++;
        AudioManager.Instance.PlaySFX(strikeGainedSound);

        UpdateStrikesUI();

        if (strikes >= maxStrikes)
        {
            LoseGame();
            return;
        }

        float survivalFraction = Mathf.Clamp01(timeSurvivedThisRun / surviveDuration);
        float reduction = Mathf.Lerp(meterPenaltyAfterDinoDeath, meterDropAfterSurvivingDino, survivalFraction);
        offlineMeter = Mathf.Clamp01(offlineMeter - reduction);
        UpdateMeterUI();
    }

    private void LoseGame()
    {
        gameOver = true;
        AudioManager.Instance.PlaySFX(gameOverSound);
        losePanel.SetActive(true);

        if (loseSummaryText != null)
        {
            loseSummaryText.text =
                "Three strikes. You're the dino now. Forever.\n\n" +
                $"Customers Served: {customersServed}\n" +
                $"Customers Failed: {customersFailed}";
        }
    }

    private string CalculateGrade()
    {
        int total = customersServed + customersFailed;
        float ratio = total > 0 ? (float)customersServed / total : 1f;

        if (strikes == 0 && ratio >= 0.85f) return "S";
        if (ratio >= 0.7f) return "A";
        if (ratio >= 0.5f) return "B";
        if (ratio >= 0.3f) return "C";
        return "F";
    }

    private void WinShift()
    {
        gameOver = true;
        AudioManager.Instance.PlaySFX(winSound);
        winPanel.SetActive(true);
        string grade = CalculateGrade();
        summaryText.text =
            $"Grade: {grade}\n\n" +
            $"Customers Served: {customersServed}\n" +
            $"Customers Failed: {customersFailed}\n" +
            $"Strikes: {strikes}/{maxStrikes}\n" +
            $"Final Offline Meter: {Mathf.RoundToInt(offlineMeter * 100)}%\n\n" +
            "Congratulations! You kept the internet running.\n\n" +
            "...\n\nThe internet goes down anyway.";
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}