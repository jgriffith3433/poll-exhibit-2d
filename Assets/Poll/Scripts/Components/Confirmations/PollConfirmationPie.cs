using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollConfirmationPie : PollConfirmation
{
    public PieChartComponent AnswerPieChartPrefab;
    private PieChartComponent AnswerPieChartInstance;

    public override void CreateObjects()
    {
        AnswerPieChartInstance = Instantiate(AnswerPieChartPrefab, transform).GetComponent<PieChartComponent>();
        ConfirmationObjectInstance = AnswerPieChartInstance.gameObject;
        OriginalConfirmationInstancePosition = AnswerPieChartInstance.transform.GetChild(0).position;
        var usedLightGray = false;
        foreach (var answer in AnswerTimes)
        {
            if (!usedLightGray)
            {
                usedLightGray = true;
                AnswerPieChartInstance.SetValue(answer.Key, answer.Value.Count, AnswerCorrectIncorrect[answer.Key] ? PieChartComponent.PieChartColor.Red : PieChartComponent.PieChartColor.LightGray);
            }
            else
            {
                AnswerPieChartInstance.SetValue(answer.Key, answer.Value.Count, AnswerCorrectIncorrect[answer.Key] ? PieChartComponent.PieChartColor.Red : PieChartComponent.PieChartColor.Grey);
            }
        }
        AnswerPieChartInstance.transform.GetChild(0).position += new Vector3(0, -100, 0);
        AnswerPieChartInstance.transform.GetChild(0).localScale = new Vector3(2, 2, 2);
        AnswerPieChartInstance.gameObject.SetActive(false);
    }

    public override void DoAnimation()
    {
        AnswerPieChartInstance.gameObject.SetActive(true);
        AnswerPieChartInstance.transform.GetChild(0).position = OriginalConfirmationInstancePosition;
        AnswerPieChartInstance.DoAnimation();
    }

    public override void TransitionIn()
    {
    }

    public override void TransitionOut()
    {
        AnswerPieChartInstance.transform.GetChild(0).position += new Vector3(0, -100, 0);
        AnswerPieChartInstance.gameObject.SetActive(false);
    }

    public override void ShowObjects()
    {
        AnswerPieChartInstance.gameObject.SetActive(true);
    }

    public override void HideObjects()
    {
        AnswerPieChartInstance.gameObject.SetActive(false);
    }
}
