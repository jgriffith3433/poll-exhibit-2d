using UnityEngine;
using System.Collections;
using System;
using ChartAndGraph;
using System.Collections.Generic;
using System.Globalization;

public class BarDataFiller : MonoBehaviour
{
    [Serializable]
    public enum DataType
    {
        VectorArray,
        ArrayForEachElement,
        ObjectArray,
    }

    public enum DocumentFormat
    {
        XML,
        JSON
    }

    public enum VectorFormat
    {
        X_Y,
        Y_X,
        X_Y_SIZE,
        Y_X_SIZE,
        SIZE_X_Y,
        SIZE_Y_X,
        X_Y_GAP_SIZE,
        Y_X_GAP_SIZE
    }

    class VectorFormatData
    {
        public int X, Y, Size, Length;
        public VectorFormatData(int x, int y, int size, int length)
        {
            X = x;
            Y = y;
            Size = size;
            Length = length;
        }
    }
    
    [Serializable]
    public class CategoryData
    {
        public bool Enabled = true;

        [BarFillerEditor(DataType.ObjectArray)]
        [BarFillerEditor(DataType.ArrayForEachElement)]
        [BarFillerEditorAttribute(DataType.VectorArray)]
        public string Name;

        /// <summary>
        /// The way the data is stored in the object
        /// </summary>
        public DataType DataType;

        [BarFillerEditorAttribute(DataType.VectorArray)]
        public VectorFormat DataFormat;

        /// <summary>
        /// the amount of items to skip after each dataformat instance
        /// </summary>
        [BarFillerEditorAttribute(DataType.VectorArray)]
        public int Skip = 0;

        /// <summary>
        /// if this is empty then DataObjectName is not relative
        /// </summary>
        [BarFillerEditor(DataType.ObjectArray)]
        [BarFillerEditor(DataType.ArrayForEachElement)]
        public string ParentObjectName;

        [BarFillerEditorAttribute(DataType.VectorArray)]
        public string DataObjectName;


        [BarFillerEditor(DataType.ObjectArray)]
        [BarFillerEditor(DataType.ArrayForEachElement)]
        public string XDataObjectName;

        [BarFillerEditor(DataType.ObjectArray)]
        [BarFillerEditor(DataType.ArrayForEachElement)]
        public string YDataObjectName;

        [BarFillerEditor(DataType.ObjectArray)]
        [BarFillerEditor(DataType.ArrayForEachElement)]
        public string SizeDataObjectName;

        /// <summary>
        /// set to empty null or "none" for numbers. Set to a date format for a date :  https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings
        /// </summary>
        [BarFillerEditor(DataType.ObjectArray)]
        [BarFillerEditor(DataType.ArrayForEachElement)]
        public string XDateFormat = "";

        [BarFillerEditor(DataType.ObjectArray)]
        [BarFillerEditor(DataType.ArrayForEachElement)]
        public string YDateFormat = "" ;

    }

    public BarChart BarPrefab;
    private BarChart BarInstance;
    /// <summary>
    /// assign a bar chart prefab that will be used to copy category data
    /// </summary>

    public DocumentFormat Format;
    public string ParentObject;
    public CategoryData[] Categories = new CategoryData[0];

    private object[] mCategoryVisualStyle;
    delegate void CategoryLoader(CategoryData data);
    private Dictionary<DataType, CategoryLoader> mLoaders;
    private static Dictionary<VectorFormat, VectorFormatData> mVectorFormats;
    private ChartParser mParser;

    public void Awake()
    {
        BarInstance = GameObject.Find("Bar").GetComponent<BarChart>();
    }

    public void HideObjects()
    {
        
    }

    static BarDataFiller()
    {
        CreateVectorFormats();
    }

    void EnsureCreateDataTypes()
    {
        if (mLoaders != null)
            return;
        mLoaders = new Dictionary<DataType, CategoryLoader>();
        mLoaders[DataType.ArrayForEachElement] = LoadArrayForEachElement;
        mLoaders[DataType.ObjectArray] = LoadObjectArray;
        mLoaders[DataType.VectorArray] = LoadVectorArray;
    }

    static void CreateVectorFormats()
    {
        mVectorFormats = new Dictionary<VectorFormat, VectorFormatData>();
        mVectorFormats[VectorFormat.X_Y] = new VectorFormatData(0, 1, -1, 2);
        mVectorFormats[VectorFormat.Y_X] = new VectorFormatData(1, 0, -1, 2);
        mVectorFormats[VectorFormat.X_Y_SIZE] = new VectorFormatData(0, 1, 2, 3);
        mVectorFormats[VectorFormat.Y_X_SIZE] = new VectorFormatData(1, 0, 2, 3);
        mVectorFormats[VectorFormat.SIZE_X_Y] = new VectorFormatData(1, 2, 0, 3);
        mVectorFormats[VectorFormat.SIZE_Y_X] = new VectorFormatData(2, 1, 0, 3);
        mVectorFormats[VectorFormat.X_Y_GAP_SIZE] = new VectorFormatData(0, 1, 3, 4);
        mVectorFormats[VectorFormat.Y_X_GAP_SIZE] = new VectorFormatData(1, 0, 3, 4);  
    }

    private double ParseItem(string item,string format)
    {
        if(String.IsNullOrEmpty(format) || format.Equals("none",StringComparison.OrdinalIgnoreCase))
        {
            return double.Parse(item);
        }
        return ChartDateUtility.DateToValue(DateTime.ParseExact(item, format, CultureInfo.InvariantCulture));
    }

