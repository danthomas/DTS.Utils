﻿using System;

namespace DTS.Utils.Details
{
    public class RunProcessDetails
    {
        public string Exe { get; set; }
        public string Args { get; set; }
        public Action<string> SetOutput { get; set; }
    } 
}