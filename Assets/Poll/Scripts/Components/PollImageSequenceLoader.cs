using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class PollImageSequenceLoader : MonoBehaviour
{
    public static PollImageSequenceLoader Instance;

    public void Awake()
    {
        Instance = this;
        LoadedImageSequences = new Dictionary<string, List<Sprite>>();
        LoadingImageSequences = new Dictionary<string, bool>();
    }

    private Dictionary<string, List<Sprite>> LoadedImageSequences;
    private Dictionary<string, bool> LoadingImageSequences;

    public IEnumerator LoadImageSeqence(string imageSequenceFolder, System.Action<List<Sprite>> callback)
    {
        if (LoadedImageSequences.ContainsKey(imageSequenceFolder))
        {
            callback(LoadedImageSequences[imageSequenceFolder]);
        }
        else
        {
            if (LoadingImageSequences.ContainsKey(imageSequenceFolder) && LoadingImageSequences[imageSequenceFolder] == true)
            {
                var maxToWait = 10;
                var waitedTimes = 0;
                while (waitedTimes < maxToWait)
                {
                    if (!LoadedImageSequences.ContainsKey(imageSequenceFolder))
                    {
                        yield return new WaitForSeconds(0.5f);
                        waitedTimes++;
                    }
                    else
                    {
                        callback(LoadedImageSequences[imageSequenceFolder]);
                        break;
                    }
                }
            }
            else
            {
                LoadingImageSequences.Add(imageSequenceFolder, true);
                yield return GetData(imageSequenceFolder);
                var maxToWait2 = 10;
                var waitedTimes2 = 0;
                while (waitedTimes2 < maxToWait2)
                {
                    if (!LoadedImageSequences.ContainsKey(imageSequenceFolder))
                    {
                        yield return new WaitForSeconds(0.5f);
                        waitedTimes2++;
                    }
                    else
                    {
                        callback(LoadedImageSequences[imageSequenceFolder]);
                        break;
                    }
                }
            }
        }
    }

    private IEnumerator GetData(string imageBasePath)
    {
        var di = new DirectoryInfo(imageBasePath);
        var imageFileInfo = new List<FileInfo>();
        imageFileInfo.AddRange(di.GetFiles("*.jpg"));
        imageFileInfo.AddRange(di.GetFiles("*.jpeg"));
        imageFileInfo.AddRange(di.GetFiles("*.png"));

        var sprites = new List<Sprite>();
        foreach (var fi in imageFileInfo.OrderBy(i => i.Name))
        {
            var request = new WWW(fi.FullName);
            yield return request;
            if (string.IsNullOrEmpty(request.error))
            {
                sprites.Add(Sprite.Create(request.texture, new Rect(0, 0, request.texture.width, request.texture.height), new Vector2(0, 0)));
            }
            else
            {
                Debug.Log("URL request failed:" + request.error);
            }
        }
        LoadedImageSequences.Add(imageBasePath, sprites);
    }
}