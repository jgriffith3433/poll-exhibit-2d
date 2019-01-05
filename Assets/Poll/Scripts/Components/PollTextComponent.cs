using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PollTextComponent : MonoBehaviour {
    public TextMeshProUGUI ChildTextMeshPro;
    public RectTransform ChildRectTransform;
    private string Text;
    private float HeightToAnimateTo;
    private int AnimateSpeed = 5;
    private bool DoAnimateFromTop = false;
    public bool LoopAnimation = false;

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

    public void AnimateFadeIn(int animSpeed = 5)
    {
        AnimateSpeed = animSpeed;
        StartCoroutine(AnimateVertexColors(255));
    }

    public void AnimateFadeOut(int animSpeed = 5)
    {
        AnimateSpeed = animSpeed;
        StartCoroutine(AnimateVertexColors(0));
    }

    IEnumerator AnimateVertexColors(byte toAlpha)
    {
        // Need to force the text object to be generated so we have valid data to work with right from the start.
        ChildTextMeshPro.ForceMeshUpdate();

        TMP_TextInfo textInfo = ChildTextMeshPro.textInfo;
        Color32[] newVertexColors;

        int currentCharacter = 0;
        int startingCharacterRange = currentCharacter;
        bool isRangeMax = false;

        while (!isRangeMax)
        {
            for (int i = startingCharacterRange; i < currentCharacter + 1; i++)
            {
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                newVertexColors = textInfo.meshInfo[materialIndex].colors32;
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                byte newAlpha = newVertexColors[vertexIndex].a;
                if (toAlpha > newAlpha)
                {
                    newAlpha = (byte)Mathf.Clamp(newAlpha + (byte)AnimateSpeed, 0, 255);
                }
                else if (toAlpha < newAlpha)
                {
                    newAlpha = (byte)Mathf.Clamp(newAlpha - (byte)AnimateSpeed, 0, 255);
                }
                for (var vc = 0; vc < newVertexColors.Length; vc++)
                {
                    newVertexColors[vc].a = newAlpha;
                }
                if (newAlpha == toAlpha)
                {
                    startingCharacterRange += 1;

                    if (startingCharacterRange == textInfo.characterCount)
                    {
                        yield return new WaitForSeconds(1.0f);
                        ChildTextMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                        currentCharacter = 0;
                        startingCharacterRange = 0;
                        isRangeMax = true;
                    }
                }
            }

            // Upload the changed vertex colors to the Mesh.
            ChildTextMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            if (currentCharacter + 1 < textInfo.characterCount)
            {
                currentCharacter += 1;
            }
            yield return new WaitForSeconds(0.05f);
        }
        if (LoopAnimation)
        {
            if (toAlpha == 0)
            {
                AnimateFadeIn();
            }
            else
            {
                AnimateFadeIn();
            }
        }
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
