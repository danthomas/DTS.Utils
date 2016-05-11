using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DTS.Utils.TypeDefs
{
    public class TypeBuilder
    {
        private readonly Dictionary<Type, TypeDefBase> _types;

        public TypeBuilder()
        {
            _types = new Dictionary<Type, TypeDefBase>();

            AddType<string>("string", "\"\"");
            AddType<bool>("bool", "false");
            AddType<byte>("byte", "0");
            AddType<short>("short", "0");
            AddType<int>("int", "0");
            AddType<long>("long", "0");
            AddType<decimal>("decimal", "0M");
            AddType<double>("double", "0M");
            AddType<DateTime>("DateTime", "new DateTime()");
            AddType<Guid>("Guid", "new Guid()");
        }

        public List<RefTypeDef> BuildTypeDefs(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes().Where(x => !x.IsInterface && !x.IsGenericType))
            {
                GetType(type);
            }

            return _types.Values.OfType<RefTypeDef>().ToList();
        }

        private void AddType<T>(string name, string defaultValue = "")
        {
            var valueType = BuildType(typeof(T));
            valueType.Name = name;
            valueType.DefaultValue = defaultValue;
        }

        private TypeDefBase BuildType(Type propertyType)
        {
            TypeDefBase typeDefBase;

            var underlyingType = Nullable.GetUnderlyingType(propertyType);

            if (underlyingType != null)
            {
                NullableTypeDef valueTypeDef = new NullableTypeDef(propertyType, (ValueTypeDef)GetType(underlyingType));
                _types.Add(propertyType, valueTypeDef);
                typeDefBase = valueTypeDef;
            }
            else if (propertyType.IsGenericType &&
                     (propertyType.GetGenericTypeDefinition() == typeof(List<>)
                      || propertyType.GetGenericTypeDefinition() == typeof(IList<>)
                      || propertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                typeDefBase = new ListTypeDef(propertyType, propertyType.GenericTypeArguments[0]);
            }
            else if (propertyType == typeof(string))
            {
                ValueTypeDef valueTypeDef = new ValueTypeDef(propertyType);
                _types.Add(propertyType, valueTypeDef);
                typeDefBase = valueTypeDef;
            }
            else if (propertyType.IsClass || propertyType.IsInterface)
            {
                RefTypeDef refTypeDef = new RefTypeDef(propertyType);
                _types.Add(propertyType, refTypeDef);

                typeDefBase = refTypeDef;
                foreach (var propertyInfo in propertyType.GetProperties().Where(x => !new[] { "Equality" }.Any(y => x.PropertyType.Name.Contains(y))))
                {
                    refTypeDef.Props.Add(new Prop(propertyInfo, propertyInfo.Name, GetType(propertyInfo.PropertyType)));
                }
            }
            else if (propertyType.IsValueType)
            {
                ValueTypeDef valueTypeDef = new ValueTypeDef(propertyType);
                _types.Add(propertyType, valueTypeDef);
                typeDefBase = valueTypeDef;
            }
            else
            {
                throw new Exception();
            }


            return typeDefBase;
        }

        private TypeDefBase GetType(Type type)
        {
            TypeDefBase typeDefBase;

            if (_types.ContainsKey(type))
            {
                typeDefBase = _types[type];
            }
            else
            {
                typeDefBase = BuildType(type);
            }

            return typeDefBase;
        }
    }
}