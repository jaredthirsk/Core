﻿using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence.Handles
{
#if !AOT
    public interface IHasHandle<T>
        //: IHasHandle
        where T : class
    {
        IReadWriteHandleBase<T> Handle { get; }
    }
#endif

}
