using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Attach to an empty GameObject. Swaps a static sprite and flavor text based on
// how high the Offline Meter is. Nothing here is animated - sprite and text just
// change instantly, same as any other UI state change (like the timer bar's color).
public class PlayerAvatarUI : MonoBehaviour
{
    [System.Serializable]
    public class AvatarTier
    {
        public Sprite sprite;
        [TextArea] public string[] lines;
    }

    public Image avatarImage;
    public Image glowBorder;      // optional - leave empty in Inspector to skip
    public TMP_Text flavorText;

    [Header("Tiers - index 0 = calm (low meter) ... last index = panicked (high meter)")]
    public AvatarTier[] tiers;

    [Header("How often the flavor line changes while sitting in the same tier")]
    public float lineChangeInterval = 4f;

    private int currentTierIndex = -1;
    private float lineChangeTimer;

    void Update()
    {
        if (currentTierIndex < 0) 
            return;

        lineChangeTimer += Time.deltaTime;
        if (lineChangeTimer >= lineChangeInterval)
        {
            lineChangeTimer = 0f;
            PickRandomLine(currentTierIndex);
        }
    }

    // Called by GameManager every time the Offline Meter value changes
    public void UpdateAvatar(float meterValue)
    {
        if (tiers.Length == 0) 
            return;

        int tierIndex = Mathf.Clamp(
            Mathf.FloorToInt(meterValue * tiers.Length),
            0,
            tiers.Length - 1
        );

        if (glowBorder != null)
        {
            glowBorder.color = Color.Lerp(Color.green, Color.red, meterValue);
        }

        if (tierIndex != currentTierIndex)
        {
            currentTierIndex = tierIndex;
            avatarImage.sprite = tiers[tierIndex].sprite;
            PickRandomLine(tierIndex);
            lineChangeTimer = 0f;
        }
    }

    private void PickRandomLine(int tierIndex)
    {
        var lines = tiers[tierIndex].lines;
        if (lines.Length == 0) 
            return;

        flavorText.text = lines[Random.Range(0, lines.Length)];
    }
}