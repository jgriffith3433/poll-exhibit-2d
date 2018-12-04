using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeadboardTopDownComponent : MonoBehaviour
{
    public PollTextComponent ScoreText;
    public PollTextComponent NameText;
    public PollTextComponent TotalTimeText;

    public PollTextComponent LeaderboardTextPrefab;

    private Dictionary<string, List<PollTextComponent>> LeaderboardEntries;
    
    public float VerticalSpacing = 3.0f;

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
            var scoreTextInstance = Instantiate(LeaderboardTextPrefab).GetComponent<PollTextComponent>();
            scoreTextInstance.transform.SetParent(transform);
            scoreTextInstance.transform.position = ScoreText.transform.position - new Vector3(0, (i + 1) * VerticalSpacing, 0);
            scoreTextInstance.SetTextData(PlayerData[i].PlayerScore.ToString());
            scoreTextInstance.CreateAllObjects();

            var nameTextInstance = Instantiate(LeaderboardTextPrefab).GetComponent<PollTextComponent>();
            nameTextInstance.transform.SetParent(transform);
            nameTextInstance.transform.position = NameText.transform.position - new Vector3(0, (i + 1) * VerticalSpacing, 0);
            nameTextInstance.SetTextData(PlayerData[i].PlayerDisplayName);
            nameTextInstance.CreateAllObjects();

            var totalTimeTextInstance = Instantiate(LeaderboardTextPrefab).GetComponent<PollTextComponent>();
            totalTimeTextInstance.transform.SetParent(transform);
            totalTimeTextInstance.transform.position = TotalTimeText.transform.position - new Vector3(0, (i + 1) * VerticalSpacing, 0);
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
