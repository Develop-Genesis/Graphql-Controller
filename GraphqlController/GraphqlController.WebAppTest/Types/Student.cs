using GraphqlController.AspNetCore.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphqlController.WebAppTest.Types
{
    /// <summary>
    /// An student of the school
    /// </summary>    
    [CacheControl(60)]
    public class Student : INode, IPerson
    {
        public string Id { get; set; }
        /// <summary>
        /// Name of the student
        /// </summary>
        public string Name => "Alejandro";

        /// <summary>
        /// Last name of the student
        /// </summary>
        public string LastName => "Guardiola";

    }
}
