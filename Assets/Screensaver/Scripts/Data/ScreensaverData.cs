using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using System.Linq;
using System.IO;

public class ScreensaverData
{
    private string DirectoryUrl;
    public List<Texture2D> ScreensaverImages { get; set; }
    public bool Loading = true;

    public ScreensaverData(string directoryUrl)
    {
        ScreensaverImages = new List<Texture2D>();
        DirectoryUrl = directoryUrl;
    }

    public IEnumerator GetData() {
        var di = new DirectoryInfo(DirectoryUrl);
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
                ScreensaverImages.Add(request.texture);
            }
            else
            {
                Debug.Log("Poll : URL request failed ," + request.error);
            }
        }
        Loading = false;
    }
}