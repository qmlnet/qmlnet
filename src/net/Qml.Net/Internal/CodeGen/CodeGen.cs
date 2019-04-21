using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp;
using Qml.Net.Internal.Qml;
using Qml.Net.Internal.Types;

namespace Qml.Net.Internal.CodeGen
{
    internal partial class CodeGen
    {
        public delegate void InvokeMethodDelegate(NetReference reference, NetVariantList parameters, NetVariant result, ref Task taskResult);

        public static InvokeMethodDelegate BuildInvokeMethodDelegate(NetMethodInfo methodInfo)
        {
            var invokeType = Type.GetType(methodInfo.ParentType.FullTypeName);
            var parameterTypes = new List<Type>();
            for (var x = 0; x < methodInfo.ParameterCount; x++)
            {
                using (var p = methodInfo.GetParameter(x))
                using (var t = p.Type)
                {
                    parameterTypes.Add(Type.GetType(t.FullTypeName));
                }
            }
            var invokeMethod = invokeType.GetMethod(methodInfo.MethodName, parameterTypes.ToArray());
            return BuildInvokeMethodDelegate(invokeMethod);
        }

        public static InvokeMethodDelegate BuildInvokeMethodDelegate(MethodInfo methodInfo)
        {
            var invokeType = methodInfo.DeclaringType;

            var dynamicMethod = new DynamicMethod(
                "method",
                typeof(void),
                new[] { typeof(NetReference), typeof(NetVariantList), typeof(NetVariant), typeof(Task).MakeByRefType() });

            bool box = false;
            if (methodInfo.ReturnType != null && methodInfo.ReturnType != typeof(void))
            {
                MethodInfo returnMethod = null;
                var isNullable = false;
                switch (GetPrefVariantType(methodInfo.ReturnType, ref isNullable))
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
                        box = methodInfo.ReturnType.IsValueType;
                        break;
                    case NetVariantType.JsValue:
                        throw new NotImplementedException();
                    case NetVariantType.QObject:
                        throw new NotImplementedException();
                    case NetVariantType.Invalid:
                        throw new Exception("invalid type");
                    default:
                        throw new Exception("unknown type");
                }

                var il = dynamicMethod.GetILGenerator();

                var tempValue = il.DeclareLocal(methodInfo.ReturnType);

                // var tempValue = (({TYPE})netReference.Instance)).{METHOD}({PARAMETERS});
                il.Emit(OpCodes.Ldarg_0); // net reference
                il.Emit(OpCodes.Callvirt, GenericMethods.InstanceProperty.GetMethod);
                il.Emit(OpCodes.Castclass, invokeType);
                InvokeParameters(il, methodInfo);
                il.Emit(OpCodes.Callvirt, methodInfo);
                il.Emit(OpCodes.Stloc, tempValue.LocalIndex);

                // {LOADMETHOD}(result, tempvalue)
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Ldloc, tempValue.LocalIndex);
                if (box)
                {
                    il.Emit(OpCodes.Box, methodInfo.ReturnType);
                }
                il.Emit(OpCodes.Call, returnMethod);

                if (typeof(Task).IsAssignableFrom(methodInfo.ReturnType))
                {
                    // task = tempValue;
                    il.Emit(OpCodes.Ldarg_3);
                    il.Emit(OpCodes.Ldloc, tempValue.LocalIndex);
                    il.Emit(OpCodes.Stind_Ref);
                }

                il.Emit(OpCodes.Ret);
            }
            else
            {
                var il = dynamicMethod.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Callvirt, GenericMethods.InstanceProperty.GetMethod);
                il.Emit(OpCodes.Castclass, invokeType);

                InvokeParameters(il, methodInfo);

                il.Emit(OpCodes.Callvirt, methodInfo);

                il.Emit(OpCodes.Ret);
            }

            return (InvokeMethodDelegate)dynamicMethod.CreateDelegate(typeof(InvokeMethodDelegate));
        }

        public static InvokeMethodDelegate BuildReadPropertyDelegate(NetPropertyInfo propertyInfo)
        {
            var invokeType = Type.GetType(propertyInfo.ParentType.FullTypeName);

            var parameterTypes = new List<Type>();
            for (var x = 0; x < propertyInfo.IndexParameterCount; x++)
            {
                using (var p = propertyInfo.GetIndexParameter(x))
                using (var t = p.Type)
                {
                    parameterTypes.Add(Type.GetType(t.FullTypeName));
                }
            }

            var invokeProperty = invokeType.GetProperty(propertyInfo.Name);

            return BuildInvokeMethodDelegate(invokeProperty.GetMethod);
        }

        public static InvokeMethodDelegate BuildSetPropertyDelegate(NetPropertyInfo propertyInfo)
        {
            var invokeType = Type.GetType(propertyInfo.ParentType.FullTypeName);

            var parameterTypes = new List<Type>();
            for (var x = 0; x < propertyInfo.IndexParameterCount; x++)
            {
                using (var p = propertyInfo.GetIndexParameter(x))
                using (var t = p.Type)
                {
                    parameterTypes.Add(Type.GetType(t.FullTypeName));
                }
            }

            var invokeProperty = invokeType.GetProperty(propertyInfo.Name);

            return BuildInvokeMethodDelegate(invokeProperty.SetMethod);
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
                var unbox = false;
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
                        unbox = parameter.ParameterType.IsValueType;
                        break;
                    case NetVariantType.JsValue:
                        throw new NotImplementedException();
                    case NetVariantType.QObject:
                        throw new NotImplementedException();
                    case NetVariantType.Invalid:
                        throw new Exception("invalid type");
                    default:
                        throw new Exception("unknown type");
                }

                il.Emit(OpCodes.Call, returnMethod);

                if (unbox)
                {
                    il.Emit(OpCodes.Unbox_Any, parameter.ParameterType);
                }

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

            if (typeInfo.IsEnum)
            {
                return GetPrefVariantType(Enum.GetUnderlyingType(typeInfo), ref isNullable);
            }

            return NetVariantType.Object;
        }
    }
}