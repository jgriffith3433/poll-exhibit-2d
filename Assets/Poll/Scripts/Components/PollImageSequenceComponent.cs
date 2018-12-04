using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class PollImageSequenceComponent : PollImageComponent
{
    private List<Sprite> Sprites;
    private int currentSprite = 0;
    public bool Loading = true;
    private bool IsPaused = false;
    public float SpeedDelay = 0.04f;

    public void CreateObjects(string imageBasePath)
    {
        m_image = gameObject.GetComponentInChildren<Image>();
        Loading = true;
        Sprites = new List<Sprite>();
        StartCoroutine(GetData(imageBasePath));
    }

    public IEnumerator GetData(string imageBasePath)
    {
        var di = new DirectoryInfo(imageBasePath);
        var imageFileInfo = new List<FileInfo>();
        imageFileInfo.AddRange(di.GetFiles("*.jpg"));
        imageFileInfo.AddRange(di.GetFiles("*.jpeg"));
        imageFileInfo.AddRange(di.GetFiles("*.png"));

        foreach (var fi in imageFileInfo)
        {
            var request = new WWW(fi.FullName);
            yield return request;
            if (string.IsNullOrEmpty(request.error))
            {
                Sprites.Add(Sprite.Create(request.texture, new Rect(0, 0, request.texture.width, request.texture.height), new Vector2(0, 0)));
            }
            else
            {
                Debug.Log("URL request failed:" + request.error);
            }
        }
        Loading = false;
        Play();
    }

    public void Play()
    {
        if (!Loading)
        {
            StartCoroutine(PlayImageSequence());
        }
    }

    public void Pause()
    {
        IsPaused = true;
    }

    private IEnumerator PlayImageSequence()
    {
        if (!IsPaused)
        {
            currentSprite = (++currentSprite) % Sprites.Count;
            SetSprite(Sprites[currentSprite]);
            yield return new WaitForSeconds(SpeedDelay);
            yield return PlayImageSequence();
        }
    }
}
