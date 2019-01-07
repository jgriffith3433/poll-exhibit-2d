using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollConfirmation : MonoBehaviour
{
    public PollTextComponent[] ConfirmationTextInstances;

    protected List<PollAnswerData> PollAnswers;
    protected Vector3 OriginalConfirmationInstancePosition;

    protected Dictionary<string, bool> AnswerCorrectIncorrect;
    protected Dictionary<string, List<int>> AnswerTimes;

    protected GameObject ConfirmationObjectInstance;
    protected bool TransitioningIn;
    protected bool TransitioningOut;
    protected bool Test = false;

    public void SetData(Dictionary<string, List<int>> answerTimes, Dictionary<string, bool> answerCorrectIncorrect, List<PollAnswerData> pollAnswers)
    {
        PollAnswers = pollAnswers;
        AnswerTimes = answerTimes;
        AnswerCorrectIncorrect = answerCorrectIncorrect;
    }

    public void Awake()
    {
        if (Test)
        {
            StartCoroutine(DoTest());
        }
    }

    public virtual IEnumerator DoTest()
    {
        yield return null;
    }

    public virtual void CreateObjects()
    {
        for (var i = 0; i < ConfirmationTextInstances.Length; i++)
        {
            ConfirmationTextInstances[i].SetTextData(PollAnswers[i].AnswerText);
            ConfirmationTextInstances[i].CreateAllObjects();
        }
    }

    public virtual void ShowObjects()
    {
        /*for (var i = 0; i < ConfirmationTextInstances.Length; i++)
        {
            ConfirmationTextInstances[i].ShowObjects();
        }*/
        gameObject.SetActive(true);
    }

    public virtual void HideObjects()
    {
        /*for (var i = 0; i < ConfirmationTextInstances.Length; i++)
        {
            ConfirmationTextInstances[i].HideObjects();
        }*/
        gameObject.SetActive(false);
    }

    public virtual void TransitionIn()
    {
        TransitioningIn = true;
    }

    public virtual void TransitionOut()
    {
        TransitioningOut = true;
    }

    public virtual void Update()
    {
        if (TransitioningIn)
        {
            if (ConfirmationObjectInstance.transform.position.y < OriginalConfirmationInstancePosition.y)
            {
                ConfirmationObjectInstance.transform.position += new Vector3(0, 1, 0);
            }
            else if (ConfirmationObjectInstance.transform.position.y >= OriginalConfirmationInstancePosition.y)
            {
                ConfirmationObjectInstance.transform.position = new Vector3(ConfirmationObjectInstance.transform.position.x, OriginalConfirmationInstancePosition.y, ConfirmationObjectInstance.transform.position.z);
                TransitioningIn = false;
            }
        }
        if (TransitioningOut)
        {
            if (ConfirmationObjectInstance.transform.position.y > -100)
            {
                ConfirmationObjectInstance.transform.position -= new Vector3(0, 1, 0);
            }
            else if (ConfirmationObjectInstance.transform.position.y <= -100)
            {
                ConfirmationObjectInstance.transform.position = new Vector3(ConfirmationObjectInstance.transform.position.x, -100, ConfirmationObjectInstance.transform.position.z);
                TransitioningOut = false;
                Destroy(gameObject);
            }
        }
    }

    public virtual void DoAnimation()
    {

    }
}
