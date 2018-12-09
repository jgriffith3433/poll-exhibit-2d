using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PollTextComponent : MonoBehaviour {
    public TextMeshProUGUI ChildTextMeshPro;
    public RectTransform ChildRectTransform;
    private string Text;
    private float HeightToAnimateTo;
    private float AnimateSpeed = 5.0f;
    private bool DoAnimateFromTop = false;

    public void Awake()
    {
        ChildTextMeshPro = gameObject.GetComponent<TextMeshProUGUI>();

        if (ChildTextMeshPro == null)
        {
            ChildTextMeshPro = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    public void SetTextData(string text)
    {
        Text = text;
    }

    public void CreateAllObjects()
    {
        ChildTextMeshPro.SetText(Text);
    }

    public void HideObjects()
    {
        gameObject.SetActive(false);
    }

    public void ShowObjects()
    {
        gameObject.SetActive(true);
    }

    public void AnimateFromTop()
    {
        HeightToAnimateTo = ChildRectTransform.sizeDelta.y;
        ChildRectTransform.sizeDelta = new Vector2(ChildRectTransform.sizeDelta.x, 0.1f);
        DoAnimateFromTop = true;
    }

    public void Update()
    {
        if (DoAnimateFromTop)
        {
            if (ChildRectTransform.sizeDelta.y < HeightToAnimateTo)
            {
                ChildRectTransform.sizeDelta = new Vector2(ChildRectTransform.sizeDelta.x, ChildRectTransform.sizeDelta.y + AnimateSpeed);

                if (ChildRectTransform.sizeDelta.y >= HeightToAnimateTo)
                {
                    ChildRectTransform.sizeDelta = new Vector2(ChildRectTransform.sizeDelta.x, HeightToAnimateTo);
                    DoAnimateFromTop = false;
                }
                ChildTextMeshPro.SetText(Text);
            }
        }
    }
}
