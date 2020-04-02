using GraphqlController.Attributes;
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
    public class Root : GraphNodeType
    {
        TeacherRepository _teacherRepository;

        public Root(TeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }

        /// <summary>
        /// The teachers in the school
        /// </summary>
        public Teacher[] Teachers => _teacherRepository.GetTeachers().ToArray();

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

        /// <summary>
        /// Testing the input
        /// </summary>
        /// <param name="param1">Parameter 1</param>
        /// <param name="param2">Parameter 2</param>
        /// <returns></returns>
        public IPerson TestInput([NonNull]TestInputType param1, string param2)
        {
            return new Teacher() { Name = param1.Hola, LastName = param1.HolaHi };
        } 

    }

    /// <summary>
    /// Test input type
    /// </summary>
    public class TestInputType
    {
        [NonNull]
        public string Hola { get; set; }

        [NonNull]
        public string HolaHi { get; set; }
    }
}
