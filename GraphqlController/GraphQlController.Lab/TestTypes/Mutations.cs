using GraphqlController;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQlController.Lab.TestTypes
{
    /// <summary>
    /// Mutations type
    /// </summary>
    public class Mutations : GraphNodeType
    {
        public int CreateCorral()
        {
            return 5;
        }
    }
}
