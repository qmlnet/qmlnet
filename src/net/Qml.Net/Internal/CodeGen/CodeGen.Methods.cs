using System;
using System.Reflection;
using Qml.Net.Internal.Qml;
using Qml.Net.Internal.Types;

namespace Qml.Net.Internal.CodeGen
{
    internal partial class CodeGen
    {
        private class GenericMethods
        {
            public static PropertyInfo InstanceProperty = typeof(NetReference).GetProperty(nameof(NetReference.Instance));
        }

        private class LoadMethods
        {
            public static MethodInfo LoadBoolMethod = typeof(LoadMethods).GetMethod(nameof(LoadBool), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo LoadBoolNullableMethod = typeof(LoadMethods).GetMethod(nameof(LoadBoolNullable), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo LoadCharMethod = typeof(LoadMethods).GetMethod(nameof(LoadChar), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo LoadCharNullableMethod = typeof(LoadMethods).GetMethod(nameof(LoadCharNullable), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo LoadIntMethod = typeof(LoadMethods).GetMethod(nameof(LoadInt), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo LoadIntNullableMethod = typeof(LoadMethods).GetMethod(nameof(LoadIntNullable), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo LoadUIntMethod = typeof(LoadMethods).GetMethod(nameof(LoadUInt), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo LoadUIntNullableMethod = typeof(LoadMethods).GetMethod(nameof(LoadUIntNullable), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo LoadLongMethod = typeof(LoadMethods).GetMethod(nameof(LoadLong), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo LoadLongNullableMethod = typeof(LoadMethods).GetMethod(nameof(LoadLongNullable), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo LoadULongMethod = typeof(LoadMethods).GetMethod(nameof(LoadULong), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo LoadULongNullableMethod = typeof(LoadMethods).GetMethod(nameof(LoadULongNullable), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo LoadFloatMethod = typeof(LoadMethods).GetMethod(nameof(LoadFloat), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo LoadFloatNullableMethod = typeof(LoadMethods).GetMethod(nameof(LoadFloatNullable), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo LoadDoubleMethod = typeof(LoadMethods).GetMethod(nameof(LoadDouble), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo LoadDoubleNullableMethod = typeof(LoadMethods).GetMethod(nameof(LoadDoubleNullable), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo LoadStringMethod = typeof(LoadMethods).GetMethod(nameof(LoadString), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo LoadDateTimeMethod = typeof(LoadMethods).GetMethod(nameof(LoadDateTime), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo LoadDateTimeNullableMethod = typeof(LoadMethods).GetMethod(nameof(LoadDateTimeNullable), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo LoadObjectMethod = typeof(LoadMethods).GetMethod(nameof(LoadObject), BindingFlags.Static | BindingFlags.NonPublic);

            private static void LoadBool(NetVariant variant, bool value)
            {
                variant.Bool = value;
            }

            private static void LoadBoolNullable(NetVariant variant, bool? value)
            {
                if (value.HasValue)
                {
                    variant.Bool = value.Value;
                }
                else
                {
                    variant.Clear();
                }
            }

            private static void LoadChar(NetVariant variant, char value)
            {
                variant.Char = value;
            }

            private static void LoadCharNullable(NetVariant variant, char? value)
            {
                if (value.HasValue)
                {
                    variant.Char = value.Value;
                }
                else
                {
                    variant.Clear();
                }
            }

            private static void LoadInt(NetVariant variant, int value)
            {
                variant.Int = value;
            }

            private static void LoadIntNullable(NetVariant variant, int? value)
            {
                if (value.HasValue)
                {
                    variant.Int = value.Value;
                }
                else
                {
                    variant.Clear();
                }
            }

            private static void LoadUInt(NetVariant variant, uint value)
            {
                variant.UInt = value;
            }

            private static void LoadUIntNullable(NetVariant variant, uint? value)
            {
                if (value.HasValue)
                {
                    variant.UInt = value.Value;
                }
                else
                {
                    variant.Clear();
                }
            }

            private static void LoadLong(NetVariant variant, long value)
            {
                variant.Long = value;
            }

            private static void LoadLongNullable(NetVariant variant, long? value)
            {
                if (value.HasValue)
                {
                    variant.Long = value.Value;
                }
                else
                {
                    variant.Clear();
                }
            }

            private static void LoadULong(NetVariant variant, ulong value)
            {
                variant.ULong = value;
            }

            private static void LoadULongNullable(NetVariant variant, ulong? value)
            {
                if (value.HasValue)
                {
                    variant.ULong = value.Value;
                }
                else
                {
                    variant.Clear();
                }
            }

            private static void LoadFloat(NetVariant variant, float value)
            {
                variant.Float = value;
            }

            private static void LoadFloatNullable(NetVariant variant, float? value)
            {
                if (value.HasValue)
                {
                    variant.Float = value.Value;
                }
                else
                {
                    variant.Clear();
                }
            }

            private static void LoadDouble(NetVariant variant, double value)
            {
                variant.Double = value;
            }

            private static void LoadDoubleNullable(NetVariant variant, double? value)
            {
                if (value.HasValue)
                {
                    variant.Double = value.Value;
                }
                else
                {
                    variant.Clear();
                }
            }

            private static void LoadString(NetVariant variant, string value)
            {
                if (value == null)
                {
                    variant.Clear();
                }
                else
                {
                    variant.String = value;
                }
            }

            private static void LoadDateTime(NetVariant variant, DateTimeOffset value)
            {
                variant.DateTime = value;
            }

            private static void LoadDateTimeNullable(NetVariant variant, DateTimeOffset? value)
            {
                if (value.HasValue)
                {
                    variant.DateTime = value.Value;
                }
                else
                {
                    variant.Clear();
                }
            }

            private static void LoadObject(NetVariant variant, object value)
            {
                if (value != null)
                {
                    variant.Instance = NetReference.CreateForObject(value);
                }
                else
                {
                    variant.Clear();
                }
            }
        }

        private class ListMethods
        {
            public static MethodInfo BoolAtMethod = typeof(ListMethods).GetMethod(nameof(BoolAt), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo BoolNullableAtMethod = typeof(ListMethods).GetMethod(nameof(BoolNullableAt), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo CharAtMethod = typeof(ListMethods).GetMethod(nameof(CharAt), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo CharNullableAtMethod = typeof(ListMethods).GetMethod(nameof(CharNullableAt), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo IntAtMethod = typeof(ListMethods).GetMethod(nameof(IntAt), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo IntNullableAtMethod = typeof(ListMethods).GetMethod(nameof(IntNullableAt), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo UIntAtMethod = typeof(ListMethods).GetMethod(nameof(UIntAt), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo UIntNullableAtMethod = typeof(ListMethods).GetMethod(nameof(UIntNullableAt), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo LongAtMethod = typeof(ListMethods).GetMethod(nameof(LongAt), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo LongNullableAtMethod = typeof(ListMethods).GetMethod(nameof(LongNullableAt), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo ULongAtMethod = typeof(ListMethods).GetMethod(nameof(ULongAt), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo ULongNullableAtMethod = typeof(ListMethods).GetMethod(nameof(ULongNullableAt), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo FloatAtMethod = typeof(ListMethods).GetMethod(nameof(FloatAt), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo FloatNullableAtMethod = typeof(ListMethods).GetMethod(nameof(FloatNullableAt), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo DoubleAtMethod = typeof(ListMethods).GetMethod(nameof(DoubleAt), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo DoubleNullableAtMethod = typeof(ListMethods).GetMethod(nameof(DoubleNullableAt), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo StringAtMethod = typeof(ListMethods).GetMethod(nameof(StringAt), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo DateTimeAtMethod = typeof(ListMethods).GetMethod(nameof(DateTimeAt), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo DateTimeNullableAtMethod = typeof(ListMethods).GetMethod(nameof(DateTimeNullableAt), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo ObjectAtMethod = typeof(ListMethods).GetMethod(nameof(ObjectAt), BindingFlags.Static | BindingFlags.NonPublic);

            private static bool BoolAt(NetVariantList list, int index)
            {
                using (var variant = list.Get(index))
                {
                    return variant != null && variant.Bool;
                }
            }

            private static bool? BoolNullableAt(NetVariantList list, int index)
            {
                using (var variant = list.Get(index))
                {
                    if (variant == null || variant.VariantType == NetVariantType.Invalid)
                    {
                        return null;
                    }

                    return variant.Bool;
                }
            }

            private static char CharAt(NetVariantList list, int index)
            {
                using (var variant = list.Get(index))
                {
                    return variant?.Char ?? default(char);
                }
            }

            private static char? CharNullableAt(NetVariantList list, int index)
            {
                using (var variant = list.Get(index))
                {
                    if (variant == null || variant.VariantType == NetVariantType.Invalid)
                    {
                        return null;
                    }

                    return variant.Char;
                }
            }

            private static int IntAt(NetVariantList list, int index)
            {
                using (var variant = list.Get(index))
                {
                    return variant?.Int ?? default(int);
                }
            }

            private static int? IntNullableAt(NetVariantList list, int index)
            {
                using (var variant = list.Get(index))
                {
                    if (variant == null || variant.VariantType == NetVariantType.Invalid)
                    {
                        return null;
                    }

                    return variant.Int;
                }
            }

            private static uint UIntAt(NetVariantList list, int index)
            {
                using (var variant = list.Get(index))
                {
                    return variant?.UInt ?? default(uint);
                }
            }

            private static uint? UIntNullableAt(NetVariantList list, int index)
            {
                using (var variant = list.Get(index))
                {
                    if (variant == null || variant.VariantType == NetVariantType.Invalid)
                    {
                        return null;
                    }

                    return variant.UInt;
                }
            }

            private static long LongAt(NetVariantList list, int index)
            {
                using (var variant = list.Get(index))
                {
                    return variant?.Long ?? default(long);
                }
            }

            private static long? LongNullableAt(NetVariantList list, int index)
            {
                using (var variant = list.Get(index))
                {
                    if (variant == null || variant.VariantType == NetVariantType.Invalid)
                    {
                        return null;
                    }

                    return variant.Long;
                }
            }

            private static ulong ULongAt(NetVariantList list, int index)
            {
                using (var variant = list.Get(index))
                {
                    return variant?.ULong ?? default(long);
                }
            }

            private static ulong? ULongNullableAt(NetVariantList list, int index)
            {
                using (var variant = list.Get(index))
                {
                    if (variant == null || variant.VariantType == NetVariantType.Invalid)
                    {
                        return null;
                    }

                    return variant.ULong;
                }
            }

            private static float FloatAt(NetVariantList list, int index)
            {
                using (var variant = list.Get(index))
                {
                    return variant?.Float ?? default(float);
                }
            }

            private static float? FloatNullableAt(NetVariantList list, int index)
            {
                using (var variant = list.Get(index))
                {
                    if (variant == null || variant.VariantType == NetVariantType.Invalid)
                    {
                        return null;
                    }

                    return variant.Float;
                }
            }

            private static double DoubleAt(NetVariantList list, int index)
            {
                using (var variant = list.Get(index))
                {
                    return variant?.Double ?? default(double);
                }
            }

            private static double? DoubleNullableAt(NetVariantList list, int index)
            {
                using (var variant = list.Get(index))
                {
                    if (variant == null || variant.VariantType == NetVariantType.Invalid)
                    {
                        return null;
                    }

                    return variant.Double;
                }
            }

            private static string StringAt(NetVariantList list, int index)
            {
                using (var variant = list.Get(index))
                {
                    if (variant == null || variant.VariantType == NetVariantType.Invalid)
                    {
                        return null;
                    }

                    return variant.String;
                }
            }

            private static DateTimeOffset DateTimeAt(NetVariantList list, int index)
            {
                using (var variant = list.Get(index))
                {
                    return variant?.DateTime ?? default(DateTimeOffset);
                }
            }

            private static DateTimeOffset? DateTimeNullableAt(NetVariantList list, int index)
            {
                using (var variant = list.Get(index))
                {
                    if (variant == null || variant.VariantType == NetVariantType.Invalid)
                    {
                        return null;
                    }

                    return variant.DateTime;
                }
            }

            private static object ObjectAt(NetVariantList list, int index)
            {
                using (var variant = list.Get(index))
                {
                    return variant?.AsObject();
                }
            }
        }

        private class GetMethods
        {
            public static MethodInfo BoolMethod = typeof(GetMethods).GetMethod(nameof(Bool), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo BoolNullableMethod = typeof(GetMethods).GetMethod(nameof(BoolNullable), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo CharMethod = typeof(GetMethods).GetMethod(nameof(Char), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo CharNullableMethod = typeof(GetMethods).GetMethod(nameof(CharNullable), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo IntMethod = typeof(GetMethods).GetMethod(nameof(Int), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo IntNullableMethod = typeof(GetMethods).GetMethod(nameof(IntNullable), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo UIntMethod = typeof(GetMethods).GetMethod(nameof(UInt), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo UIntNullableMethod = typeof(GetMethods).GetMethod(nameof(UIntNullable), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo LongMethod = typeof(GetMethods).GetMethod(nameof(Long), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo LongNullableMethod = typeof(GetMethods).GetMethod(nameof(LongNullable), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo ULongMethod = typeof(GetMethods).GetMethod(nameof(ULong), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo ULongNullableMethod = typeof(GetMethods).GetMethod(nameof(ULongNullable), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo FloatMethod = typeof(GetMethods).GetMethod(nameof(Float), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo FloatNullableMethod = typeof(GetMethods).GetMethod(nameof(FloatNullable), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo DoubleMethod = typeof(GetMethods).GetMethod(nameof(Double), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo DoubleNullableMethod = typeof(GetMethods).GetMethod(nameof(DoubleNullable), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo StringMethod = typeof(GetMethods).GetMethod(nameof(String), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo DateTimeMethod = typeof(GetMethods).GetMethod(nameof(DateTime), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo DateTimeNullableMethod = typeof(GetMethods).GetMethod(nameof(DateTimeNullable), BindingFlags.Static | BindingFlags.NonPublic);
            public static MethodInfo ObjMethod = typeof(GetMethods).GetMethod(nameof(Obj), BindingFlags.Static | BindingFlags.NonPublic);

            private static bool Bool(NetVariant variant)
            {
                return variant.Bool;
            }

            private static bool? BoolNullable(NetVariant variant)
            {
                if (variant.VariantType == NetVariantType.Invalid)
                {
                    return null;
                }

                return variant.Bool;
            }

            private static char Char(NetVariant variant)
            {
                return variant.Char;
            }

            private static char? CharNullable(NetVariant variant)
            {
                if (variant.VariantType == NetVariantType.Invalid)
                {
                    return null;
                }

                return variant.Char;
            }

            private static int Int(NetVariant variant)
            {
                return variant.Int;
            }

            private static int? IntNullable(NetVariant variant)
            {
                if (variant.VariantType == NetVariantType.Invalid)
                {
                    return null;
                }

                return variant.Int;
            }

            private static uint UInt(NetVariant variant)
            {
                return variant.UInt;
            }

            private static uint? UIntNullable(NetVariant variant)
            {
                if (variant.VariantType == NetVariantType.Invalid)
                {
                    return null;
                }

                return variant.UInt;
            }

            private static long Long(NetVariant variant)
            {
                return variant.Long;
            }

            private static long? LongNullable(NetVariant variant)
            {
                if (variant.VariantType == NetVariantType.Invalid)
                {
                    return null;
                }

                return variant.Long;
            }

            private static ulong ULong(NetVariant variant)
            {
                return variant.ULong;
            }

            private static ulong? ULongNullable(NetVariant variant)
            {
                if (variant.VariantType == NetVariantType.Invalid)
                {
                    return null;
                }

                return variant.ULong;
            }

            private static float Float(NetVariant variant)
            {
                return variant.ULong;
            }

            private static float? FloatNullable(NetVariant variant)
            {
                if (variant.VariantType == NetVariantType.Invalid)
                {
                    return null;
                }

                return variant.Float;
            }

            private static double Double(NetVariant variant)
            {
                return variant.Double;
            }

            private static double? DoubleNullable(NetVariant variant)
            {
                if (variant.VariantType == NetVariantType.Invalid)
                {
                    return null;
                }

                return variant.Double;
            }

            private static string String(NetVariant variant)
            {
                return variant.String;
            }

            private static DateTimeOffset DateTime(NetVariant variant)
            {
                var result = variant.DateTime;
                return result ?? default(DateTimeOffset);
            }

            private static DateTimeOffset? DateTimeNullable(NetVariant variant)
            {
                return variant.DateTime;
            }

            private static object Obj(NetVariant variant)
            {
                using (var reference = variant.Instance)
                {
                    return reference?.Instance;
                }
            }
        }
    }
}