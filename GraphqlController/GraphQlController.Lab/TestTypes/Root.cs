using GraphqlController;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GraphQlController.Lab.TestTypes
{
    public class Root : GraphNodeType<object>
    {
        public string Name { get; set; } = "Alejandro";

        public string LastName { get; set; } = "Guardiola";

        public async override Task OnCreateAsync(object param)
        {
            //throw new NotImplementedException();
        }
    }
}
