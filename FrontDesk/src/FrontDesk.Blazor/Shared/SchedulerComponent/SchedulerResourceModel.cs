﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontDesk.Blazor.Shared.SchedulerComponent
{
    public class SchedulerResourceModel
    {
        public SchedulerResourceModel(string name, string textField, string valueField, string colorField, string field, string title, object data)
        {
            Name = name;
            TextField = textField;
            ValueField = valueField;
            ColorField = colorField;
            Field = field;
            Title = title;
            Data = data;
        }

        public string TextField { get; set; }
        public string ValueField { get; set; }
        public string ColorField { get; set; }
        public string Field { get; set; }
        public string Title { get; set; }
        public object Data { get; set; }
        public string Name { get; set; }
    }   
}
