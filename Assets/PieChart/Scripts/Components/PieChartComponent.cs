using ChartAndGraph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieChartComponent : MonoBehaviour
{
    public enum PieChartColor
    {
        Red,
        Grey
    }
    public Material Red;
    public Material Gray;
    public PieChart Pie;
    private float AnimateSpeed = 5.0f;

    public void SetValue(string category, int value, PieChartColor pieChartColor)
    {
        if (!Pie.DataSource.HasCategory(category))
        {
            if (pieChartColor == PieChartColor.Grey)
            {
                Pie.DataSource.AddCategory(category, Gray);

            }
            else if (pieChartColor == PieChartColor.Red)
            {
                Pie.DataSource.AddCategory(category, Red);
            }
        }
        Pie.DataSource.SlideValue(category, value, AnimateSpeed);
    }
    
    public void DoAnimation()
    {
        Pie.GetComponent<PieAnimation>().enabled = true;
    }
}
