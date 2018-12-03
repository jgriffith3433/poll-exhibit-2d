using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardComponent : MonoBehaviour {

    public BarDataFiller BarDataFillerPrefab;
    private BarDataFiller BarDataFillerInstance;
    public bool Hidden;

    public void Awake()
    {
    }

	public void FillLeaderboard()
    {
        BarDataFillerInstance.Fill();
    }

	public void RestartLeaderboard()
    {
        CreateObjects();
        BarDataFillerInstance.Zero();
    }

    public void CreateObjects()
    {
        Hidden = false;
        BarDataFillerInstance = Instantiate(BarDataFillerPrefab).GetComponent<BarDataFiller>();
        BarDataFillerInstance.transform.SetParent(transform);
        gameObject.SetActive(true);
    }

    public void HideObjects()
    {
        Hidden = true;
        if (BarDataFillerInstance)
        {
            Destroy(BarDataFillerInstance.gameObject);
        }
        gameObject.SetActive(false);
    }

    public void Play()
    {
        LeaderboardManager.Instance.OnPlay();
    }

    public void Update()
    {
        if (!Hidden && ExhibitGameManager.Instance.CurrentGameState != "Screensaver")
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
