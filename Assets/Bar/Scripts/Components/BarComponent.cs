using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarComponent : MonoBehaviour {
    public Transform BarModel;
    public PollTextComponent Text;
    public Material Red;
    public Material Grey;

    public float HeightScale = 10;
    public float ScaleSpeed = 0.1f;
    public float DefaultValue;

    private bool Animate;
    private float MaxValue;
    private float Value;
    public enum BarColor
    {
        Red,
        Grey
    }

    public void Awake()
    {
        Value = DefaultValue;
    }

    public void SetValue(float value)
    {
        Value = value;
    }

    public void DoAnimation()
    {
        Animate = true;
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

    public void SetBarColor(BarColor barGraphColor)
    {
        switch (barGraphColor)
        {
            case BarColor.Red:
                BarModel.GetComponent<MeshRenderer>().material = Red;
                break;
            case BarColor.Grey:
                BarModel.GetComponent<MeshRenderer>().material = Grey;
                break;
            default:
                break;
        }
    }

    public void Update()
    {
        if (Animate)
        {
            var newHeight = (Value / MaxValue * 100) * HeightScale;
            var textYPosition = (Value / MaxValue) * HeightScale * 1.5f;
            if (BarModel.transform.localScale.z < newHeight)
            {
                BarModel.transform.localScale = new Vector3(BarModel.transform.localScale.x, BarModel.transform.localScale.y, BarModel.transform.localScale.z + newHeight * ScaleSpeed * Time.deltaTime);
                if (BarModel.transform.localScale.z > newHeight)
                {
                    BarModel.transform.localScale = new Vector3(BarModel.transform.localScale.x, BarModel.transform.localScale.y, newHeight);
                    Text.transform.localPosition = new Vector3(Text.transform.localPosition.x, textYPosition, Text.transform.localPosition.z);
                    Animate = false;
                }
            }
            else if (BarModel.transform.localScale.z > newHeight)
            {
                BarModel.transform.localScale = new Vector3(BarModel.transform.localScale.x, BarModel.transform.localScale.y, BarModel.transform.localScale.z - newHeight * ScaleSpeed * Time.deltaTime);
                if (BarModel.transform.localScale.z < newHeight)
                {
                    BarModel.transform.localScale = new Vector3(BarModel.transform.localScale.x, BarModel.transform.localScale.y, newHeight);
                    Text.transform.localPosition = new Vector3(Text.transform.localPosition.x, textYPosition, Text.transform.localPosition.z);
                    Animate = false;
                }
            }
        }
    }
}
