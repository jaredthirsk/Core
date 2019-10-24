﻿using System;
using System.Collections.Generic;
using System.Text;
using LionFire.ObjectBus;
using LionFire.Ontology;
using LionFire.Referencing;

namespace LionFire.Vos
{
    public class VosReference : ReferenceBase<VosReference>, IVosReference
    {
        #region Ontology

        IOBase IHas<IOBase>.Object => VosOBus.Instance.DefaultOBase;
        IOBus IHas<IOBus>.Object => VosOBus.Instance;

        #endregion

        #region Construction and Implicit Construction

        public VosReference() { }

        public VosReference(string path)
        {
            Path = path;
        }
        public VosReference(params string[] pathComponents)
        {
            string path;
            for (int i = 0; i < pathComponents.Length; i++)
            {
                string chunk = pathComponents[i];
                pathComponents[i] = String.Concat(LionPath.SeparatorChar, chunk.TrimStart(LionPath.SeparatorChar));

                //if (i == pathComponents.Length - 1)
                //{
                //    // TODO: Clean trailing 2 slashes? probably don't need this.
                //}
            }

            path = string.Concat(pathComponents);
            Path = path;
        }

        public static implicit operator VosReference(string path) => new VosReference(path);

        public static IReference TryGetFromString(string referenceString)
        {
            var scheme = referenceString.GetUriScheme();
            if (scheme != UriSchemeDefault)
            {
                return null;
            }
            return new VosReference(referenceString);
        }

        #endregion

        #region GetHandle

        // Better/direct versions of the ObjectBus ToHandle

        public VobHandle<object> GetHandle(/*object obj = null*/) => new VobHandle<object>(this);
        public VobHandle<T> GetHandle<T>(/*TValue obj = default(TValue)*/) => new VobHandle<T>(this);
        //public VobHandle<TValue> ToHandle<TValue>(this TValue obj) => throw new NotImplementedException("FUTURE: if obj != null, create a NamedObjectHandle and assign a random key");

        public VobReadHandle<object> GetReadHandle(/*object obj = null*/) => new VobReadHandle<object>(this);
        public VobReadHandle<T> GetReadHandle<T>(/*TValue obj = default(TValue)*/) => new VobReadHandle<T>(this);
        //public VobReadOnlyHandle<TValue> ObjectToReadHandle<TValue>(this TValue obj) => throw new NotImplementedException("FUTURE: if obj != null, create a NamedObjectHandle and assign a random key");

        #endregion

        #region Scheme

        public const string UriSchemeDefault = "vos";
        public const string UriPrefixDefault = "vos:";

        public override IEnumerable<string> AllowedSchemes
        {
            get { yield return UriSchemeDefault; }
        }

        public override string Scheme => UriSchemeDefault;
        public static readonly string[] UriSchemes = new string[] { UriSchemeDefault };

        #endregion

        #region Key

        public override string Key
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (Host != null)
                {
                    sb.Append(LionPath.HostDelimiter);
                    sb.Append(Host);
                }
                if (Port != null)
                {
                    sb.Append(":");
                    sb.Append(Port);
                }
                if (Path != null)
                {
                    sb.Append(Path);
                }
                if (Package != null)
                {
                    sb.Append("|");
                    sb.Append(Package);
                }
                if (Location != null)
                {
                    sb.Append("^");
                    sb.Append(Location);
                }
                return sb.ToString();
            }
            protected set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    Reset();
                    return;
                }
                int index = 0;
                if (value.StartsWith(LionPath.HostDelimiter))
                {
                    index += LionPath.HostDelimiter.Length;
                    Host = value.Substring(index, index + value.IndexOfAny(LionPath.Delimiters, index));
                }
                if (value[index] == LionPath.PortDelimiter)
                {
                    index++;
                    Port = value.Substring(index, index + value.IndexOfAny(LionPath.Delimiters, index));
                }
                if (value[index] == LionPath.PathDelimiter)
                {
                    //index += PathDelimiter.Length; -- Keep the initial /
                    //Path = value.Substring(index);
                    Path = value.Substring(index, index + value.IndexOfAny(LionPath.Delimiters, index));
                }
                if (value[index] == VosPath.LayerDelimiter)
                {
                    throw new NotImplementedException("TODO: Reimplement Layer delimiter");
                    //index++;
                    //Package = value.Substring(index, index + value.IndexOfAny(VosPath.Delimiters, index));
                }
            }
        }
        
        #endregion


        #region Type

        public Type Type
        {
            get { return type; }
            set
            {
                if (type == value) return;
                if (type != default(Type)) throw new AlreadySetException();
                type = value;
            }
        }
        private Type type;

        #endregion

        public override string Host { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string Port { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string Path { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Package { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Location { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private void Reset()
        {
            throw new NotImplementedException("Don't allow this anymore?");
            ////this.Host = null;
            ////this.Port = null;
            //Path = null;
            //Package = null;
            //this.TypeName = null;
        }

#if TOPORT

        #region Convenience

    public VBase Vos { get { return VosOBaseProvider.Instance.DefaultVos; } }

    public Vob Vob
    {
        get
        {
            return this.Vos[Path];
        }
    }

#if !AOT
    public override IHandle<T> GetHandle<T>(T obj)
    //where T : class//, new()
    {
        var vh = GetVobHandle<T>();
        if (obj != null)
        {
            vh.Object = obj;
        }
        return vh;
    }

#endif

    public VobHandle<T> GetVobHandle<T>()
        where T : class//, new()
    {
        //return new VobHandle<T>(this.GetVob());// OLD 
        //return this.GetVob().GetHandle<T>();
        return this.Vob.GetHandle<T>();
    }

        #endregion

    
#endif

        #region Misc

        public override string ToString()
        {
            //return String.Concat(UriPrefixDefault, Path);
            return String.Concat(UriPrefixDefault, Key);
        }

        public override bool Equals(object obj)
        {
            VosReference other = obj as VosReference;
            if (other == null)
            {
                return false;
            }

            if (!Key.Equals(other.Key))
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode() => Key.GetHashCode();

        #endregion

        //public static class VosReferenceExtensions
        //{
        //    public static Vob GetVob(this VosReference vosReference)
        //    {
        //        if (vosReference == null) throw new ArgumentNullException("vosReference");
        //        return Vos.Default[vosReference.Path];
        //    }
        //}

    }
}
