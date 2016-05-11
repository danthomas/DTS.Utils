using System;
using System.Collections.Generic;

namespace DTS.Utils.TypeDefs
{
    public class RefTypeDef : TypeDefBase
    {

        public RefTypeDef(Type type) : base(type)
        {
            Type = type;
            Props = new List<Prop>();
        }

        public List<Prop> Props { get; set; }

        public override string ToString()
        {
            return Type.FullName;
        }
    }
}