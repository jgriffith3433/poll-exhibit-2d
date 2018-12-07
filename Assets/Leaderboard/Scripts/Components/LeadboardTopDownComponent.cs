using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeadboardTopDownComponent : MonoBehaviour
{
    public PollTextComponent ScoreText;
    public PollTextComponent NameText;
    public PollTextComponent TotalTimeText;

    public PollTextComponent NameTextPrefab;
    public PollTextComponent SocreTextPrefab;
    public PollTextComponent TotalTimeTextPrefab;
    public PollTextComponent TopScoreTextPrefab;

    private Dictionary<string, List<PollTextComponent>> LeaderboardEntries;

    public float VerticalSpacing = 3.0f;
    private float VerticalOffset = 10.5f;

    private List<LeaderboardPlayerData> PlayerData;
    public void SetPlayerData(List<LeaderboardPlayerData> playerData)
    {
        PlayerData = playerData;
    }

    public void CreateObjects()
    {
        LeaderboardEntries = new Dictionary<string, List<PollTextComponent>>();
        for (var i = 0; i < PlayerData.Count; i++)
        {
            if (i == 0)
            {
                var scoreTextInstance = Instantiate(TopScoreTextPrefab).GetComponent<PollTextComponent>();
                scoreTextInstance.transform.SetParent(transform);
                scoreTextInstance.transform.position = ScoreText.transform.position - new Vector3(0, -5 + ((i + 1) * VerticalSpacing) + VerticalOffset, 0);
                scoreTextInstance.SetTextData(PlayerData[i].PlayerScore.ToString());
                scoreTextInstance.CreateAllObjects();

                var nameTextInstance = Instantiate(TopScoreTextPrefab).GetComponent<PollTextComponent>();
                nameTextInstance.transform.SetParent(transform);
                nameTextInstance.transform.position = NameText.transform.position - new Vector3(0, -5 + ((i + 1) * VerticalSpacing) + VerticalOffset, 0);
                nameTextInstance.SetTextData(PlayerData[i].PlayerDisplayName);
                nameTextInstance.CreateAllObjects();

                var totalTimeTextInstance = Instantiate(TopScoreTextPrefab).GetComponent<PollTextComponent>();
                totalTimeTextInstance.transform.SetParent(transform);
                totalTimeTextInstance.transform.position = TotalTimeText.transform.position - new Vector3(0, -5 + ((i + 1) * VerticalSpacing) + VerticalOffset, 0);
                totalTimeTextInstance.SetTextData(PlayerData[i].TotalTime);
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
                scoreTextInstance.transform.position = ScoreText.transform.position - new Vector3(0, ((i + 1) * VerticalSpacing) + VerticalOffset, 0);
                scoreTextInstance.SetTextData(PlayerData[i].PlayerScore.ToString());
                scoreTextInstance.CreateAllObjects();

                var nameTextInstance = Instantiate(NameTextPrefab).GetComponent<PollTextComponent>();
                nameTextInstance.transform.SetParent(transform);
                nameTextInstance.transform.position = NameText.transform.position - new Vector3(0, ((i + 1) * VerticalSpacing) + VerticalOffset, 0);
                nameTextInstance.SetTextData(PlayerData[i].PlayerDisplayName);
                nameTextInstance.CreateAllObjects();

                var totalTimeTextInstance = Instantiate(TotalTimeTextPrefab).GetComponent<PollTextComponent>();
                totalTimeTextInstance.transform.SetParent(transform);
                totalTimeTextInstance.transform.position = TotalTimeText.transform.position - new Vector3(0, ((i + 1) * VerticalSpacing) + VerticalOffset, 0);
                totalTimeTextInstance.SetTextData(PlayerData[i].TotalTime);
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
