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
    public bool Playing = false;
    public bool Loop = false;
    public bool PlayOnAwake = false;
    public float SpeedDelay = 0.04f;
    public string ImageSequenceFolder;

    public void Awake()
    {
        if (PlayOnAwake)
        {
            CreateObjects(true);
        }
    }

    public void SetImageSequenceFolder(string imageSequenceFolder)
    {
        ImageSequenceFolder = Application.dataPath + "/" + imageSequenceFolder;
    }

    public void SetLoop(bool loop)
    {
        Loop = loop;
    }

    public void CreateObjects(bool autoPlay)
    {
        m_image = gameObject.GetComponentInChildren<Image>();
        m_image.gameObject.SetActive(false);
        Loading = true;
        Sprites = new List<Sprite>();
        StartCoroutine(GetData(autoPlay));
    }

    public IEnumerator GetData(bool autoPlay)
    {
        yield return PollImageSequenceLoader.Instance.LoadImageSeqence(ImageSequenceFolder, (spriteList) =>
        {
            Sprites = spriteList;
            Loading = false;
            if (autoPlay)
            {
                Play();
            }
        });
    }

    public void Play()
    {
        StartCoroutine(PlayAfterLoaded());
    }

    private IEnumerator PlayAfterLoaded()
    {
        if (!Loading)
        {
            m_image.gameObject.SetActive(true);
            Playing = true;
            yield return StartCoroutine(PlayImageSequence());
        }
        else
        {
            yield return new WaitForSeconds(.1f);
            StartCoroutine(PlayAfterLoaded());
        }
    }

    public void Pause()
    {
        Playing = false;
    }

    public void UnPause()
    {
        Playing = true;
    }

    private IEnumerator PlayImageSequence()
    {
        if (Playing)
        {
            if (currentSprite + 1 >= Sprites.Count && !Loop)
            {
                yield return null;
            }
            else
            {
                currentSprite = (++currentSprite) % Sprites.Count;
                SetSprite(Sprites[currentSprite]);
                yield return new WaitForSeconds(SpeedDelay);
                yield return PlayImageSequence();
            }
        }
    }
}
