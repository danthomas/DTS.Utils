using System.Reflection;

namespace DTS.Utils.TypeDefs
{
    public class Prop
    {
        public PropertyInfo PropertyInfo { get; set; }
        public string Name { get; set; }
        public TypeDefBase TypeDef { get; set; }

        public Prop(PropertyInfo propertyInfo, string name, TypeDefBase typeDef)
        {
            PropertyInfo = propertyInfo;
            Name = name;
            TypeDef = typeDef;
        }
    }
}