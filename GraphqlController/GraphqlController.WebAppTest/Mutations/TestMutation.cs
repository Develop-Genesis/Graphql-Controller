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

}
