using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PollImageComponent : MonoBehaviour {
    protected Image m_image;
    protected string m_ImagePath;
    public bool Loading = true;

    public virtual void CreateObjects(string imagePath)
    {
        m_image = gameObject.GetComponentInChildren<Image>();
        m_ImagePath = Application.dataPath + "/" + imagePath;
        StartCoroutine(GetData());
    }

    private IEnumerator GetData()
    {
        var request = new WWW(m_ImagePath);
        yield return request;
        if (string.IsNullOrEmpty(request.error))
        {
            SetSprite(Sprite.Create(request.texture, new Rect(0, 0, request.texture.width, request.texture.height), new Vector2(0, 0)));
        }
        else
        {
            Debug.Log("m_ImagePath: " + m_ImagePath);
            Debug.Log("Poll : URL request failed, " + request.error);
        }
        Loading = false;
    }

    public virtual void CreateObjects(Texture2D texture)
    {
        m_image = gameObject.GetComponentInChildren<Image>();
        SetSprite(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0)));
    }

    public virtual void HideObjects()
    {
        gameObject.SetActive(false);
    }

    public virtual void ShowObjects()
    {
        gameObject.SetActive(true);
    }

    protected void SetSprite(Sprite newSprite)
    {
        m_image.sprite = newSprite;
    }
}
