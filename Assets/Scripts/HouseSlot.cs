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
        if (!isActive || isSolved) return;

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

    public void OnHouseClicked()
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
        GameManager.Instance.OnHouseResolved(this, wasCorrect);
    }

    public void ClearSlot()
    {
        isActive = false;
        isSolved = false;
        houseNameText.text = "";
        problemPopup.SetActive(false);
    }
}