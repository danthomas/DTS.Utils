using System;

namespace DTS.Utils.TypeDefs
{
    public class ValueTypeDef : TypeDefBase
    {
        public bool Nullable { get; set; }

        public ValueTypeDef(Type type) : base(type)
        {
        }
    }
}