using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class PollAnswerData {
    public string AnswerText { get; set; }
    public Vector3 AnswerTextPosition { get; set; }
    
    public string AnswerButtonText { get; set; }
    public Vector3 AnswerButtonTextPosition { get; set; }

    public bool Correct { get; set; }
    public int AnswerId { get; set; }

    public PollAnswerData(JSONNode xObj) {
        ParseData(xObj);
    }

    private void ParseData(JSONNode xObj) {
        var answerTextPositionSplit = xObj["answer_text_position"].Value.Split(',');
        AnswerTextPosition = new Vector3(float.Parse(answerTextPositionSplit[0]), float.Parse(answerTextPositionSplit[1]), float.Parse(answerTextPositionSplit[2]));

        var answerButtonTextPositionSplit = xObj["answer_button_text_position"].Value.Split(',');
        AnswerButtonTextPosition = new Vector3(float.Parse(answerButtonTextPositionSplit[0]), float.Parse(answerButtonTextPositionSplit[1]), float.Parse(answerButtonTextPositionSplit[2]));

        AnswerId = int.Parse(xObj["answer_id"].Value);
        AnswerText = xObj["answer_text"].Value;
        AnswerButtonText = xObj["answer_button_text"].Value;
        Correct = xObj["correct"].AsBool;
    }
}
