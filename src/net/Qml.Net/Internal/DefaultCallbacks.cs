using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.CSharp.RuntimeBinder;
using Qml.Net.Internal.Qml;
using Qml.Net.Internal.Types;

namespace Qml.Net.Internal
{
    internal class DefaultCallbacks : ICallbacks
    {
        private Dictionary<int, CodeGen.CodeGen.InvokeMethodDelegate> _cachedReadProperties = new Dictionary<int, CodeGen.CodeGen.InvokeMethodDelegate>();
        private Dictionary<int, CodeGen.CodeGen.InvokeMethodDelegate> _cachedSetProperties = new Dictionary<int, CodeGen.CodeGen.InvokeMethodDelegate>();
        private Dictionary<int, CodeGen.CodeGen.InvokeMethodDelegate> _cachedInvokeMethods = new Dictionary<int, CodeGen.CodeGen.InvokeMethodDelegate>();

        public bool IsTypeValid(string typeName)
        {
            var t = Type.GetType(typeName);
            return t != null;
        }

        public void CreateLazyTypeInfo(IntPtr t)
        {
            using (var type = new NetTypeInfo(t))
            {
                var typeInfo = Type.GetType(type.FullTypeName);
                if (typeInfo == null)
                {
                    throw new InvalidOperationException();
                }

                var baseType = typeInfo.BaseType;
                if (baseType != null)
                {
                    type.BaseType = baseType.AssemblyQualifiedName;
                }

                type.ClassName = typeInfo.Name;

                type.PrefVariantType = GetPrefVariantType(typeInfo);

                // All the methods/properties/signals are later populated when needed.
            }
        }

