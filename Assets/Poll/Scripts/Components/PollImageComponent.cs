using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PollImageComponent : MonoBehaviour {
    protected Image m_image;

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
