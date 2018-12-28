using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollConfirmationSingle : PollConfirmation
{
    public override void CreateObjects()
    {
        PollAnswerData correctAnswer = null;
        for (var i = 0; i < PollAnswers.Count; i++)
        {
            if (PollAnswers[i].Correct)
            {
                correctAnswer = PollAnswers[i];
            }
        }
        if (correctAnswer != null)
        {
            ConfirmationTextInstances[0].SetTextData(correctAnswer.AnswerText);
            ConfirmationTextInstances[0].CreateAllObjects();
        }
    }
}
