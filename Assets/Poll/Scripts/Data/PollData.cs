using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class PollData
{
    private string RemoteUrl;
    public bool IsDoneParsing = false;
    public string PollTitle { get; set; }
    public int PollId { get; set; }
    public int NumberOfQuestionsAsked { get; set; }
    public Vector3 PollTitlePosition { get; set; }
    public string PollDataRawText { get; set; }

    public List<PollQuestionData> QuestionsData { get; set; }

    public PollData(string remoteUrl)
    {
        RemoteUrl = remoteUrl;
    }

    public IEnumerator GetData() {
        WWW request = new WWW(RemoteUrl);
        yield return request;
        if (string.IsNullOrEmpty(request.error)) {
            try
            {
                PollDataRawText = request.text;
                StartParse(JSON.Parse(PollDataRawText));
            }
            catch (Exception e)
            {
                Debug.LogError("Poll : Invalid document format, please check your settings, with exception " + e.ToString());
            }
        }
        else
        {
            Debug.LogError("Poll : URL request failed ," + request.error);
        }
        IsDoneParsing = true;
    }

    private void StartParse(JSONNode xObj) {
        var titlePositionSplit = xObj["title_text_position"].Value.Split(',');
        PollTitlePosition = new Vector3(float.Parse(titlePositionSplit[0]), float.Parse(titlePositionSplit[1]), float.Parse(titlePositionSplit[2]));

        PollId = int.Parse(xObj["poll_id"].Value);
        NumberOfQuestionsAsked = int.Parse(xObj["num_questions_asked"].Value);
        PollTitle = xObj["title_text"].Value;
        var allQuestions = xObj["questions"];
        try
        {
            QuestionsData = new List<PollQuestionData>();
            for (int i = 0; i < allQuestions.Count; i++)
            {
                var qd = new PollQuestionData(allQuestions[i]);
                if (qd.Enabled)
                {
                    QuestionsData.Add(qd);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
        }
    }
}