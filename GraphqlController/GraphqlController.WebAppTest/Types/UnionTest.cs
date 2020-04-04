using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphqlController.WebAppTest.Types
{
    public class TypeA
    {
        public string A { get; set; }
    }

    public class TypeB
    {
        public string B { get; set; }
    }

    public class UnionT : Union<TypeA, TypeB> 
    {
        public UnionT(TypeA v) : base(v)
        {

        }

        public UnionT(TypeB v) : base(v)
        {

        }
    }

}
