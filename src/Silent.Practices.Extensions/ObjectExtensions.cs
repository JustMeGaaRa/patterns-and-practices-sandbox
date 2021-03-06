﻿using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Silent.Practices.Extensions
{
    public static class ObjectExtensions
    {
        public static PropertyInfo[] GetProperties(this Type instance)
        {
            return instance.GetTypeInfo().GetProperties();
        }

        public static void Patch<TSource>(this TSource original, TSource modified)
        {
            if (ReferenceEquals(original, modified))
            {
                return;
            }

            var properties = typeof(TSource).GetProperties()
                .Where(p => p.CanRead && p.CanWrite)
                .ToList();

            foreach (var property in properties)
            {
                property.SetValue(original, property.GetValue(modified, null), null);
            }
        }

        public static T[] AsArray<T>(this T instance)
        {
            return new[] { instance };
        }

        public static List<T> AsList<T>(this T instance)
        {
            return new List<T>(1) { instance };
        }
    }
}
