namespace Spatial.Roslyn.Tests.ApiCompare
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    public static class ReflectionExtensions
    {
        /// <summary>
        /// Return the method signature as a string.
        /// </summary>
        /// <param name="method">The Method</param>
        /// <returns>Method signature</returns>
        public static string GetSignature(this MethodInfo method)
        {
            var firstParam = true;
            var sigBuilder = new StringBuilder();

            if (method.IsPublic)
            {
                sigBuilder.Append("public ");
            }
            else if (method.IsPrivate)
            {
                sigBuilder.Append("private ");
            }
            else if (method.IsAssembly)
            {
                sigBuilder.Append("internal ");
            }

            if (method.IsFamily)
            {
                sigBuilder.Append("protected ");
            }

            if (method.IsStatic)
            {
                sigBuilder.Append("static ");
            }

            sigBuilder.Append(TypeName(method.ReturnType));
            sigBuilder.Append(' ');
            sigBuilder.Append(method.Name);

            // Add method generics
            if (method.IsGenericMethod)
            {
                sigBuilder.Append("<");
                foreach (var g in method.GetGenericArguments())
                {
                    if (firstParam)
                    {
                        firstParam = false;
                    }
                    else
                    {
                        sigBuilder.Append(", ");
                    }

                    sigBuilder.Append(TypeName(g));
                }

                sigBuilder.Append(">");
            }

            sigBuilder.Append("(");
            firstParam = true;
            var secondParam = false;
            foreach (var param in method.GetParameters())
            {
                if (firstParam)
                {
                    firstParam = false;
                    if (method.IsDefinedReflectOnly(typeof(System.Runtime.CompilerServices.ExtensionAttribute)))
                    {
                        sigBuilder.Append("this ");
                    }
                }
                else if (secondParam == true)
                {
                    secondParam = false;
                }
                else
                {
                    sigBuilder.Append(", ");
                }

                if (param.ParameterType.IsByRef)
                {
                    sigBuilder.Append("ref ");
                }
                else if (param.IsOut)
                {
                    sigBuilder.Append("out ");
                }

                sigBuilder.Append(TypeName(param.ParameterType));

                // sigBuilder.Append(' ');
                // sigBuilder.Append(param.Name);
            }

            sigBuilder.Append(")");
            return sigBuilder.ToString();
        }

        /// <summary>
        /// Return the method signature as a string.
        /// </summary>
        /// <param name="method">The Method</param>
        /// <returns>Method signature</returns>
        public static string GetSignature(this ConstructorInfo method)
        {
            var firstParam = true;
            var sigBuilder = new StringBuilder();
            if (method.IsDefinedReflectOnly(typeof(ObsoleteAttribute)))
            {
                sigBuilder.Append("[Obsolete]");
            }

            if (method.IsPublic)
            {
                sigBuilder.Append("public ");
            }
            else if (method.IsPrivate)
            {
                sigBuilder.Append("private ");
            }
            else if (method.IsAssembly)
            {
                sigBuilder.Append("internal ");
            }

            if (method.IsFamily)
            {
                sigBuilder.Append("protected ");
            }

            if (method.IsStatic)
            {
                sigBuilder.Append("static ");
            }

            sigBuilder.Append(' ');
            sigBuilder.Append(method.Name);

            // Add method generics
            if (method.IsGenericMethod)
            {
                sigBuilder.Append("<");
                foreach (var g in method.GetGenericArguments())
                {
                    if (firstParam)
                    {
                        firstParam = false;
                    }
                    else
                    {
                        sigBuilder.Append(", ");
                    }

                    sigBuilder.Append(TypeName(g));
                }

                sigBuilder.Append(">");
            }

            sigBuilder.Append("(");
            firstParam = true;
            var secondParam = false;
            foreach (var param in method.GetParameters())
            {
                if (firstParam)
                {
                    firstParam = false;
                    if (method.IsDefinedReflectOnly(typeof(System.Runtime.CompilerServices.ExtensionAttribute)))
                    {
                        sigBuilder.Append("this ");
                    }
                }
                else if (secondParam == true)
                {
                    secondParam = false;
                }
                else
                {
                    sigBuilder.Append(", ");
                }

                if (param.ParameterType.IsByRef)
                {
                    sigBuilder.Append("ref ");
                }
                else if (param.IsOut)
                {
                    sigBuilder.Append("out ");
                }

                sigBuilder.Append(TypeName(param.ParameterType));

                // sigBuilder.Append(' ');
                // sigBuilder.Append(param.Name);
            }

            sigBuilder.Append(")");
            return sigBuilder.ToString();
        }

        public static bool IsDefinedReflectOnly(this Assembly a, Type attributeType)
        {
            var attribset = CustomAttributeData.GetCustomAttributes(a);
            foreach (var attrib in attribset)
            {
                if (attrib.AttributeType == attributeType)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsDefinedReflectOnly(this MethodInfo m, Type attributeType)
        {
            var attribset = CustomAttributeData.GetCustomAttributes(m);
            foreach (var attrib in attribset)
            {
                if (attrib.AttributeType == attributeType)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsDefinedReflectOnly(this ConstructorInfo m, Type attributeType)
        {
            var attribset = CustomAttributeData.GetCustomAttributes(m);
            foreach (var attrib in attribset)
            {
                if (attrib.AttributeType == attributeType)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsDefinedReflectOnly(this Type m, Type attributeType)
        {
            var attribset = CustomAttributeData.GetCustomAttributes(m);
            foreach (var attrib in attribset)
            {
                if (attrib.AttributeType == attributeType)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get full type name with full namespace names
        /// </summary>
        /// <param name="type">Type. May be generic or nullable</param>
        /// <returns>Full type name, fully qualified namespaces</returns>
        public static string TypeName(Type type)
        {
            var nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType != null)
            {
                return nullableType.Name + "?";
            }

            if (!(type.IsGenericType && type.Name.Contains('`')))
            {
                switch (type.Name)
                {
                    case "String": return "string";
                    case "Int32": return "int";
                    case "Decimal": return "decimal";
                    case "Object": return "object";
                    case "Void": return "void";
                    default:
                        {
                            return type.Name;

                            // return string.IsNullOrWhiteSpace(type.FullName) ? type.Name : type.FullName;
                        }
                }
            }

            var sb = new StringBuilder(type.Name.Substring(0, type.Name.IndexOf('`')));
            sb.Append('<');
            var first = true;
            foreach (var t in type.GetGenericArguments())
            {
                if (!first)
                {
                    sb.Append(',');
                }

                sb.Append(TypeName(t));
                first = false;
            }

            sb.Append('>');
            return sb.ToString();
        }

        public static string FullMethodName(this MethodInfo method)
        {
            var firstParam = true;
            var sigBuilder = new StringBuilder();
            string methodname = method.Name.Split('.').Last();
            sigBuilder.Append(methodname);

            // Add method generics
            if (method.IsGenericMethod)
            {
                sigBuilder.Append("<");
                foreach (var g in method.GetGenericArguments())
                {
                    if (firstParam)
                    {
                        firstParam = false;
                    }
                    else
                    {
                        sigBuilder.Append(", ");
                    }

                    sigBuilder.Append(TypeName(g));
                }

                sigBuilder.Append(">");
            }

            return sigBuilder.ToString();
        }

        public static string PartialMethodSig(this MethodInfo method)
        {
            var firstParam = true;
            var sigBuilder = new StringBuilder();
            sigBuilder.Append(method.FullMethodName());

            sigBuilder.Append("(");
            firstParam = true;
            foreach (var param in method.GetParameters())
            {
                if (param.IsOptional)
                {
                    continue;
                }

                if (param.RawDefaultValue != System.DBNull.Value)
                {
                    continue;
                }

                if (firstParam)
                {
                    firstParam = false;
                    if (method.IsDefinedReflectOnly(typeof(System.Runtime.CompilerServices.ExtensionAttribute)))
                    {
                        sigBuilder.Append("this ");
                    }
                }
                else
                {
                    sigBuilder.Append(", ");
                }

                if (param.ParameterType.IsByRef)
                {
                    sigBuilder.Append("ref ");
                }
                else if (param.IsOut)
                {
                    sigBuilder.Append("out ");
                }

                sigBuilder.Append(TypeName(param.ParameterType));

                // sigBuilder.Append(' ');
                // sigBuilder.Append(param.Name);
            }

            return sigBuilder.ToString();
        }
    }
}
