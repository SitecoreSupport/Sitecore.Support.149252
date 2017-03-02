using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Sitecore.Support
{
    public static class ReflectionUtil
    {
        public static object GetField(object obj, Type type, string name)
        {
            if ((obj != null) && (name.Length > 0))
            {
                FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null)
                {
                    return field.GetValue(obj);
                }
            }
            return null;
        }
    }
}