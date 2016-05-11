using System;

namespace DTS.Utils.TypeDefs
{
    public class ItemsTypeDef : TypeDefBase
    {
        public Type ItemType { get; set; }

        public ItemsTypeDef(Type type, Type itemType) : base(type)
        {
            ItemType = itemType;
            
            if (itemType.IsNested)
            {
                ItemTypeName = $"{itemType.DeclaringType.Name}.{itemType.Name}";
            }
            else
            {
                ItemTypeName = itemType.Name;
            }
        }

        public string ItemTypeName { get; set; }

        public override bool Params => true;
    }
}