    void LoadArrayForEachElement(CategoryData data)
    {
        if(mParser.SetPathRelativeTo(data.ParentObjectName) == false)
        {
            Debug.LogWarning("Object " + data.ParentObjectName + " does not exist in the document");
            return;
        }
        var xObj = mParser.GetObject(data.XDataObjectName);
        object sizeObj = null;
        if(String.IsNullOrEmpty(data.SizeDataObjectName) == false)
            sizeObj = mParser.GetObject(data.SizeDataObjectName);
        int length = mParser.GetArraySize(xObj);
        if (sizeObj != null)
            length = Math.Min(length, mParser.GetArraySize(sizeObj));
        try
        {
            for (int i = 0; i < length; i++)
            {
                double x = ParseItem(mParser.GetItem(xObj, i),data.XDateFormat);
                BarInstance.DataSource.SlideValue(data.Name, data.XDataObjectName, x, 5f);
            }
        }
        catch(Exception e)
        {
            Debug.LogWarning("Data for category " + data.Name + " does not match the specified format. Ended with exception : "  + e.ToString());
        }
    }

    void LoadObjectArray(CategoryData data)
    {
        var parent = mParser.GetObject(data.ParentObjectName);
        if (parent == null)
        {
            Debug.LogWarning("Object " + data.ParentObjectName + " does not exist in the document");
            return;
        }

        int length = mParser.GetArraySize(parent);
        try
        {
            for (int i = 0; i < length; i++)
            {
                object item = mParser.GetItemObject(parent, i);
                double x = ParseItem(mParser.GetChildObjectValue(item, data.XDataObjectName),data.XDateFormat);
                BarInstance.DataSource.SlideValue(data.Name, data.XDataObjectName, x, 7f);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Data for category " + data.Name + " does not match the specified format. Ended with exception : " + e.ToString());
        }
    }

    void LoadVectorArray(CategoryData data)
    {
        var obj = mParser.GetObject(data.DataObjectName);
        int size = mParser.GetArraySize(obj);
        VectorFormatData formatData = mVectorFormats[data.DataFormat];
        if (size <0 ) // this is not an array , show warning
        {
            Debug.LogWarning("DataType " + data.DataType + " does not match category " + data.Name);
            return;
        }
        int itemLength = data.Skip + formatData.Length;
        try
        {
            for (int i = 0; i < size; i += itemLength)
            {
                double x = ParseItem(mParser.GetItem(obj, i + formatData.X),data.XDateFormat);
                BarInstance.DataSource.SlideValue(data.Name, data.DataObjectName, x, 5f);
            }
        }
        catch(Exception e)
        {
            Debug.LogWarning("Data for category " + data.Name + " does not match the specified format. Ended with exception : " + e.ToString());
        }
    }
    
    public void Fill()
    {
        StartCoroutine(GetData(Application.dataPath + "/poll-leaderboard-test.json"));
    }

    public void Zero()
    {
        ApplyData("");
    }
    void LoadCategoryVisualStyle()
    {
        if (BarPrefab == null)
        {
            Debug.LogError("missing resources for graph and chart, please re-import the package");
        }
        else
        {
            mCategoryVisualStyle = BarPrefab.DataSource.StoreAllCategories();
        }
    }

    void ApplyData(string text)
    {
        if (text == "")
        {
            BarInstance.DataSource.ClearCategories();
            BarInstance.DataSource.ClearValues();
        }
        else
        {
            BarInstance.DataSource.ClearCategories();
            BarInstance.DataSource.ClearValues();

            if (Format == DocumentFormat.JSON)
                mParser = new JsonParser(text);
            else
                mParser = new XMLParser(text);

            LoadCategoryVisualStyle();
            EnsureCreateDataTypes();
            if (mCategoryVisualStyle.Length == 0)
            {
                Debug.LogWarning("no visual styles defined for BarDataFiller, aborting");
                return;
            }

            if (mCategoryVisualStyle.Length < Categories.Length)
                Debug.LogWarning("not enough visual styles in BarDataFiller");

            BarInstance.DataSource.StartBatch();
            for (int i = 0; i < Categories.Length; i++)
            {
                var cat = Categories[i];
                if (cat.Enabled == false)
                    continue;

                var parent = mParser.GetObject(cat.ParentObjectName);
                if (parent == null)
                {
                    Debug.LogWarning("Object " + cat.ParentObjectName + " does not exist in the document");
                    return;
                }

                string oldName = cat.Name;
                string categoryName = cat.Name;
                int length = mParser.GetArraySize(parent);
                try
                {
                    for (int k = 0; k < length; k++)
                    {
                        object item = mParser.GetItemObject(parent, k);
                        var newCategoryName = mParser.GetChildObjectValue(item, "DisplayName");
                        if (!string.IsNullOrEmpty(newCategoryName))
                        {
                            cat.Name = newCategoryName;
                            categoryName = newCategoryName;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning("Data for category " + cat.Name + " does not match the specified format. Ended with exception : " + e.ToString());
                }

                var visualStyle = BarPrefab.DataSource.GetCategoryMaterial(oldName);

                if (BarInstance.DataSource.HasCategory(cat.Name))
                    BarInstance.DataSource.RemoveCategory(cat.Name);

                BarInstance.DataSource.AddCategory(categoryName, visualStyle);
                
                var loader = mLoaders[cat.DataType];    // find the loader based on the data type
                loader(cat); // load the category data
            }
            BarInstance.DataSource.EndBatch();
        }
    }

    IEnumerator GetData(string RemoteUrl)
    {
        WWW request = new WWW(RemoteUrl);
        yield return request;
        if(String.IsNullOrEmpty(request.error))
        {
            try
            {
                string text = request.text;
                ApplyData(text);
            }
            catch(Exception e)
            {
                Debug.LogError("Bar Data Filler : Invalid document format, please check your settings , with exception " +e.ToString());
            }
        }
        else
        {
            Debug.LogError("Bar Data Filler : URL request failed ," + request.error);
        }
    }
}
