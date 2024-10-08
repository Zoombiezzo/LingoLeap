using System;
using System.Collections.Generic;

namespace _Client.Scripts.Infrastructure.Helpers
{
    public static class TypesHelper<T> where T : class
    {
        public static IEnumerable<Type> GetTypesChild()
        {
            var list = new List<Type>(128);

            var assembly = typeof(T).Assembly;

            var classType = typeof(T);
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(classType) || (classType.IsAssignableFrom(type) && classType != type))
                    list.Add(type);
            }

            return list;
        }

        public static IEnumerable<Type> GetTypes()
        {
            var list = new List<Type>( 128 );

            var assembly = typeof( T ).Assembly;

            foreach ( var type in assembly.GetTypes() )
            {
                list.Add( type );
            }

            return list;
        }
    }
}