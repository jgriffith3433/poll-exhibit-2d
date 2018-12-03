using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PollImageComponent : MonoBehaviour {
    private Image m_image;
    private string m_image_path;
    
    public void CreateObjects()
    {
        m_image = gameObject.GetComponentInChildren<Image>();
    }

    public void HideObjects()
    {
        gameObject.SetActive(false);
    }

    public void ShowObjects()
    {
        gameObject.SetActive(true);
    }

    public void SetSprite(Sprite newSprite)
    {
        m_image.sprite = newSprite;
    }
}
