using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HouseSlot : MonoBehaviour
{
    [Header("UI References - drag these in from the Inspector")]
    public TMP_Text houseNameText;
    public Image timerFillBar;
    public GameObject problemPopup;
    public TMP_Text problemText;
    public Button[] fixButtons;
    public TMP_Text[] fixButtonLabels;
    public Button selfButton; // drag this ticket's OWN Button component in here
    public AudioClip correctFixSound;
    public AudioClip wrongFixSound;

    private HouseProblem currentProblem;
    private float timeRemaining;
    private bool isActive = false;
    private bool isSolved = false;

    public void Setup(string houseName, HouseProblem problem, float timeLimitMultiplier = 1f)
    {
        houseNameText.text = houseName;
        currentProblem = problem;
        timeRemaining = problem.timeLimit * timeLimitMultiplier;
        isActive = true;
        isSolved = false;
        problemPopup.SetActive(false);
        timerFillBar.fillAmount = 1f;
        timerFillBar.color = Color.green;
    }

    void Update()
    {
        Debug.Log("IsGamePaused: " + (GameManager.Instance != null ? GameManager.Instance.IsGamePaused.ToString() : "NULL INSTANCE"));

        if (!isActive || isSolved) return;
        if (GameManager.Instance != null && GameManager.Instance.IsGamePaused) return;


        timeRemaining -= Time.deltaTime;
        timerFillBar.fillAmount = timeRemaining / currentProblem.timeLimit;

        if (timerFillBar.fillAmount < 0.3f)
            timerFillBar.color = Color.red;
        else if (timerFillBar.fillAmount < 0.6f)
            timerFillBar.color = Color.yellow;

        if (timeRemaining <= 0f)
        {
            isActive = false;
            GameManager.Instance.OnHouseTimedOut(this);
        }
    }

    // Now requests a travel delay instead of opening the popup immediately -
    // this is what makes clicking a house a real decision instead of a free action.
    public void OnHouseClicked()
    {
        if (!isActive || isSolved) return;
        GameManager.Instance.RequestTravel(this);
    }

    // Called by GameManager once the travel delay finishes
    public void ShowPopupNow()
    {
        if (!isActive || isSolved) return;

        problemPopup.SetActive(true);
        problemText.text = currentProblem.problemText;

        for (int i = 0; i < fixButtons.Length; i++)
        {
            int choiceIndex = i;
            bool hasOption = i < currentProblem.fixOptions.Length;

            fixButtons[i].gameObject.SetActive(hasOption);
            if (!hasOption) continue;

            fixButtonLabels[i].text = currentProblem.fixOptions[i];

            fixButtons[i].onClick.RemoveAllListeners();
            fixButtons[i].onClick.AddListener(() => OnFixChosen(choiceIndex));
        }
    }

    private void OnFixChosen(int choiceIndex)
    {
        problemPopup.SetActive(false);
        isSolved = true;
        isActive = false;

        bool wasCorrect = (choiceIndex == currentProblem.correctFixIndex);
        AudioManager.Instance.PlaySFX(wasCorrect ? correctFixSound : wrongFixSound);
        GameManager.Instance.OnHouseResolved(this, wasCorrect);
    }

    public void SetInteractable(bool value)
    {
        if (selfButton != null) selfButton.interactable = value;
    }

    public void ClearSlot()
    {
        isActive = false;
        isSolved = false;
        houseNameText.text = "";
        problemPopup.SetActive(false);
    }
}