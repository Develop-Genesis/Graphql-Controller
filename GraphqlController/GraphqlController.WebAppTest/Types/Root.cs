using GraphqlController.AspNetCore.Authorization;
using GraphqlController.AspNetCore.Cache;
using GraphqlController.Attributes;
using GraphqlController.Services;
using GraphqlController.WebAppTest.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphqlController.WebAppTest.Types
{
    /// <summary>
    /// The root type
    /// </summary>
    [RootType]    
    public class Root : BaseTest
    {

        /// <summary>
        /// All people in the school
        /// </summary>
        [Authorize(Roles = "Admin")]
        public IEnumerable<Teacher> AllTeacher(int skip, int take)
        {
            var list = new Teacher[]
            {
                new Teacher(){ Name = "Roberto", LastName = "Gonzales" },
                new Teacher(){ Name = "Jose", LastName = "Martinez" },
                new Teacher(){ Name = "Landy", LastName = "Acosta" },
                new Teacher(){ Name = "Rubio", LastName = "Jaime" }
            };

            return list.Skip(skip).Take(take);
        }

        public static string GenerateName(Type type)
        {
            return "ElRoot";
        }


    }

    public class BaseTest : GraphNodeType
    {
        public string TestProperty => "Test";
        public string TestMethod(int a) => "Test";
    }

}
