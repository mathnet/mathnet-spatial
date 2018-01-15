namespace MathNet.Spatial.Serialization
{
#if NETSTANDARD1_3 == false
    using System;
    using System.Runtime.Serialization;
    using MathNet.Spatial.Serialization;

    /// <summary>
    /// This class provides serialization for spatial objects
    /// </summary>
    internal class SpatialBinarySerializer : ISerializationSurrogate
    {
        /// <inheritdoc />
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            var surrogate = SpatialSerialization.GetObjectToSerialize(obj);
            Type surrogateType = surrogate.GetType();
            foreach (var field in surrogateType.GetFields())
            {
                if (field.FieldType.IsArray)
                {
                    // unfortunately the legacy formatters do not handled nested objects in arrays
                    // so need to manually convert the type into a surrogate
                    var list = field.GetValue(surrogate);
                    int cnt = ((Array)list).Length;
                    Type surrogateArrayElementType = SpatialSerialization.GetSurrogateType(field.FieldType.GetElementType());
                    var surrogatedList = Array.CreateInstance(surrogateArrayElementType, cnt);
                    int i = 0;
                    foreach (object item in (Array)list)
                    {
                        var itemSurrogate = SpatialSerialization.GetObjectToSerialize(item);
                        surrogatedList.SetValue(itemSurrogate, i++);
                    }

                    info.AddValue(field.Name, surrogatedList);
                }
                else
                {
                    info.AddValue(field.Name, field.GetValue(surrogate));
                }
            }
        }

        /// <inheritdoc />
        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Type surrogateType = SpatialSerialization.GetSurrogateType(obj.GetType());
            var surrogate = Activator.CreateInstance(surrogateType);
            foreach (var field in surrogateType.GetFields())
            {
                if (field.FieldType.IsArray)
                {
                    // unfortunately the legacy formatters do not handled nested objects in arrays
                    // so need to manually convert back the type
                    Type surrogateArrayElementType = SpatialSerialization.GetSurrogateType(field.FieldType.GetElementType());
                    Type surrogateArrayType = surrogateArrayElementType.MakeArrayType();
                    var surrogateArray = info.GetValue(field.Name, surrogateArrayType);
                    int cnt = ((Array)surrogateArray).Length;
                    int i = 0;
                    var actualItemList = Array.CreateInstance(field.FieldType.GetElementType(), cnt);
                    foreach (object item in (Array)surrogateArray)
                    {
                        var itemSurrogate = SpatialSerialization.GetDeserializedObject(item);
                        actualItemList.SetValue(itemSurrogate, i++);
                    }

                    field.SetValue(surrogate, actualItemList);
                }
                else
                {
                    var value = info.GetValue(field.Name, field.FieldType);
                    field.SetValue(surrogate, value);
                }
            }

            var actual = SpatialSerialization.GetDeserializedObject(surrogate);
            return actual;
        }
    }
#endif
}
