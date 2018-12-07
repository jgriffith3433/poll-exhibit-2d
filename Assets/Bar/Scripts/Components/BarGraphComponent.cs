using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarGraphComponent : MonoBehaviour
{
    public float MaxBarValue = 30;
    public float BarSpacing = 4;
    public BarComponent BarPrefab;
    private Dictionary<string, BarComponent> BarInstances;

    public void Awake()
    {
        BarInstances = new Dictionary<string, BarComponent>();
    }

    public void SetValue(string category, float value, BarComponent.BarColor barColor)
    {
        if (BarInstances.ContainsKey(category) == false)
        {
            var bar = Instantiate(BarPrefab).GetComponent<BarComponent>();
            bar.transform.SetParent(transform);
            bar.transform.position += new Vector3(BarInstances.Count * BarSpacing, 0, 0);
            bar.SetMaxValue(MaxBarValue);
            bar.SetCategory(category);
            bar.CreateObjects();
            bar.SetBarColor(barColor);
            BarInstances.Add(category, bar);
        }
        BarInstances[category].SetValue(value);
    }
}
