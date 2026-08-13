using UnityEngine;

[System.Serializable]
public class HouseProblem
{
    public string problemText;
    public string[] fixOptions;
    public int correctFixIndex;
    public float timeLimit = 15f;

    public HouseProblem(string text, string[] options, int correctIndex, float time)
    {
        problemText = text;
        fixOptions = options;
        correctFixIndex = correctIndex;
        timeLimit = time;
    }
}

public static class ProblemDatabase
{
    public static HouseProblem[] AllProblems = new HouseProblem[]
    {
        new HouseProblem(
            "My Wi-Fi isn't working at all!",
            new string[] { "Restart the router", "Yell at the modem", "Unplug the TV" },
            0,
            15f
        ),
        new HouseProblem(
            "The internet is SO slow today.",
            new string[] { "Move the router closer", "Throw the router away", "Restart the router" },
            2,
            18f
        ),
        new HouseProblem(
            "My Wi-Fi just... disappeared?",
            new string[] { "Buy a new laptop", "Turn Wi-Fi back on in settings", "Call a plumber" },
            1,
            12f
        ),
        new HouseProblem(
            "Internet doesn't work upstairs.",
            new string[] { "Move the router upstairs", "Scream louder at the router", "Get a new ISP" },
            0,
            16f
        ),
    };
}