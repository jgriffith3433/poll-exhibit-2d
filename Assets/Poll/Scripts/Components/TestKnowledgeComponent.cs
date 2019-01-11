using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestKnowledgeComponent : MonoBehaviour
{
    public GameObject ConsentGameObject;
    public GameObject TopScoresGameObject;
    public GameObject RulesGameObject;

    public LoginComponent LoginPrefab;
    private LoginComponent LoginInstance;

    private int ScreenIndex;

    public void Awake()
    {
        LoginInstance = Instantiate(LoginPrefab).GetComponent<LoginComponent>();
        LoginInstance.transform.SetParent(transform);
        LoginInstance.CreateAllObjects();
        LoginInstance.gameObject.SetActive(false);
        ScreenIndex = 0;
    }

    private void GoToNextScreen()
    {
        ScreenIndex++;
        switch(ScreenIndex)
        {
            case 1:
                ConsentGameObject.SetActive(false);
                TopScoresGameObject.SetActive(true);
                break;
            case 2:
                TopScoresGameObject.SetActive(false);
                RulesGameObject.SetActive(true);
                LoginInstance.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void ButtonPress(string buttonName)
    {
        if (ScreenIndex == 0)
        {
            if (buttonName == "DisagreeButton")
            {
                ExhibitGameManager.Instance.OnDoNotConsent();
            }
            else if (buttonName == "AgreeButton")
            {
                GoToNextScreen();
            }
        }
        else if (ScreenIndex == 2)
        {
            LoginInstance.FillValues();
            if (LoginInstance.ValuesFilled)
            {
                PollManager.Instance.BeginPoll(LoginInstance.DisplayName, LoginInstance.FirstName, LoginInstance.LastName);
            }
        }
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
            if (ScreenIndex == 1)
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
