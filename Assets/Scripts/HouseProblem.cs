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

        new HouseProblem(
    "My cat unplugged the router. Again.",
    new string[] { "Adopt a dog instead", "Plug it back in", "File a police report" },
    1,
    13f
),
        new HouseProblem(
    "My neighbor is stealing our Wi-Fi.",
    new string[] {  "Challenge them to a duel", "Unplug the internet forever", "Change the password" },
    2,
    14f
),
        new HouseProblem(
    "The router is making a weird beeping noise.",
    new string[] {  "Perform an exorcism", "Restart it", "Feed it a snack" },
    1,
    12f
),
        new HouseProblem(
    "The Wi-Fi signal disappears whenever it rains.",
    new string[] {  "Pray for sunshine", "Build an ark", "Move the router away from windows" },
    2,
    16f
),
    };
}