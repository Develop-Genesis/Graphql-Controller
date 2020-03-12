using GraphqlController;
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
    public class Person : GraphNodeType
    {
        public string Name { get; set; }

        public string LastName { get; set; }

        /// <summary>
        /// The name of the mother
        /// </summary>
        /// <param name="jojoto">Just for testing</param>        
        public string MotherName(string jojoto) => "Hilsy";
    }
}
