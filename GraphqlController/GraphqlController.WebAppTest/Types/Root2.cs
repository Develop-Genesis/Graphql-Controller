using GraphqlController.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphqlController.WebAppTest.Types
{
    [RootType]
    public class Root2 : GraphNodeType
    {
        public string RandomValue => "Root2";
    }
}
