using GraphqlController.Attributes;
using GraphqlController.WebAppTest.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphqlController.WebAppTest.Mutations
{
    [Mutation(typeof(Root))]
    public class TestMutation : GraphNodeType
    {
        /// <summary>
        /// Add a person
        /// </summary>
        /// <param name="user">the person to add</param>
        /// <returns></returns>
        public async Task<string> AddPerson(User user) 
        {
            return "OK";
        }
    }

    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    
    public class StudentInput
    {
        /// <summary>
        /// Name of the student
        /// </summary>
        [NonNull]
        public string Name { get; set; }

        /// <summary>
        /// Last name of the student
        /// </summary>
        [NonNull]
        public string LastName { get; set; }

        /// <summary>
        /// Grades
        /// </summary>
        [NonNull]
        public int Grades { get; set; }
    }

}
