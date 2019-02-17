using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Microsoft.CSharp;
using Qml.Net.Internal.Qml;
using Qml.Net.Internal.Types;

namespace Qml.Net.Internal.CodeGen
{
    internal partial class CodeGen
    {
        public delegate void InvokeMethodDelegate(NetReference reference, NetVariantList parameters, NetVariant result);

        public delegate void ReadPropertyDelegate(NetReference reference, NetVariant result);

        public delegate void SetPropertyDelegate(NetReference reference, NetVariant value);

        public static InvokeMethodDelegate BuildInvokeMethodDelegate(NetMethodInfo methodInfo)
        {
            var invokeType = Type.GetType(methodInfo.ParentType.FullTypeName);
            var invokeMethod = invokeType.GetMethod(methodInfo.MethodName);

            var dynamicMethod = new DynamicMethod(
                "method",
                typeof(void),
                new[] { typeof(NetReference), typeof(NetVariantList), typeof(NetVariant) });

            if (invokeMethod.ReturnType != null && invokeMethod.ReturnType != typeof(void))
            {
                MethodInfo returnMethod = null;
                var isNullable = false;
                switch (GetPrefVariantType(invokeMethod.ReturnType, ref isNullable))
                {
                    case NetVariantType.Bool:
                        returnMethod = isNullable ? LoadMethods.LoadBoolNullableMethod : LoadMethods.LoadBoolMethod;
                        break;
                    case NetVariantType.Char:
                        returnMethod = isNullable ? LoadMethods.LoadCharNullableMethod : LoadMethods.LoadCharMethod;
                        break;
                    case NetVariantType.Int:
                        returnMethod = isNullable ? LoadMethods.LoadIntNullableMethod : LoadMethods.LoadIntMethod;
                        break;
                    case NetVariantType.UInt:
                        returnMethod = isNullable ? LoadMethods.LoadUIntNullableMethod : LoadMethods.LoadUIntMethod;
                        break;
                    case NetVariantType.Long:
                        returnMethod = isNullable ? LoadMethods.LoadLongNullableMethod : LoadMethods.LoadLongMethod;
                        break;
                    case NetVariantType.ULong:
                        returnMethod = isNullable ? LoadMethods.LoadULongNullableMethod : LoadMethods.LoadULongMethod;
                        break;
                    case NetVariantType.Float:
                        returnMethod = isNullable ? LoadMethods.LoadFloatNullableMethod : LoadMethods.LoadFloatMethod;
                        break;
                    case NetVariantType.Double:
                        returnMethod = isNullable ? LoadMethods.LoadDoubleNullableMethod : LoadMethods.LoadDoubleMethod;
                        break;
                    case NetVariantType.String:
                        returnMethod = LoadMethods.LoadStringMethod;
                        break;
                    case NetVariantType.DateTime:
                        returnMethod = isNullable ? LoadMethods.LoadDateTimeNullableMethod : LoadMethods.LoadDateTimeMethod;
                        break;
                    case NetVariantType.Object:
                        returnMethod = LoadMethods.LoadObjectMethod;
                        break;
                    case NetVariantType.JsValue:
                        throw new NotImplementedException();
                    case NetVariantType.Invalid:
                        throw new Exception("invalid type");
                    default:
                        throw new Exception("unknown type");
                }

                var il = dynamicMethod.GetILGenerator();

                il.Emit(OpCodes.Ldarg_2); // result
                il.Emit(OpCodes.Ldarg_0); // net reference
                il.Emit(OpCodes.Callvirt, GenericMethods.InstanceProperty.GetMethod);
                il.Emit(OpCodes.Castclass, invokeType);

                InvokeParameters(il, invokeMethod);

                il.Emit(OpCodes.Callvirt, invokeMethod);
                il.Emit(OpCodes.Call, returnMethod);

                il.Emit(OpCodes.Ret);
            }
            else
            {
                var il = dynamicMethod.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Callvirt, GenericMethods.InstanceProperty.GetMethod);
                il.Emit(OpCodes.Castclass, invokeType);

                InvokeParameters(il, invokeMethod);

                il.Emit(OpCodes.Callvirt, invokeMethod);

                il.Emit(OpCodes.Ret);
            }

            return (InvokeMethodDelegate)dynamicMethod.CreateDelegate(typeof(InvokeMethodDelegate));
        }

