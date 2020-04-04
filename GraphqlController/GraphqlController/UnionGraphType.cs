using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController
{
    public interface IUnionGraphType
    {
        object Value { get; }        
    }

    public class Union<T, T1> : IUnionGraphType
        where T : class
        where T1 : class
    {
        public object Value { get; private set; }

        public Union(T value)
        {
            Value = value;
        }

        public Union(T1 value)
        {
            Value = value;
        }
    }

    public class Union<T, T1, T2> : IUnionGraphType
         where T : class
         where T1 : class
         where T2 : class
    {
        public object Value { get; private set; }

        public Union(T value)
        {
            Value = value;
        }

        public Union(T1 value)
        {
            Value = value;
        }

        public Union(T2 value)
        {
            Value = value;
        }
    }



    public class Union<T, T1, T2, T3> : IUnionGraphType
         where T : class
         where T1 : class
         where T2 : class
         where T3 : class
    {
        public object Value { get; private set; }

        public Union(T value)
        {
            Value = value;
        }

        public Union(T1 value)
        {
            Value = value;
        }

        public Union(T2 value)
        {
            Value = value;
        }

        public Union(T3 value)
        {
            Value = value;
        }
    }

    public class Union<T, T1, T2, T3, T4> : IUnionGraphType
         where T : class
         where T1 : class
         where T2 : class
         where T3 : class
         where T4 : class
    {
        public object Value { get; private set; }

        public Union(T value)
        {
            Value = value;
        }

        public Union(T1 value)
        {
            Value = value;
        }

        public Union(T2 value)
        {
            Value = value;
        }

        public Union(T3 value)
        {
            Value = value;
        }

        public Union(T4 value)
        {
            Value = value;
        }
    }

    public class Union<T, T1, T2, T3, T4, T5> : IUnionGraphType
         where T : class
         where T1 : class
         where T2 : class
         where T3 : class
         where T4 : class
         where T5 : class
    {
        public object Value { get; private set; }

        public Union(T value)
        {
            Value = value;
        }

        public Union(T1 value)
        {
            Value = value;
        }

        public Union(T2 value)
        {
            Value = value;
        }

        public Union(T3 value)
        {
            Value = value;
        }

        public Union(T4 value)
        {
            Value = value;
        }

        public Union(T5 value)
        {
            Value = value;
        }
    }

    public class Union<T, T1, T2, T3, T4, T5, T6> : IUnionGraphType
        where T : class
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
    {
        public object Value { get; private set; }

        public Union(T value)
        {
            Value = value;
        }

        public Union(T1 value)
        {
            Value = value;
        }

        public Union(T2 value)
        {
            Value = value;
        }

        public Union(T3 value)
        {
            Value = value;
        }


        public Union(T4 value)
        {
            Value = value;
        }

        public Union(T5 value)
        {
            Value = value;
        }

        public Union(T6 value)
        {
            Value = value;
        }
    }

    public class Union<T, T1, T2, T3, T4, T5, T6, T7> : IUnionGraphType
        where T : class
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
        where T7 : class
    {
        public object Value { get; private set; }

        public Union(T value)
        {
            Value = value;
        }

        public Union(T1 value)
        {
            Value = value;
        }

        public Union(T2 value)
        {
            Value = value;
        }

        public Union(T3 value)
        {
            Value = value;
        }


        public Union(T4 value)
        {
            Value = value;
        }

        public Union(T5 value)
        {
            Value = value;
        }

        public Union(T6 value)
        {
            Value = value;
        }

        public Union(T7 value)
        {
            Value = value;
        }
    }

    public class Union<T, T1, T2, T3, T4, T5, T6, T7, T8> : IUnionGraphType
        where T : class
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
        where T7 : class
        where T8 : class
    {
        public object Value { get; private set; }

        public Union(T value)
        {
            Value = value;
        }

        public Union(T1 value)
        {
            Value = value;
        }

        public Union(T2 value)
        {
            Value = value;
        }

        public Union(T3 value)
        {
            Value = value;
        }


        public Union(T4 value)
        {
            Value = value;
        }

        public Union(T5 value)
        {
            Value = value;
        }

        public Union(T6 value)
        {
            Value = value;
        }

        public Union(T7 value)
        {
            Value = value;
        }


        public Union(T8 value)
        {
            Value = value;
        }
    }

    public class Union<T, T1, T2, T3, T4, T5, T6, T7, T8, T9> : IUnionGraphType
        where T : class
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
        where T7 : class
        where T8 : class
        where T9 : class
    {
        public object Value { get; private set; }

        public Union(T value)
        {
            Value = value;
        }

        public Union(T1 value)
        {
            Value = value;
        }

        public Union(T2 value)
        {
            Value = value;
        }

        public Union(T3 value)
        {
            Value = value;
        }

        public Union(T4 value)
        {
            Value = value;
        }

        public Union(T5 value)
        {
            Value = value;
        }

        public Union(T6 value)
        {
            Value = value;
        }

        public Union(T7 value)
        {
            Value = value;
        }
        
        public Union(T8 value)
        {
            Value = value;
        }

        public Union(T9 value)
        {
            Value = value;
        }
    }
}
