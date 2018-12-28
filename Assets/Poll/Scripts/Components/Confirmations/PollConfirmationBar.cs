using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PollConfirmationBar : PollConfirmation
{
    public BarGraphComponent AnswerBarGraphPrefab;
    private BarGraphComponent AnswerBarGraphInstance;

    public override void CreateObjects()
    {
        AnswerBarGraphInstance = Instantiate(AnswerBarGraphPrefab, transform).GetComponent<BarGraphComponent>();
        ConfirmationObjectInstance = AnswerBarGraphInstance.gameObject;
        OriginalConfirmationInstancePosition = AnswerBarGraphInstance.transform.position;
        AnswerBarGraphInstance.MaxBarValue = AnswerTimes.OrderByDescending(at => at.Value.Count).FirstOrDefault().Value.Count;

        var usedLightGrey = false;
        foreach (var answer in AnswerTimes)
        {
            if (!usedLightGrey)
            {
                usedLightGrey = true;
                AnswerBarGraphInstance.SetValue(answer.Key, answer.Value.Count, AnswerCorrectIncorrect[answer.Key] ? BarComponent.BarColor.Red : BarComponent.BarColor.LightGrey);
            }
            else
            {
                AnswerBarGraphInstance.SetValue(answer.Key, answer.Value.Count, AnswerCorrectIncorrect[answer.Key] ? BarComponent.BarColor.Red : BarComponent.BarColor.Grey);
            }
        }
        AnswerBarGraphInstance.transform.position += new Vector3(0, -100, 0);
    }

    public override void DoAnimation()
    {
        AnswerBarGraphInstance.DoAnimation();
    }

    public override void ShowObjects()
    {
        AnswerBarGraphInstance.gameObject.SetActive(true);
    }

    public override void HideObjects()
    {
        AnswerBarGraphInstance.gameObject.SetActive(false);
    }
}
