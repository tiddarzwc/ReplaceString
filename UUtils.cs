using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Mono.Cecil;

namespace _ReplaceString_
{
    public static class UUtils
    {
        public static string RemoveChars(this string str, params char[] chars)
        {
            StringBuilder sb = new StringBuilder(str.Length);
            int currentPos = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (chars.Contains(str[i]))
                {
                    if (currentPos != i)
                    {
                        sb.Append(str[currentPos..i]);
                        currentPos = i + 1;
                    }
                    else
                    {
                        currentPos++;
                    }
                }
            }
            sb.Append(str[currentPos..]);
            return sb.Length == 0 ? str[currentPos..] : sb.ToString();
        }
        public static string GetSpecialMethodName(MethodBase method)
        {
            StringBuilder name = new StringBuilder();
            name.Append(method.Name.RemoveChars(',', '`', '.'));
            if (method.IsGenericMethod)
            {
                name.Append("_T");
            }
            else if (method.IsGenericMethodDefinition)
            {
                name.Append("_T");
                foreach (var param in method.GetGenericArguments())
                {
                    name.Append('_');
                    name.Append(GetSpecialTypeName(param));
                }
            }
            foreach (var param in method.GetParameters())
            {
                name.Append('_');
                name.Append(GetSpecialTypeName(param.ParameterType));
            }
            if (method is MethodInfo temp)
            {
                name.Append('_').Append(GetSpecialTypeName(temp.ReturnType));
            }
            return name.ToString();
        }
        public static string GetSpecialMethodName(MethodDefinition method)
        {
            StringBuilder name = new StringBuilder();
            name.Append(method.Name.RemoveChars(',', '`', '.'));
            if (method.HasGenericParameters)
            {
                name.Append("_T");
            }
            else if (method.IsGenericInstance)
            {
                name.Append("_T");
                foreach (var param in method.GenericParameters)
                {
                    name.Append('_');
                    name.Append(GetSpecialTypeName(param.DeclaringType));
                }
            }
            foreach (var param in method.Parameters)
            {
                name.Append('_');
                name.Append(GetSpecialTypeName(param.ParameterType));
            }
            Debug.Assert(!name.ToString().Contains('.'));
            return name.ToString();
        }
        public static string GetSpecialTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(type.Name.RemoveChars('[', ',', '`').Replace(']', 's').Replace("&", "Ref"));
                sb.Append('g');
                foreach (var t in type.GetGenericArguments())
                {
                    sb.Append('_');
                    sb.Append(GetSpecialTypeName(t));
                }
                return sb.ToString();
            }
            return type.Name.RemoveChars('[', ']', '`').Replace("&", "Ref");
        }
        public static string GetSpecialTypeName(TypeReference type)
        {
            if (type.HasGenericParameters)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(type.Name.RemoveChars('[', ',', '`').Replace(']', 's').Replace("&", "Ref"));
                sb.Append('g');
                foreach (var t in type.GenericParameters)
                {
                    sb.Append('_');
                    sb.Append(GetSpecialTypeName(t));
                }
                return sb.ToString();
            }
            return type.Name.RemoveChars('[', ']', '`').Replace("&", "Ref");
        }
    }
}
