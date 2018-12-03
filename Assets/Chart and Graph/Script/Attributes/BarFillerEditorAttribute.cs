using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChartAndGraph
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class BarFillerEditorAttribute : Attribute
    {
        public BarDataFiller.DataType ShowForType;
        public BarFillerEditorAttribute(BarDataFiller.DataType type)
        {
            ShowForType = type;
        }
    }
}
