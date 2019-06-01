using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using NUnit.Framework;

namespace Spatial.Benchmarks
{
    public class CodeGen
    {
        [Explicit("Script")]
        [TestCase(typeof(Point2D))]
        [TestCase(typeof(Vector2D))]
        [TestCase(typeof(Point3D))]
        [TestCase(typeof(Vector3D))]
        [TestCase(typeof(UnitVector3D))]
        [TestCase(typeof(Ray3D))]
        [TestCase(typeof(Plane))]
        public void DumpBenchmark(Type type)
        {
            var builder = new StringBuilder();

            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                builder.AppendLine("        [Benchmark]")
                    .AppendLine($"        public {property.PropertyType} {property.Name}()")
                    .AppendLine("        {")
                    .AppendLine($"             return {type.Name}1.{property.Name};")
                    .AppendLine("        }")
                    .AppendLine();
            }

            foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public).Where(m => m.Name.StartsWith("op")))
            {
                if (method.Name == "op_Inequality")
                {
                    continue;
                }

                if (method.Name == "op_UnaryNegation")
                {
                    builder.AppendLine("        [Benchmark]")
                        .AppendLine($"        public {method.ReturnType.Name} Operator{method.Name.Substring(3)}()")
                        .AppendLine("        {")
                        .AppendLine($"             return -{type.Name}1;")
                        .AppendLine("        }")
                        .AppendLine();
                }
                else
                {
                    builder.AppendLine("        [Benchmark]")
                        .AppendLine($"        public {method.ReturnType.Name} Operator{method.Name.Substring(3)}{method.GetParameters()[0].ParameterType.Name}{method.GetParameters()[1].ParameterType.Name}()")
                        .AppendLine("        {")
                        .AppendLine($"             return {type.Name}1 {Operator(method)} {Argument(method.GetParameters().Last())};")
                        .AppendLine("        }")
                        .AppendLine();
                }
            }

            foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public).Where(m => !m.IsSpecialName))
            {
                if (method.GetParameters().Any(p => p.ParameterType == typeof(XmlReader)) ||
                    method.Name == "TryParse")
                {
                    continue;
                }

                var parameters = string.Join(", ", method.GetParameters().Select(Argument));
                builder.AppendLine("        [Benchmark]")
                    .AppendLine($"        public {method.ReturnType.Name} {method.Name}()")
                    .AppendLine("        {")
                    .AppendLine($"             return {type.Name}.{method.Name}({parameters});")
                    .AppendLine("        }")
                    .AppendLine();
            }

            foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                if (method.Name == "GetType" ||
                    method.Name == "GetHashCode" ||
                    (method.Name == "Equals" && method.GetParameters().Any(x => x.ParameterType == typeof(object))) ||
                    method.Name == "ToString")
                {
                    continue;
                }

                var parameters = string.Join(", ", method.GetParameters().Select(Argument));
                builder.AppendLine("        [Benchmark]")
                    .AppendLine($"        public {method.ReturnType.Name} {method.Name}()")
                    .AppendLine("        {")
                    .AppendLine($"             return {type.Name}1.{method.Name}({parameters});")
                    .AppendLine("        }")
                    .AppendLine();
            }

            var code = builder.ToString();
            Console.Write(code);
        }

        private static string Operator(MethodInfo methodInfo)
        {
            switch (methodInfo.Name)
            {
                case "op_Addition":
                    return "+";
                case "op_Subtraction":
                    return "-";
                case "op_Multiply":
                    return "*";
                case "op_Division":
                    return "/";
                case "op_Equality":
                    return "==";
                default:
                    return methodInfo.Name;
            }
        }

        private static string Argument(ParameterInfo parameter)
        {
            if (parameter.ParameterType == parameter.Member.DeclaringType)
            {
                return parameter.ParameterType.Name + 2;
            }

            if (parameter.ParameterType == typeof(double))
            {
                return "2";
            }

            if (parameter.ParameterType == typeof(Angle))
            {
                return "Angle.FromRadians(1)";
            }

            if (parameter.ParameterType == typeof(IFormatProvider))
            {
                return "CultureInfo.InvariantCulture";
            }

            return parameter.ParameterType.Name;
        }
    }
}
