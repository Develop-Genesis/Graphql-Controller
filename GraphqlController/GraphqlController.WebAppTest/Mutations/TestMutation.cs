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
        public Task<IEnumerable<IPerson>> AllPeople(int take) => Task.FromResult(new IPerson[]
        {
            new Teacher(){ Name = "Roberto", LastName = "Gonzales" },
            new Teacher(){ Name = "Jose", LastName = "Martinez" },
            new Teacher(){ Name = "Landy", LastName = "Acosta" },
            new Teacher(){ Name = "Rubio", LastName = "Jaime" },
            new Student(),
            new Student(),
            new Student(),
            new Student()
        }.Take(take));
    }
}
