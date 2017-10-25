﻿using LionFire.Applications.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;
using LionFire.Assets.Providers.FileSystem;
using LionFire.Assets;
using LionFire.DependencyInjection;
using LionFire.MultiTyping;
using LionFire.Structures;
using LionFire.Execution;
using LionFire.Persistence;
using Newtonsoft.Json.Serialization;
using LionFire.Instantiating;
using Newtonsoft.Json.Linq;

namespace LionFire.Assets.Providers.FileSystem
{
    public class ShouldSerializeContractResolver : DefaultContractResolver
    {
        public static readonly ShouldSerializeContractResolver Instance = new ShouldSerializeContractResolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            var attr = member.GetCustomAttribute(typeof(SerializeIgnoreAttribute));
            if (attr != null)
            {
                property.ShouldSerialize = instance => false;
            }
            else if (member as PropertyInfo != null)
            {
                var pi = (PropertyInfo)member;
                if (!pi.CanWrite || !pi.SetMethod.IsPublic) property.ShouldSerialize = instance => false;
            }
            else
            {
                var fi = member as FieldInfo;
                if (fi != null)
                {
                    if (fi.IsInitOnly) property.ShouldSerialize = instance => false;
                }
            }

            return property;
        }
    }
}