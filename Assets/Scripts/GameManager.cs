using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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

    public Color meterSafeColor = new Color(0.373f, 0.851f, 0.478f);    // #5FD97A
    public Color meterWarningColor = new Color(1f, 0.788f, 0.235f);     // #FFC93C
    public Color meterDangerColor = new Color(1f, 0.353f, 0.322f);      // #FF5A52

    [Header("Difficulty Scaling")]
    public float difficultyRampInterval = 20f;
    public float timeLimitMultiplier = 1f;
    public float timeLimitMultiplierStep = 0.9f;

    [Header("Win/Lose Screens")]
    public GameObject winPanel;
    public GameObject losePanel;
    public TMP_Text summaryText;

    private float shiftTimeRemaining;
    private float difficultyTimer;
    private bool gameOver = false;
    private bool inDinoMode = false;
    private bool shiftStarted = false;
    private int customersServed = 0;
    private int customersFailed = 0;

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

        UpdateMeterUI();
    }

    public void StartShift()
    {
        introPanel.SetActive(false);
        shiftStarted = true;

        foreach (var slot in houseSlots)
        {
            AssignNewProblem(slot);
        }
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

        RefreshSlot(slot);
    }

    public void OnHouseTimedOut(HouseSlot slot)
    {
        if (gameOver || inDinoMode) return;

        customersFailed++;
        ChangeMeter(meterGainOnFail);
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
        {
            offlineMeterBar.color = Color.Lerp(meterSafeColor, meterWarningColor, offlineMeter / 0.6f);
        }
        else
        {
            offlineMeterBar.color = Color.Lerp(meterWarningColor, meterDangerColor, (offlineMeter - 0.6f) / 0.4f);
        }

        offlineMeterText.text = $"Offline Meter: {Mathf.RoundToInt(offlineMeter * 100)}%";

        if (playerAvatar != null)
        {
            playerAvatar.UpdateAvatar(offlineMeter);
        }
    }

    private void EnterDinoMode()
    {
        inDinoMode = true;
        cityWorldRoot.SetActive(false);
        dinoWorldRoot.SetActive(true);
        dinoRunManager.BeginRun();
    }

    public void ExitDinoMode(bool survived)
    {
        inDinoMode = false;
        dinoWorldRoot.SetActive(false);
        cityWorldRoot.SetActive(true);

        if (survived)
        {
            offlineMeter = Mathf.Clamp01(offlineMeter - meterDropAfterSurvivingDino);
        }
        else
        {
            offlineMeter = Mathf.Clamp01(offlineMeter - meterPenaltyAfterDinoDeath);
        }
        UpdateMeterUI();
    }

    private void WinShift()
    {
        gameOver = true;
        winPanel.SetActive(true);
        summaryText.text =
            $"Customers Served: {customersServed}\n" +
            $"Customers Failed: {customersFailed}\n" +
            $"Final Offline Meter: {Mathf.RoundToInt(offlineMeter * 100)}%\n\n" +
            "Congratulations! You kept the internet running.\n\n" +
            "...\n\nThe internet goes down anyway.";
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}