        public void LoadTypeInfo(IntPtr t)
        {
            using (var type = new NetTypeInfo(t))
            {
                var typeInfo = Type.GetType(type.FullTypeName);
                if (typeInfo == null)
                {
                    throw new InvalidOperationException($"Invalid type {type.FullTypeName}");
                }

                // Don't grab properties and methods for system-level types.
                if (Helpers.IsPrimitive(typeInfo))
                {
                    return;
                }

                if (typeInfo.IsArray)
                {
                    type.IsArray = true;
                }
                else
                {
                    if (typeof(IList).IsAssignableFrom(typeInfo))
                    {
                        type.IsList = true;
                    }
                    else if (typeInfo.IsGenericType)
                    {
                        if (typeof(IList<>).IsAssignableFrom(typeInfo.GetGenericTypeDefinition()))
                        {
                            type.IsList = true;
                        }
                    }
                }

                if (typeof(IQmlComponentCompleted).IsAssignableFrom(typeInfo))
                {
                    type.HasComponentCompleted = true;
                }

                if (typeof(IQmlObjectDestroyed).IsAssignableFrom(typeInfo))
                {
                    type.HasObjectDestroyed = true;
                }

                foreach (var methodInfo in typeInfo.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
                {
                    if (methodInfo.IsGenericMethod) continue; // No generics supported.
                    if (Helpers.IsPrimitive(methodInfo.DeclaringType)) continue;

                    if (methodInfo.IsSpecialName) continue; // Ignore the property get/set methods.

                    NetTypeInfo returnType = null;

                    if (methodInfo.ReturnParameter != null && methodInfo.ReturnParameter.ParameterType != typeof(void))
                    {
                        returnType =
                            NetTypeManager.GetTypeInfo(methodInfo.ReturnParameter.ParameterType);
                    }

                    var method = new NetMethodInfo(type, methodInfo.Name, returnType, methodInfo.IsStatic);

                    foreach (var parameter in methodInfo.GetParameters())
                    {
                        method.AddParameter(parameter.Name, NetTypeManager.GetTypeInfo(parameter.ParameterType));
                    }

                    type.AddMethod(method);
                }

                var signals = new Dictionary<string, NetSignalInfo>();

                foreach (var signalAttribute in typeInfo.GetCustomAttributes(false).OfType<SignalAttribute>())
                {
                    if (string.IsNullOrEmpty(signalAttribute.Name))
                    {
                        throw new InvalidOperationException($"Signal name was null for {typeInfo.Name}");
                    }

                    if (!char.IsLower(signalAttribute.Name[0]))
                    {
                        throw new InvalidOperationException($"Signal {signalAttribute.Name} for {typeInfo.Name} must begin with a lower case letter.");
                    }

                    var signal = new NetSignalInfo(type, signalAttribute.Name);
                    foreach (var parameter in signalAttribute.Parameters)
                    {
                        signal.AddParameter(parameter);
                    }
                    type.AddSignal(signal);
                    signals.Add(signal.Name, signal);
                }

                foreach (var propertyInfo in typeInfo.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (Helpers.IsPrimitive(propertyInfo.DeclaringType)) continue;

                    NetSignalInfo notifySignal = null;
                    var notifySignalAttribute = propertyInfo.GetCustomAttribute<NotifySignalAttribute>();
                    if (notifySignalAttribute != null)
                    {
                        var name = notifySignalAttribute.Name;
                        if (string.IsNullOrEmpty(name))
                        {
                            name = $"{propertyInfo.Name}Changed";
                            name = char.ToLower(name[0]) + name.Substring(1);
                        }

                        if (signals.ContainsKey(name))
                        {
                            notifySignal = signals[name];

                            // Make sure the signal we are referencing has no parameters.
                            if (notifySignal.ParameterCount != 0)
                            {
                                // TODO: They can actually of parameters, but not implemented yet.
                                throw new Exception("Notify signals must have no parameters.");
                            }
                        }
                        else
                        {
                            if (!char.IsLower(name[0]))
                            {
                                throw new InvalidOperationException($"Signal {name} for {typeInfo.Name} must begin with a lower case letter.");
                            }

                            notifySignal = new NetSignalInfo(type, name);
                            type.AddSignal(notifySignal);
                        }
                    }

                    using (var property = new NetPropertyInfo(
                        type,
                        propertyInfo.Name,
                        NetTypeManager.GetTypeInfo(propertyInfo.PropertyType),
                        propertyInfo.CanRead,
                        propertyInfo.CanWrite,
                        notifySignal))
                    {
                        foreach (var indexParameter in propertyInfo.GetIndexParameters())
                        {
                            property.AddIndexParameter(indexParameter.Name, NetTypeManager.GetTypeInfo(indexParameter.ParameterType));
                        }

                        type.AddProperty(property);
                    }
                }

                // NOTE: This type is going to get a typeInfo object
                // with IsLoading=true and IsLoaded=false. It technically
                // is loaded, but it's misleading.
                InteropBehaviors.OnNetTypeInfoCreated(type, typeInfo);
            }
        }

        public void ReleaseNetReference(UInt64 objectId)
        {
            NetReference.OnRelease(objectId);
        }

        public void ReleaseNetDelegateGCHandle(IntPtr handle)
        {
            NetDelegate.ReleaseGCHandle((GCHandle)handle);
        }

        public IntPtr InstantiateType(IntPtr type)
        {
            try
            {
                var typeName = Utilities.ContainerToString(Interop.NetTypeInfo.GetFullTypeName(type));
                var typeInfo = Type.GetType(typeName);
                if (typeInfo == null) throw new InvalidOperationException($"Invalid type {typeName}");
                var netReference = NetReference.CreateForObject(TypeCreator.Create(typeInfo));

                // When .NET collects this NetReference, we don't want it to delete this
                // handle. Ownership has been passed to the caller.
                return Interop.NetReference.Clone(netReference.Handle);
            }
            finally
            {
                Interop.NetTypeInfo.Destroy(type);
            }
        }

        public void CallComponentCompleted(IntPtr t)
        {
            using (var target = new NetReference(t))
            {
                var instance = target.Instance;
                var componentCompelted = instance as IQmlComponentCompleted;
                if (componentCompelted == null)
                {
                    throw new Exception($"Type {instance.GetType().FullName} doesn't implement IQmlComponentCompleted");
                }

                var result = componentCompelted.ComponentCompleted();
                if (QmlNetConfig.ListenForExceptionsWhenInvokingTasks)
                {
                    result?.ContinueWith(
                        task =>
                        {
                            QmlNetConfig.RaiseUnhandledTaskException(task.Exception);
                        },
                        TaskContinuationOptions.OnlyOnFaulted);
                }
            }
        }

        public void CallObjectDestroyed(IntPtr t)
        {
            using (var target = new NetReference(t))
            {
                var instance = target.Instance;
                var objectDestroyed = instance as IQmlObjectDestroyed;
                if (objectDestroyed == null)
                {
                    throw new Exception($"Type {instance.GetType().FullName} doesn't implement IQmlObjectDestroyed");
                }

                objectDestroyed.ObjectDestroyed();
            }
        }

        public void ReadProperty(IntPtr p, IntPtr t, IntPtr ip, IntPtr r)
        {
            using (var property = new NetPropertyInfo(p))
            using (var target = new NetReference(t))
            using (var indexParameter = ip != IntPtr.Zero ? new NetVariant(ip) : null)
            using (var result = new NetVariant(r))
            {
                CodeGen.CodeGen.InvokeMethodDelegate del;
                if (!_cachedReadProperties.TryGetValue(property.Id, out del))
                {
                    del = CodeGen.CodeGen.BuildReadPropertyDelegate(property);
                    _cachedReadProperties[property.Id] = del;
                }

                using (var list = indexParameter != null ? new NetVariantList() : null)
                {
                    if (indexParameter != null)
                    {
                        list.Add(indexParameter);
                    }

                    Task resultTask = null;
                    del(target, list, result, ref resultTask);
                }
            }
        }

        public void WriteProperty(IntPtr p, IntPtr t, IntPtr ip, IntPtr v)
        {
            using (var property = new NetPropertyInfo(p))
            using (var target = new NetReference(t))
            using (var indexParameter = ip != IntPtr.Zero ? new NetVariant(ip) : null)
            using (var value = new NetVariant(v))
            {
                CodeGen.CodeGen.InvokeMethodDelegate del;
                if (!_cachedSetProperties.TryGetValue(property.Id, out del))
                {
                    del = CodeGen.CodeGen.BuildSetPropertyDelegate(property);
                    _cachedSetProperties[property.Id] = del;
                }

                using (var list = new NetVariantList())
                {
                    if (indexParameter != null)
                    {
                        list.Add(indexParameter);
                    }

                    list.Add(value);
                    Task resultTask = null;
                    del(target, list, null, ref resultTask);
                }
            }
        }

        public void InvokeMethod(IntPtr m, IntPtr t, IntPtr p, IntPtr r)
        {
            using (var method = new NetMethodInfo(m))
            using (var target = new NetReference(t))
            using (var parameters = new NetVariantList(p))
            using (var result = r != IntPtr.Zero ? new NetVariant(r) : null)
            {
                CodeGen.CodeGen.InvokeMethodDelegate del;
                if (!_cachedInvokeMethods.TryGetValue(method.Id, out del))
                {
                    del = CodeGen.CodeGen.BuildInvokeMethodDelegate(method);
                    _cachedInvokeMethods[method.Id] = del;
                }

                Task resultTask = null;
                del(target, parameters, result, ref resultTask);

                if (QmlNetConfig.ListenForExceptionsWhenInvokingTasks)
                {
                    resultTask?.ContinueWith(
                        task =>
                        {
                            QmlNetConfig.RaiseUnhandledTaskException(task.Exception);
                        },
                        TaskContinuationOptions.OnlyOnFaulted);
                }
            }
        }

        public void GCCollect(int maxGeneration)
        {
            GC.Collect(maxGeneration);
        }

        public bool RaiseNetSignals(IntPtr t, string signalName, IntPtr p)
        {
            using (var target = new NetReference(t))
            using (var parameters = p != IntPtr.Zero ? new NetVariantList(p) : null)
            {
                var signals = target.Instance.GetAttachedDelegates(signalName);
                if (signals != null && signals.Count > 0)
                {
                    List<object> methodParameters = null;

                    if (parameters != null && parameters.Count > 0)
                    {
                        methodParameters = new List<object>();
                        var parameterCount = parameters.Count;
                        for (var x = 0; x < parameterCount; x++)
                        {
                            object v = null;
                            Helpers.Unpackvalue(ref v, parameters.Get(x));
                            methodParameters.Add(v);
                        }
                    }

                    foreach (var signal in signals)
                    {
                        signal.DynamicInvoke(methodParameters?.ToArray());
                    }

                    return true; /*some signals were raised*/
                }

                return false; /*no signals were raised*/
            }
        }

        public async Task AwaitTask(IntPtr t, IntPtr sc, IntPtr fc)
        {
            using (var target = new NetReference(t))
            using (var successCallback = new NetJsValue(sc))
            using (var failureCallback = fc != IntPtr.Zero ? new NetJsValue(fc) : null)
            {
                var taskObject = target.Instance;
                if (taskObject is Task task)
                {
                    try
                    {
                        await task;
                    }
                    catch (Exception ex)
                    {
                        // The task throw an exception.
                        // Invoke our failure callback.
                        if (failureCallback == null)
                        {
                            // The caller didn't want to listen to failures.
                            // Just throw it and let "TaskScheduler.UnobservedTaskException"
                            // handle it.
                            throw;
                        }

                        failureCallback.Call(ex);
                        return;
                    }

                    try
                    {
                        var result = (object)((dynamic)task).Result;
                        successCallback.Call(result);
                    }
                    catch (RuntimeBinderException)
                    {
                        // No "Result" property, a task with no callbacks.
                        // TODO: Find a better way to handle this than catching an exception.
                        successCallback.Call();
                    }
                }
                else
                {
                    throw new InvalidOperationException("Attempted to await on a .NET object that wasn't a Task.");
                }
            }
        }

        public bool Serialize(IntPtr i, IntPtr r)
        {
            using (var instance = new NetReference(i))
            using (var result = new NetVariant(r))
            {
                try
                {
                    result.String = Serializer.Current.Serialize(instance.Instance);
                    return true;
                }
                catch (Exception ex)
                {
                    // TODO: Propagate this error to the user.
                    Console.Error.WriteLine($"Error serializing .NET object: {ex.Message}");
                    return false;
                }
            }
        }

        private NetVariantType GetPrefVariantType(Type typeInfo)
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
                // ReSharper disable TailRecursiveCall
                return GetPrefVariantType(typeInfo.GetGenericArguments()[0]);
                // ReSharper restore TailRecursiveCall
            }

            return NetVariantType.Object;
        }
    }
}