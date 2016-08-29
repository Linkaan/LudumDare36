using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class ScoreManager : MonoBehaviour {

	public InputField input;
    string value;
    Dictionary<string, UserScore> userScores;
    dreamloLeaderBoard dl;
	bool isAlive;

    public enum leaderboardState
    {
        waiting,
        leaderboard,
        done
    };

    public volatile leaderboardState ls;

    public struct UserScore {
        public Dictionary<string, int> scores;
        public string name;
    }

    void Start()
    {
        this.dl = dreamloLeaderBoard.GetSceneDreamloLeaderboard();
        this.ls = leaderboardState.waiting;
		isAlive = true;
    }

    void Update()
    {
        if (ls == leaderboardState.leaderboard)
        {
            List<dreamloLeaderBoard.Score> scoreList = dl.ToListHighToLow();
            if (scoreList != null)
            {
                foreach (dreamloLeaderBoard.Score currentScore in scoreList)
                {
                    SetScore(currentScore.playerName, currentScore.playerName, "score", currentScore.score);
                }               
                ls = leaderboardState.done;
				if (!isAlive) { // change this to display scores instead
					LoadScene (0);
				}
            }
        }
    }
    
    public int GetScore(string userid, string scoreType)
    {
        if (userScores == null) userScores = new Dictionary<string, UserScore>();

        if (!userScores.ContainsKey(userid))
        {
            return 0;
        }
        if (!userScores[userid].scores.ContainsKey(scoreType))
        {
            return 0;
        }
        return userScores[userid].scores[scoreType];
    }

    public void SetScore(string userid, string username, string scoreType, int value)
    {
        if (userScores == null) userScores = new Dictionary<string, UserScore>();
        if (!userScores.ContainsKey(userid))
        {
            userScores[userid] = new UserScore() { name = username, scores = new Dictionary<string, int>() };
        }
        userScores[userid].scores[scoreType] = value;
    }

    public string GetUserName(string userid)
    {
        return userScores[userid].name;
    }

    public string[] GetUserIDs(string sortingType)
    {
        if (userScores == null) userScores = new Dictionary<string, UserScore>();

        return userScores.Keys.OrderByDescending(n => GetScore(n, sortingType)).ToArray();
    }

    public void LoadScene(int scene)
    {
        Application.LoadLevel(scene);
    }

    public void LoadLeaderboard()
    {
        dl.LoadScores();
        ls = leaderboardState.leaderboard;
    }

    public void SubmitScore()
    {
		int score = (int)GameObject.FindObjectOfType<proceduralRoadGenerator>().getScore();
		value = input.text;
		if (value.Length == 0)
			return;
        dl.AddScore(value, score);
        ls = leaderboardState.leaderboard;
		isAlive = false;
		Debug.Log ("Submitted score as " + value);
    }
}
