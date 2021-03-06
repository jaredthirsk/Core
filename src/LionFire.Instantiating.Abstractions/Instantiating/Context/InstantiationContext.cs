﻿using LionFire.MultiTyping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using LionFire.Structures;
using LionFire.Types;

namespace LionFire.Instantiating
{
    public class InstantiationContext : MultiTypable
    {

        public InstantiationContext() { }
        public InstantiationContext(object rootObject) { this.RootObject = rootObject; }


        public object RootObject { get; set; }
        public HashSet<object> Dependencies { get; set; } = new HashSet<object>();

        public LoadingContext Loading
        {
            get { return this.AsTypeOrCreate<LoadingContext>(); }
        }
        public SavingContext Saving
        {
            get { return this.AsTypeOrCreate<SavingContext>(); }
        }

        public TypeResolver TypeNaming
        {
            get { return this.AsTypeOrInject<TypeResolver>(); }
            set { this.SetType<TypeResolver>(value); }
        }

        public static InstantiationContext Default => ManualSingleton<InstantiationContext>.GuaranteedInstance;

        public bool? AllowInstantiator { get; set; }
    }

}
