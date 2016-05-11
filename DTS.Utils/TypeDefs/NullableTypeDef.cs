using System;

namespace DTS.Utils.TypeDefs
{
    public class NullableTypeDef : ValueTypeDef
    {
        public ValueTypeDef UnderlyingTypeDef { get; set; }

        public NullableTypeDef(Type type, ValueTypeDef underlyingTypeDef) : base(type)
        {
            UnderlyingTypeDef = underlyingTypeDef;
            Name = underlyingTypeDef.Name + "?";
        }
    }
}