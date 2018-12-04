using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarComponent : MonoBehaviour {
    public Transform BarModel;
    public PollTextComponent Text;
    public float HeightScale = 10;
    public float ScaleSpeed = 1;
    public float DefaultValue;

    private float MaxValue;
    private float Value;

    public void Awake()
    {
        Value = DefaultValue;
    }

    public void SetValue(float value)
    {
        Value = value;
    }

    public void SetMaxValue(float value)
    {
        MaxValue = value;
    }

    public void SetCategory(string text)
    {
        Text.SetTextData(text);
    }

    public void CreateObjects()
    {
        Text.CreateAllObjects();
    }

    public void Update()
    {
        var newHeight = (Value / MaxValue * 100) * HeightScale;
        if (BarModel.transform.localScale.z < newHeight)
        {
            BarModel.transform.localScale = new Vector3(BarModel.transform.localScale.x, BarModel.transform.localScale.y, BarModel.transform.localScale.z + newHeight * ScaleSpeed * Time.deltaTime);
            if (BarModel.transform.localScale.z > newHeight)
            {
                BarModel.transform.localScale = new Vector3(BarModel.transform.localScale.x, BarModel.transform.localScale.y, newHeight);
            }
        }
        else if (BarModel.transform.localScale.z > newHeight)
        {
            BarModel.transform.localScale = new Vector3(BarModel.transform.localScale.x, BarModel.transform.localScale.y, BarModel.transform.localScale.z - newHeight * ScaleSpeed * Time.deltaTime);
            if (BarModel.transform.localScale.z < newHeight)
            {
                BarModel.transform.localScale = new Vector3(BarModel.transform.localScale.x, BarModel.transform.localScale.y, newHeight);
            }
        }
    }
}
