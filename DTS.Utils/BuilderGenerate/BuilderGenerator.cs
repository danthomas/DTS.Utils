using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using DTS.Utils.Core;
using DTS.Utils.TypeDefs;

namespace MPD.V2.Utils.Cli.BuilderGenerate
{
    class BuilderGenerator
    {
        private readonly TypeBuilder _typeBuilder;

        public BuilderGenerator(TypeBuilder typeBuilder)
        {
            _typeBuilder = typeBuilder;
        }

        public List<GenFile> GenBuilders(Assembly assembly)
        {
            List<GenFile> files = new List<GenFile>();

            List<RefTypeDef> refTypes = _typeBuilder.BuildTypeDefs(assembly);

            var refTypeDefs = refTypes.Where(x => !x.Type.FullName.StartsWith("System.")
                                                  && (x.Type.IsPublic || x.Type.IsNestedPublic)
                                                  && !x.Type.IsAbstract).ToArray();

            foreach (var refType in refTypeDefs)
            {
                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.AppendLine(@"/*Generated using MPD.V2.Utils.Cli*/
using System;
using System.Collections.Generic;
using System.Linq;");
                List<string> namespaces = refType.Props
                   .Select(x => x.TypeDef.Type.Namespace)
                   .Where(x => x.StartsWith("MPD") && x != refType.Type.Namespace)
                   .Distinct()
                   .ToList();

                namespaces.AddRange(
                    refType.Props
                        .Where(x => x.PropertyInfo.PropertyType.IsGenericTypeDefinition)
                        .Select(x => x.TypeDef.Type.GetGenericArguments()[0].Namespace)
                        .Where(x => x.StartsWith("MPD") && x != refType.Type.Namespace)
                        .Distinct());

                namespaces.Add("NUnit.Framework");

                foreach (var ns in namespaces.OrderBy(x => x))
                {
                    stringBuilder.AppendLine($@"using {ns};");
                }

                var props = refType.Props.Where(x => !x.TypeDef.Name.StartsWith("Dictionary")
                && x.PropertyInfo.CanWrite
                && x.PropertyInfo.GetSetMethod() != null
                && x.PropertyInfo.GetSetMethod().IsPublic
                ).ToList();

                stringBuilder.AppendLine(
                    $@"
// ReSharper disable once CheckNamespace
namespace {refType.Type.Namespace}.Builders
{{
    public class {refType.IdentifierName}Builder
    {{");
                foreach (var prop in props)
                {
                    stringBuilder.AppendLine($@"        private {prop.TypeDef.Name} _{prop.Name.ToCamelCase()};");
                }

                stringBuilder.AppendLine();

                foreach (var prop in props)
                {
                    stringBuilder.AppendLine(
                        $@"        public static {prop.TypeDef.Name} Default{prop.Name} {{ get; set; }}");
                }

                stringBuilder.AppendLine($@"
        static {refType.IdentifierName}Builder()
        {{");

                foreach (var prop in props.Where(x => !String.IsNullOrEmpty(x.TypeDef.DefaultValue)))
                {
                    stringBuilder.AppendLine($@"            Default{prop.Name} = {prop.TypeDef.DefaultValue};");
                }

                stringBuilder.AppendLine($@"        }}");

                stringBuilder.AppendLine($@"
        private {refType.IdentifierName}Builder()
        {{");

                foreach (var prop in props)
                {
                    stringBuilder.AppendLine($@"            _{prop.Name.ToCamelCase()} = Default{prop.Name};");
                }

                stringBuilder.AppendLine($@"        }}

        public static {refType.Name} Default()
        {{
            return New.Build();
        }}

        public static {refType.IdentifierName}Builder New
        {{
            get {{ return new {refType.IdentifierName}Builder(); }} 
        }}");

                foreach (var prop in props)
                {
                    stringBuilder.AppendLine(
                        $@"
        public {refType.IdentifierName}Builder With{prop.Name}({(prop.TypeDef.Params ? $"params {((ItemsTypeDef)prop.TypeDef).ItemTypeName}[]" : prop.TypeDef.Name)} {prop.Name.ToCamelCase()})
        {{
            _{prop.Name.ToCamelCase()} = {(prop.TypeDef.Params ? $"{prop.Name.ToCamelCase()} == null ? null :" : "") + prop.Name.ToCamelCase() + (prop.TypeDef.Params ? "." + prop.TypeDef.ParamToMethod + "()" : "")};
            return this;
        }}");
                }


                stringBuilder.AppendLine($@"
        public {refType.Name} Build()
        {{
            return new {refType.Name}
            {{");

                foreach (var prop in props)
                {
                    stringBuilder.AppendLine($@"                {prop.Name} = _{prop.Name.ToCamelCase()},");
                }

                stringBuilder.AppendLine($@"            }};");


                stringBuilder.AppendLine($@"
        }}");

                stringBuilder.AppendLine($@"    }}
}}");
                string assemblyName = refType.Type.Assembly.GetName().Name;
                string namespaceName = refType.Type.FullName.Replace(assemblyName + ".", "").Replace("." + refType.Type.Name, "");

                string path = assemblyName + (String.IsNullOrEmpty(namespaceName) ? "" : $@"\{namespaceName}");

                path = $@"{path}\{refType.Name}Builder.cs";

                files.Add(new GenFile(path, stringBuilder.ToString()));
            }

            return files;
        }
    }
}