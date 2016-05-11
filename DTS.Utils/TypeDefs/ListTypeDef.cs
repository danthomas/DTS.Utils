using System;

namespace DTS.Utils.TypeDefs
{
    public class ListTypeDef : ItemsTypeDef
    {
        public ListTypeDef(Type type, Type itemType) : base(type, itemType)
        {

            if (itemType.IsNested)
            {
                Name = $"{itemType.DeclaringType.Name}.{itemType.Name}";
            }
            else
            {
                Name = itemType.Name;
            }

            Name = $"List<{Name}>";
        }

        public override string ParamToMethod => "ToList";
    }
}