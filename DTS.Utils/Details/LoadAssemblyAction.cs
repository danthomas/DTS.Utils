using System;
using System.Reflection;

namespace DTS.Utils.Details
{
    public class LoadAssemblyAction
    {
        public string FilePath { get; set; }
        public Action<Assembly> Action { get; set; }
    }
}