        public static ReadPropertyDelegate BuildReadPropertyDelegate(NetPropertyInfo propertyInfo)
        {
            var invokeType = Type.GetType(propertyInfo.ParentType.FullTypeName);
            var invokeProperty = invokeType.GetProperty(propertyInfo.Name);

            MethodInfo loadMethod = null;
            var isNullable = false;
            switch (GetPrefVariantType(invokeProperty.PropertyType, ref isNullable))
            {
                case NetVariantType.Bool:
                    loadMethod = isNullable ? LoadMethods.LoadBoolNullableMethod : LoadMethods.LoadBoolMethod;
                    break;
                case NetVariantType.Char:
                    loadMethod = isNullable ? LoadMethods.LoadCharNullableMethod : LoadMethods.LoadCharMethod;
                    break;
                case NetVariantType.Int:
                    loadMethod = isNullable ? LoadMethods.LoadIntNullableMethod : LoadMethods.LoadIntMethod;
                    break;
                case NetVariantType.UInt:
                    loadMethod = isNullable ? LoadMethods.LoadUIntNullableMethod : LoadMethods.LoadUIntMethod;
                    break;
                case NetVariantType.Long:
                    loadMethod = isNullable ? LoadMethods.LoadLongNullableMethod : LoadMethods.LoadLongMethod;
                    break;
                case NetVariantType.ULong:
                    loadMethod = isNullable ? LoadMethods.LoadULongNullableMethod : LoadMethods.LoadULongMethod;
                    break;
                case NetVariantType.Float:
                    loadMethod = isNullable ? LoadMethods.LoadFloatNullableMethod : LoadMethods.LoadFloatMethod;
                    break;
                case NetVariantType.Double:
                    loadMethod = isNullable ? LoadMethods.LoadDoubleNullableMethod : LoadMethods.LoadDoubleMethod;
                    break;
                case NetVariantType.String:
                    loadMethod = LoadMethods.LoadStringMethod;
                    break;
                case NetVariantType.DateTime:
                    loadMethod = isNullable ? LoadMethods.LoadDateTimeNullableMethod : LoadMethods.LoadDateTimeMethod;
                    break;
                case NetVariantType.Object:
                    loadMethod = LoadMethods.LoadObjectMethod;
                    break;
                case NetVariantType.JsValue:
                    throw new NotImplementedException();
                case NetVariantType.Invalid:
                    throw new Exception("invalid type");
                default:
                    throw new Exception("unknown type");
            }

            var dynamicMethod = new DynamicMethod(
                "method",
                typeof(void),
                new[] { typeof(NetReference), typeof(NetVariant) });

            var il = dynamicMethod.GetILGenerator();

            il.Emit(OpCodes.Ldarg_1); // result
            il.Emit(OpCodes.Ldarg_0); // net reference
            il.Emit(OpCodes.Callvirt, GenericMethods.InstanceProperty.GetMethod);
            il.Emit(OpCodes.Castclass, invokeType);
            il.Emit(OpCodes.Callvirt, invokeProperty.GetMethod);
            il.Emit(OpCodes.Call, loadMethod);
            il.Emit(OpCodes.Ret);

            return (ReadPropertyDelegate)dynamicMethod.CreateDelegate(typeof(ReadPropertyDelegate));
        }

        public static SetPropertyDelegate BuildSetPropertyDelegate(NetPropertyInfo propertyInfo)
        {
            var invokeType = Type.GetType(propertyInfo.ParentType.FullTypeName);
            var invokeProperty = invokeType.GetProperty(propertyInfo.Name);

            MethodInfo getMethod = null;
            var isNullable = false;
            switch (GetPrefVariantType(invokeProperty.PropertyType, ref isNullable))
            {
                case NetVariantType.Bool:
                    getMethod = isNullable ? GetMethods.BoolNullableMethod : GetMethods.BoolMethod;
                    break;
                case NetVariantType.Char:
                    getMethod = isNullable ? GetMethods.CharNullableMethod : GetMethods.CharMethod;
                    break;
                case NetVariantType.Int:
                    getMethod = isNullable ? GetMethods.IntNullableMethod : GetMethods.IntMethod;
                    break;
                case NetVariantType.UInt:
                    getMethod = isNullable ? GetMethods.UIntNullableMethod : GetMethods.UIntMethod;
                    break;
                case NetVariantType.Long:
                    getMethod = isNullable ? GetMethods.LongNullableMethod : GetMethods.LongMethod;
                    break;
                case NetVariantType.ULong:
                    getMethod = isNullable ? GetMethods.ULongNullableMethod : GetMethods.LongMethod;
                    break;
                case NetVariantType.Float:
                    getMethod = isNullable ? GetMethods.FloatNullableMethod : GetMethods.FloatMethod;
                    break;
                case NetVariantType.Double:
                    getMethod = isNullable ? GetMethods.DoubleNullableMethod : GetMethods.DoubleMethod;
                    break;
                case NetVariantType.String:
                    getMethod = GetMethods.StringMethod;
                    break;
                case NetVariantType.DateTime:
                    getMethod = isNullable ? GetMethods.DateTimeNullableMethod : GetMethods.DateTimeMethod;
                    break;
                case NetVariantType.Object:
                    getMethod = GetMethods.ObjMethod;
                    break;
                case NetVariantType.JsValue:
                    throw new NotImplementedException();
                case NetVariantType.Invalid:
                    throw new Exception("invalid type");
                default:
                    throw new Exception("unknown type");
            }

            var dynamicMethod = new DynamicMethod(
                "method",
                typeof(void),
                new[] { typeof(NetReference), typeof(NetVariant) });

            var il = dynamicMethod.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0); // net reference
            il.Emit(OpCodes.Callvirt, GenericMethods.InstanceProperty.GetMethod);
            il.Emit(OpCodes.Castclass, invokeType);
            il.Emit(OpCodes.Ldarg_1); // variant
            il.Emit(OpCodes.Call, getMethod);
            il.Emit(OpCodes.Callvirt, invokeProperty.SetMethod);

