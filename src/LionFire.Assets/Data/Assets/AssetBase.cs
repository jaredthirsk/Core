﻿using System;

namespace LionFire.Assets
{
    public class AssetBase : AssetBase<object> { }

    public class AssetBase<TValue> : AssetBaseBase<TValue>
    {

        public AssetBase() { }
        public AssetBase(AssetReference<TValue> reference) : base(reference) { }

        #region Key

        public override string Key
        {
            get => Reference?.ToString();
            //set => Reference = value;
            //set => Reference = AssetReference<TValue>.FromKey(value);
        }
        ///// <summary>
        ///// This is where AssetPath is stored.  Reference creates an AssetReference from this via AssetPath.
        ///// </summary>
        //public override string Key 
        //{
        //    get => key;
        //    set
        //    {
        //        if (key == value) return;
        //        key = value;
        //        OnKeyChanged();
        //        OnPropertyChanged(nameof(Key));
        //    }
        //}
        //private string key;

        #endregion
    }
}
