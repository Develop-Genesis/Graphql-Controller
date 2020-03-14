using GraphqlController;
using GraphqlController.Arguments;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphQlController.Lab.TestTypes
{
    /// <summary>
    /// Represents a person
    /// </summary>
    public class Person : GraphNodeType, TestInterface
    {
        public string Name => "Alejandro";

        public string LastName => "Guardiola";

        public TestInterface Father => new Person();

        /// <summary>
        /// The name of the mother
        /// </summary>
        /// <param name="jojoto">Just for testing</param>        
        public string MotherName([NonNullArgument]string jojoto) => "Hilsy";
    }
}
