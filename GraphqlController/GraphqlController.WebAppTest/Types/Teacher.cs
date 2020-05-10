using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphqlController.WebAppTest.Types
{
    /// <summary>
    /// Represents a teacher
    /// </summary>
    public class Teacher
    {
        public string Id { get; set; }
        /// <summary>
        /// Name of the teacher
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Last Name of the Teacher
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Students that this teacher has
        /// </summary>
        public Student[] Students => new Student[]
        {
            new Student(),
            new Student(),
            new Student(), 
            new Student()
        };

    }
}
