using UnityEngine;
using TMPro;

public class DinoRunManager : MonoBehaviour
{
    public static DinoRunManager Instance;

    [Header("Setup")]
    public DinoController dino;
    public ObstacleSpawner spawner;
    public Vector3 dinoStartPosition;

    [Header("Session")]
    public float surviveDuration = 12f;

    [Header("UI")]
    public TMP_Text dinoStatusText;
    public TMP_Text survivalTimerText;

    private float timeSurvived;
    private bool runActive = false;
    private bool pendingResult;

    void Awake()
    {
        Instance = this;
    }

    public void BeginRun()
    {
        timeSurvived = 0f;
        runActive = true;
        dino.ResetDino(dinoStartPosition);
        dino.UnfreezeDino();
        spawner.StartSpawning();
        dinoStatusText.text = "NO INTERNET";
    }

    void Update()
    {
        if (!runActive) return;

        timeSurvived += Time.deltaTime;
        float remaining = Mathf.Max(0f, surviveDuration - timeSurvived);
        survivalTimerText.text = $"Survive: {remaining:F1}s";

        if (timeSurvived >= surviveDuration)
        {
            EndRun(survived: true);
        }
    }

    public void OnDinoHit()
    {
        if (!runActive) return;
        EndRun(survived: false);
    }

    private void EndRun(bool survived)
    {
        runActive = false;
        spawner.StopSpawning();
        dino.FreezeDino();

        dinoStatusText.text = survived
            ? "Connection restored. Back to work."
            : "You got got. Back to the grind.";

        Invoke(nameof(ReturnToCity), 1.5f);
        pendingResult = survived;
    }

    private void ReturnToCity()
    {
        GameManager.Instance.ExitDinoMode(pendingResult);
    }
}