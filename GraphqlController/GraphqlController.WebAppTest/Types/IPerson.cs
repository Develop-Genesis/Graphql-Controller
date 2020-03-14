using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphqlController.WebAppTest.Types
{
    /// <summary>
    /// A person in the school
    /// </summary>
    public interface IPerson
    {
        /// <summary>
        /// Name of the person
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Last Name of the person
        /// </summary>
        public string LastName { get; }
    }
}
