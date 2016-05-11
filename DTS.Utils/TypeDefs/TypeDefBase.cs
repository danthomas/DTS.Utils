using System;

namespace DTS.Utils.TypeDefs
{
    public class TypeDefBase
    {
        public Type Type { get; set; }

        public TypeDefBase(Type type)
        {
            Type = type;
            if (type.IsNested)
            {
                Name = $"{type.DeclaringType.Name}.{type.Name}";
            }
            else
            {
                Name = type.Name;
            }
        }

        public string Name { get; set; }
        public string DefaultValue { get; set; }
        public virtual bool Params => false;
        public virtual string ParamToMethod => "";
    }
}