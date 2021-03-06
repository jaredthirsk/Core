﻿using LionFire.Resolves;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace LionFire.Persistence
{
    public static class IReadWriteHandleExtensions
    {
        public static void SetValue(this IReadWriteHandle handle, object value/*, Type type = null*/)
        {
            //type ??= value?.GetType() ?? typeof(object);

            var setValueMethod = handle.GetType().GetProperty("Value").GetSetMethod();
            setValueMethod.Invoke(handle, new object[] { value });
        }

        public static async Task<object> TryGetOrCreate(this IReadWriteHandle handle)
        {
            var result = (await handle.Resolve().ConfigureAwait(false)).ToRetrieveResult();
            if (result.IsFound() == true) return result.Value;

            throw new NotImplementedException("TODO: Create");
        }

        // ENH: Return Put task separately and don't await it
        public static async Task<T> TryGetOrCreate<T>(this IReadWriteHandle<T> handle)  // REVIEW - return PersistenceResult<T>?  Generic doesn't exist yet.
        {
            if (handle is ILazilyResolves<T> lr)
            {
                var result = await lr.TryGetValue().ConfigureAwait(false);
                if (result.HasValue)
                {
                    return lr.Value;
                }
            }
            else
            {
                var result = (await handle.Resolve().ConfigureAwait(false)).ToRetrieveResult();
                if (result.IsFound() == true) return result.Value;
            }

            handle.Value = Activator.CreateInstance<T>();
            var putResult = await handle.Put().ConfigureAwait(false);
            if (putResult.IsSuccess != true)
            {
                throw new PersistenceException(putResult as IPersistenceResult, "Failed to create. Put result: " + putResult);
            }
            return handle.Value;
        }
    }
}
