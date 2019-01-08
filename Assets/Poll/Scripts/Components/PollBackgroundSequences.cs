using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollBackgroundSequences : MonoBehaviour
{
    public PollImageSequenceComponent TopLeft;
    public PollImageSequenceComponent BottomLeft;
    public PollImageSequenceComponent TopMiddle;
    public PollImageSequenceComponent BottomMiddle;
    public PollImageSequenceComponent TopRight;
    public PollImageSequenceComponent BottomRight;

    public void Awake()
    {
        StartCoroutine(WaitThenPlaySequences());
    }

    private IEnumerator WaitThenPlaySequences()
    {
        yield return new WaitForSeconds(1);
        TopLeft.transform.position += new Vector3(0, 0, 2);
        BottomLeft.transform.position += new Vector3(0, 0, 2);
        TopMiddle.transform.position += new Vector3(0, 0, 2);
        BottomMiddle.transform.position += new Vector3(0, 0, 2);
        TopRight.transform.position += new Vector3(0, 0, 2);
        BottomRight.transform.position += new Vector3(0, 0, 2);

        TopLeft.SetImageSequenceFolder("ExhibitGame/Images/background_image_sequence");
        BottomLeft.SetImageSequenceFolder("ExhibitGame/Images/background_image_sequence");
        TopMiddle.SetImageSequenceFolder("ExhibitGame/Images/background_image_sequence");
        BottomMiddle.SetImageSequenceFolder("ExhibitGame/Images/background_image_sequence");
        TopRight.SetImageSequenceFolder("ExhibitGame/Images/background_image_sequence");
        BottomRight.SetImageSequenceFolder("ExhibitGame/Images/background_image_sequence");

        TopLeft.SetLoop(true);
        TopLeft.CreateObjects(false);
        BottomLeft.SetLoop(true);
        BottomLeft.CreateObjects(false);
        TopMiddle.SetLoop(true);
        TopMiddle.CreateObjects(false);
        BottomMiddle.SetLoop(true);
        BottomMiddle.CreateObjects(false);
        TopRight.SetLoop(true);
        TopRight.CreateObjects(false);
        BottomRight.SetLoop(true);
        BottomRight.CreateObjects(false);

        yield return new WaitForSeconds(1);
        TopLeft.Play();
        BottomLeft.Play();
        TopMiddle.Play();
        BottomMiddle.Play();
        TopRight.Play();
        BottomRight.Play();
    }
}
