﻿using LionFire.Assets;
using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Structures;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace LionFire.Assets
{
    public interface IAssetReadHandle : IReferencable<IAssetReference>, IReadHandle
    {
    }
    public interface IHRAsset : IAssetReadHandle // PORTINGGUIDE IHRAsset > RAsset
    {
        // PORTINGGUIDE - Type > TreatAsType
    } // TEMP TOPORT

    //public class ReferencableConverter : JsonConverter
    //{
    //    public override bool CanConvert(Type objectType)
    //        => objectType.IsConstructedGenericType && objectType.GetGenericTypeDefinition() == typeof(RAsset<>);

    //    static ConcurrentDictionary<Type, MethodInfo> FromKeyMethods { get; } = new ConcurrentDictionary<Type, MethodInfo>();

    //    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
    //        throw new NotImplementedException();
    //    }
    //    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    //    {
    //        writer.WriteValue((IReferencable) value )
    //    }
    //}

    public class KeyableConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => typeof(IKeyable<string>).IsAssignableFrom(objectType) && !objectType.IsInterface && !objectType.IsAbstract;

        //static ConcurrentDictionary<Type, MethodInfo> FromKeyMethods { get; } = new ConcurrentDictionary<Type, MethodInfo>();

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = (IKeyable<string>)existingValue;
#if DEBUG
            if (result != null && result.Key != reader.Value as string)
            {
                Debug.WriteLine($"[deserialize] {this.GetType().Name} overwriting existing object of type {existingValue.GetType().Name}: {result.Key} => {reader.Value}");
            }
#endif

            //result ??= (IKeyable<string>)Activator.CreateInstance(objectType);
            result = (IKeyable<string>)Activator.CreateInstance(objectType);

            result.Key = reader.Value as string;
            return result;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((IKeyed<string>)value).Key);
        }
    }
    [JsonConverter(typeof(KeyableConverter))]
    public class RAsset<TValue> : ReadHandlePassthrough<TValue, IAssetReference>, IAssetReadHandle, IKeyable, IEquatable<RAsset<TValue>> where TValue : IAsset<TValue>
    {
        public new string Key
        {
            get => base.Key;
            set => Reference = new AssetReference<TValue>(value);
        }

        public static RAsset<TValue> FromKey(string key)
        {
            throw new NotImplementedException();
        }


        #region Construction and Implicit Operators

        public static implicit operator RAsset<TValue>(string assetPath) => assetPath == default ? default : new RAsset<TValue> { Reference = new AssetReference<TValue>(assetPath) };
        public static implicit operator RAsset<TValue>(TValue asset) => Object.Equals(asset, default(TValue)) ? default : new RAsset<TValue> { Reference = (AssetReference<TValue>)asset.Reference, Value = asset };
        public static implicit operator RAsset<TValue>(RWAsset<TValue> asset) => asset == null ? null : new RAsset<TValue>(asset.ReadWriteHandle); // TOFLYWEIGHT
        public static implicit operator AssetReference<TValue>(RAsset<TValue> asset) => asset == null ? null : asset.Reference;
        public static implicit operator TValue(RAsset<TValue> rAsset) => rAsset == null ? default : rAsset.Value;

        public RAsset() { }
        public RAsset(IReadHandle<TValue> handle) : base(handle) { }

        #endregion

        public string AssetPath => Reference.Path;
        public new AssetReference<TValue> Reference { get => (AssetReference<TValue>)base.Reference; set => base.Reference = value; }

        public static RAsset<TValue> Get(string assetPath)
            => assetPath;

        public override string ToString() => Reference.ToString();

        #region Equality

        public bool Equals(RAsset<TValue> other) => other != null && Key == other.Key;
        public override bool Equals(object obj) => Equals(obj as RAsset<TValue>);
        public override int GetHashCode() => 990326508 + EqualityComparer<string>.Default.GetHashCode(Key);

        public static bool operator ==(RAsset<TValue> left, RAsset<TValue> right) => EqualityComparer<RAsset<TValue>>.Default.Equals(left, right);
        public static bool operator !=(RAsset<TValue> left, RAsset<TValue> right) => !(left == right);

        #endregion
    }

    public static class RAsset
    {
        public static RAsset<TValue> Get<TValue>(string assetPath)
            where TValue : IAsset<TValue> => assetPath;
    }


    public static class RAssetExtensions
    {
        // UNUSED UNNEEDED?
        public static RAsset<TValue> ToRAsset<TValue>(this IReadHandle<TValue> readHandle)
            where TValue : IAsset<TValue>
            => new RAsset<TValue>(readHandle);

        public static RAsset<TValue> ToRAsset<TValue>(this string assetPath)
         where TValue : IAsset<TValue>
         => assetPath;
    }
}