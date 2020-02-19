using GraphqlController;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GraphQlController.Lab.TestTypes
{
    public class Person : GraphNodeType<object>
    {
        public string Name { get; set; }

        public string LastName { get; set; }

        public async override Task OnCreateAsync(object param)
        {
            //throw new NotImplementedException();
        }
    }
}
