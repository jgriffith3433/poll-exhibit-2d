using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollConfirmationAnim_07 : PollConfirmation
{
    public PollImageSequenceComponent ConfirmationSequenceInstance;
    public string AnimDirectoryName;

    public override IEnumerator DoTest()
    {
        yield return new WaitForSeconds(1);
        var answerTimes = new Dictionary<string, List<int>>();
        answerTimes.Add("answer 1", new List<int> { 1 });
        answerTimes.Add("answer 2", new List<int> { 2 });
        answerTimes.Add("answer 3", new List<int> { 3 });
        var answerCorrectIncorrect = new Dictionary<string, bool>();
        answerCorrectIncorrect.Add("answer 1", true);
        answerCorrectIncorrect.Add("answer 2", false);
        answerCorrectIncorrect.Add("answer 3", false);
        var pollAnswers = new List<PollAnswerData>();
        pollAnswers.Add(new PollAnswerData("answer 1", "A", true, 1));
        pollAnswers.Add(new PollAnswerData("answer 2", "B", false, 2));
        pollAnswers.Add(new PollAnswerData("answer 3", "C", false, 3));

        SetData(answerTimes, answerCorrectIncorrect, pollAnswers);
        CreateObjects();
        StartCoroutine(CheckIsDoneLoading());
    }

    public IEnumerator CheckIsDoneLoading()
    {
        if (!ConfirmationSequenceInstance.Loading)
        {
            TransitionIn();
            yield return new WaitForSeconds(2);
            DoAnimation();
            yield return new WaitForSeconds(5);
            TransitionOut();
            yield return new WaitForSeconds(1);
        }
        else
        {
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(CheckIsDoneLoading());
        }
    }

    public override void CreateObjects()
    {
        for (var i = 0; i < ConfirmationTextInstances.Length; i++)
        {
            if (PollAnswers[i].Correct)
            {
                ConfirmationTextInstances[i].SetTextData(PollAnswers[i].AnswerText);
                ConfirmationTextInstances[i].CreateAllObjects();
            }
        }

        for (var i = 0; i < ConfirmationTextInstances.Length; i++)
        {
            ConfirmationTextInstances[i].AnimateFadeOut(255);
        }

        ConfirmationObjectInstance = ConfirmationSequenceInstance.gameObject;
        ConfirmationSequenceInstance.transform.SetParent(transform);
        ConfirmationSequenceInstance.transform.position += new Vector3(0, 0, 6);
        ConfirmationSequenceInstance.SetImageSequenceFolder("Poll/Images/Confirmations/" + AnimDirectoryName);
        ConfirmationSequenceInstance.SetLoop(false);
        ConfirmationSequenceInstance.CreateObjects(false);
        ConfirmationSequenceInstance.ShowFirstFrame();
    }

    public override void DoAnimation()
    {
        ConfirmationSequenceInstance.Play();
        StartCoroutine(TransitionAnswerIn());
    }

    public override void TransitionOut()
    {
        base.TransitionOut();
        StartCoroutine(TransitionAnswerOut());
    }

    private IEnumerator TransitionAnswerIn()
    {
        yield return new WaitForSeconds(2);
        for (var i = 0; i < ConfirmationTextInstances.Length; i++)
        {
            ConfirmationTextInstances[i].AnimateFadeIn();
        }
    }

    private IEnumerator TransitionAnswerOut()
    {
        for (var i = 0; i < ConfirmationTextInstances.Length; i++)
        {
            ConfirmationTextInstances[i].AnimateFadeOut(255);
        }
        yield return null;
    }
}
