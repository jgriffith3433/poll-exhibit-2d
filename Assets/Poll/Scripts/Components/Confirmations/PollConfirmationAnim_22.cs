using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollConfirmationAnim_22 : PollConfirmation
{
    public PollImageSequenceComponent ConfirmationSequenceInstance;
    public string AnimDirectoryName;

    public override void CreateObjects()
    {
        base.CreateObjects();
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
    }
}
