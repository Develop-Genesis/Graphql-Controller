using GraphqlController.WebAppTest.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphqlController.WebAppTest.Repositories
{
    public class TeacherRepository
    {
        public IEnumerable<Teacher> GetTeachers() => new Teacher[]
        {
            new Teacher(){ Name = "Roberto", LastName = "Gonzales" },
            new Teacher(){ Name = "Jose", LastName = "Martinez" },
            new Teacher(){ Name = "Landy", LastName = "Acosta" },
            new Teacher(){ Name = "Rubio", LastName = "Jaime" },
        };
    }
}