            il.Emit(OpCodes.Ret);

            return (SetPropertyDelegate)dynamicMethod.CreateDelegate(typeof(SetPropertyDelegate));
        }

        private static void InvokeParameters(ILGenerator il, MethodInfo methodInfo)
        {
            int parameterIndex = 0;
            foreach (var parameter in methodInfo.GetParameters())
            {
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldc_I4_S, parameterIndex);

                MethodInfo returnMethod = null;
                var isNullable = false;
                switch (GetPrefVariantType(parameter.ParameterType, ref isNullable))
                {
                    case NetVariantType.Bool:
                        returnMethod = isNullable ? ListMethods.BoolNullableAtMethod : ListMethods.BoolAtMethod;
                        break;
                    case NetVariantType.Char:
                        returnMethod = isNullable ? ListMethods.CharNullableAtMethod : ListMethods.CharAtMethod;
                        break;
                    case NetVariantType.Int:
                        returnMethod = isNullable ? ListMethods.IntNullableAtMethod : ListMethods.IntAtMethod;
                        break;
                    case NetVariantType.UInt:
                        returnMethod = isNullable ? ListMethods.UIntNullableAtMethod : ListMethods.UIntAtMethod;
                        break;
                    case NetVariantType.Long:
                        returnMethod = isNullable ? ListMethods.LongNullableAtMethod : ListMethods.LongAtMethod;
                        break;
                    case NetVariantType.ULong:
                        returnMethod = isNullable ? ListMethods.ULongNullableAtMethod : ListMethods.ULongAtMethod;
                        break;
                    case NetVariantType.Float:
                        returnMethod = isNullable ? ListMethods.FloatNullableAtMethod : ListMethods.FloatAtMethod;
                        break;
                    case NetVariantType.Double:
                        returnMethod = isNullable ? ListMethods.DoubleNullableAtMethod : ListMethods.DoubleAtMethod;
                        break;
                    case NetVariantType.String:
                        returnMethod = ListMethods.StringAtMethod;
                        break;
                    case NetVariantType.DateTime:
                        returnMethod = isNullable ? ListMethods.DateTimeNullableAtMethod : ListMethods.DateTimeAtMethod;
                        break;
                    case NetVariantType.Object:
                        returnMethod = ListMethods.ObjectAtMethod;
                        break;
                    case NetVariantType.JsValue:
                        throw new NotImplementedException();
                    case NetVariantType.Invalid:
                        throw new Exception("invalid type");
                    default:
                        throw new Exception("unknown type");
                }

                il.Emit(OpCodes.Call, returnMethod);
                parameterIndex++;
            }
        }

        private static NetVariantType GetPrefVariantType(Type typeInfo, ref bool isNullable)
        {
            if (typeInfo == typeof(bool))
                return NetVariantType.Bool;
            if (typeInfo == typeof(char))
                return NetVariantType.Char;
            if (typeInfo == typeof(int))
                return NetVariantType.Int;
            if (typeInfo == typeof(uint))
                return NetVariantType.UInt;
            if (typeInfo == typeof(long))
                return NetVariantType.Long;
            if (typeInfo == typeof(ulong))
                return NetVariantType.ULong;
            if (typeInfo == typeof(float))
                return NetVariantType.Float;
            if (typeInfo == typeof(double))
                return NetVariantType.Double;
            if (typeInfo == typeof(string))
                return NetVariantType.String;
            if (typeInfo == typeof(DateTimeOffset))
                return NetVariantType.DateTime;

            if (typeInfo.IsGenericType &&
                typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                isNullable = true;
                // ReSharper disable TailRecursiveCall
                return GetPrefVariantType(typeInfo.GetGenericArguments()[0], ref isNullable);
                // ReSharper restore TailRecursiveCall
            }

            return NetVariantType.Object;
        }
    }
}