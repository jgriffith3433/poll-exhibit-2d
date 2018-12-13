using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestKnowledgeComponent : MonoBehaviour
{
    public GameObject TopScoresGameObject;
    public GameObject RulesGameObject;

    private bool OnFirstScreen = true;
    
    private void GoToNextScreen()
    {
        OnFirstScreen = false;
        TopScoresGameObject.SetActive(false);
        RulesGameObject.SetActive(true);
    }

    public void PlayNow()
    {
        PollManager.Instance.BeginPoll();
    }

    public void Update()
    {
        Vector2 touchClickPosition = Vector2.zero;
        if (Input.GetMouseButtonDown(0))
        {
            touchClickPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        for (var i = 0; i < Input.touchCount; ++i)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                touchClickPosition = Input.GetTouch(i).position;
            }
        }
        if (touchClickPosition != Vector2.zero)
        {
            if (OnFirstScreen)
            {
                GoToNextScreen();
            }
            else
            {
                RaycastHit hitInfo;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(touchClickPosition), out hitInfo))
                {
                    if (hitInfo.collider != null)
                    {
                        if (hitInfo.transform)
                        {
                            var buttonComponent = hitInfo.transform.GetComponent<PollButtonComponent>();
                            if (buttonComponent)
                            {
                                buttonComponent.OnClick();
                            }
                        }
                    }
                }
            }
        }
    }
}
