using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardTopDownComponent : MonoBehaviour
{
    public PollTextComponent RankText;
    public PollTextComponent ScoreText;
    public PollTextComponent NameText;
    public PollTextComponent TotalTimeText;

    public PollTextComponent NameTextPrefab;
    public PollTextComponent RankTextPrefab;
    public PollTextComponent SocreTextPrefab;
    public PollTextComponent TotalTimeTextPrefab;
    public PollTextComponent TopScoreTextPrefab;

    private Dictionary<string, List<PollTextComponent>> LeaderboardEntries;

    private float LargeVerticalSpacing = 4.0f;
    private float SmallVerticalSpacing = 2.6f;
    private float LargeVerticalOffset = 2.8f;
    private float SmallVerticalOffset = 3.25f;

    private List<LeaderboardPlayerData> PlayerData;
    public void SetPlayerData(List<LeaderboardPlayerData> playerData)
    {
        PlayerData = playerData;
    }

    public void CreateObjects()
    {
        LeaderboardEntries = new Dictionary<string, List<PollTextComponent>>();
        for (var i = 0; i < 10; i++)
        {
            if (i < 3)
            {
                var rankTextInstance = Instantiate(TopScoreTextPrefab).GetComponent<PollTextComponent>();
                rankTextInstance.transform.SetParent(transform);
                rankTextInstance.transform.position = RankText.transform.position - new Vector3(0, -5 + ((i + 1) * LargeVerticalSpacing) + LargeVerticalOffset, 0);
                rankTextInstance.SetTextData((i + 1).ToString());
                rankTextInstance.CreateAllObjects();
            }
            else
            {
                var rankTextInstance = Instantiate(SocreTextPrefab).GetComponent<PollTextComponent>();
                rankTextInstance.transform.SetParent(transform);
                rankTextInstance.transform.position = RankText.transform.position - new Vector3(0, ((i + 1) * SmallVerticalSpacing) + SmallVerticalOffset, 0);
                rankTextInstance.SetTextData((i + 1).ToString());
                rankTextInstance.CreateAllObjects();
            }
        }
        for (var i = 0; i < PlayerData.Count; i++)
        {
            if (i < 3)
            {
                var scoreTextInstance = Instantiate(TopScoreTextPrefab).GetComponent<PollTextComponent>();
                scoreTextInstance.transform.SetParent(transform);
                scoreTextInstance.transform.position = ScoreText.transform.position - new Vector3(0, -5 + ((i + 1) * LargeVerticalSpacing) + LargeVerticalOffset, 0);
                scoreTextInstance.SetTextData(PlayerData[i].PlayerScore.ToString());
                scoreTextInstance.CreateAllObjects();

                var nameTextInstance = Instantiate(TopScoreTextPrefab).GetComponent<PollTextComponent>();
                nameTextInstance.transform.SetParent(transform);
                nameTextInstance.transform.position = NameText.transform.position - new Vector3(0, -5 + ((i + 1) * LargeVerticalSpacing) + LargeVerticalOffset, 0);
                nameTextInstance.SetTextData(PlayerData[i].PlayerDisplayName);
                nameTextInstance.CreateAllObjects();

                var totalTimeTextInstance = Instantiate(TopScoreTextPrefab).GetComponent<PollTextComponent>();
                totalTimeTextInstance.transform.SetParent(transform);
                totalTimeTextInstance.transform.position = TotalTimeText.transform.position - new Vector3(3, -5 + ((i + 1) * LargeVerticalSpacing) + LargeVerticalOffset, 0);
                totalTimeTextInstance.SetTextData(string.Format("{0:N0}:{1:N0}:{2:N0}", PlayerData[i].TotalTime.Hours, PlayerData[i].TotalTime.Minutes, PlayerData[i].TotalTime.Seconds));
                totalTimeTextInstance.CreateAllObjects();

                LeaderboardEntries.Add(PlayerData[i].PlayerBaseName, new List<PollTextComponent>
                {
                    scoreTextInstance,
                    nameTextInstance,
                    totalTimeTextInstance
                });
            }
            else
            {
                var scoreTextInstance = Instantiate(SocreTextPrefab).GetComponent<PollTextComponent>();
                scoreTextInstance.transform.SetParent(transform);
                scoreTextInstance.transform.position = ScoreText.transform.position - new Vector3(0, ((i + 1) * SmallVerticalSpacing) + SmallVerticalOffset, 0);
                scoreTextInstance.SetTextData(PlayerData[i].PlayerScore.ToString());
                scoreTextInstance.CreateAllObjects();

                var nameTextInstance = Instantiate(NameTextPrefab).GetComponent<PollTextComponent>();
                nameTextInstance.transform.SetParent(transform);
                nameTextInstance.transform.position = NameText.transform.position - new Vector3(0, ((i + 1) * SmallVerticalSpacing) + SmallVerticalOffset, 0);
                nameTextInstance.SetTextData(PlayerData[i].PlayerDisplayName);
                nameTextInstance.CreateAllObjects();

                var totalTimeTextInstance = Instantiate(TotalTimeTextPrefab).GetComponent<PollTextComponent>();
                totalTimeTextInstance.transform.SetParent(transform);
                totalTimeTextInstance.transform.position = TotalTimeText.transform.position - new Vector3(0, ((i + 1) * SmallVerticalSpacing) + SmallVerticalOffset, 0);
                totalTimeTextInstance.SetTextData(string.Format("{0:N0}:{1:N0}:{2:N0}", PlayerData[i].TotalTime.Hours, PlayerData[i].TotalTime.Minutes, PlayerData[i].TotalTime.Seconds));
                totalTimeTextInstance.CreateAllObjects();

                LeaderboardEntries.Add(PlayerData[i].PlayerBaseName, new List<PollTextComponent>
                {
                    scoreTextInstance,
                    nameTextInstance,
                    totalTimeTextInstance
                });
            }
        }
    }
}
