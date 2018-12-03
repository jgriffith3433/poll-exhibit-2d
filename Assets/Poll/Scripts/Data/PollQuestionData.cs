using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class PollQuestionData {

    public List<PollAnswerData> PollAnswersData { get; set; }
    public string QuestionText { get; set; }
    public Vector3 QuestionTextPosition { get; set; }
    public string QuestionType { get; set; }
    public int QuestionId { get; set; }

    public PollQuestionData(JSONNode xObj) {
        ParseData(xObj);
    }

    private void ParseData(JSONNode xObj)
    {
        var questionTextPositionSplit = xObj["question_text_position"].Value.Split(',');
        QuestionTextPosition = new Vector3(float.Parse(questionTextPositionSplit[0]), float.Parse(questionTextPositionSplit[1]), float.Parse(questionTextPositionSplit[2]));

        QuestionId = int.Parse(xObj["question_id"].Value);
        QuestionText = xObj["question_text"].Value;
        QuestionType = xObj["question_type"].Value;
        var allAnswers = xObj["answers"];
        PollAnswersData = new List<PollAnswerData>();
        for (int i = 0; i < allAnswers.Count; i++) {
            PollAnswersData.Add(new PollAnswerData(allAnswers[i]));
        }
    }
}
