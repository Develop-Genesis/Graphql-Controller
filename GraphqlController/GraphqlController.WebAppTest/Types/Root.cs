using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphqlController.WebAppTest.Types
{
    public class Root : GraphNodeType
    {
        /// <summary>
        /// The teachers in the school
        /// </summary>
        public Teacher[] Teachers => new Teacher[]
        {
            new Teacher(){ Name = "Roberto", LastName = "Gonzales" },
            new Teacher(){ Name = "Jose", LastName = "Martinez" },
            new Teacher(){ Name = "Landy", LastName = "Acosta" },
            new Teacher(){ Name = "Rubio", LastName = "Jaime" },
        };

        /// <summary>
        /// All people in the school
        /// </summary>
        public IPerson[] AllPeople => new IPerson[]
        {
            new Teacher(){ Name = "Roberto", LastName = "Gonzales" },
            new Teacher(){ Name = "Jose", LastName = "Martinez" },
            new Teacher(){ Name = "Landy", LastName = "Acosta" },
            new Teacher(){ Name = "Rubio", LastName = "Jaime" },
            new Student(), 
            new Student(),
            new Student(),
            new Student()
        };
    }
}
