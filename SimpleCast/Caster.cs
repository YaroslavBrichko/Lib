using System;
using System.Linq.Expressions;


namespace SimpleCast
{
    static class Caster<TFrom, TTo>
    {
        public static readonly Func<TFrom, TTo> Cast = compile();
        private static Func<TFrom, TTo> compile()
        {
            var param = Expression.Parameter(typeof(TFrom));
            var lambda = Expression.Lambda<Func<TFrom, TTo>>(param, param);
            return lambda.Compile();
        }
    }
}
