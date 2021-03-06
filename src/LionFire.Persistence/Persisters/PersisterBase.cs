﻿using LionFire.Serialization;
using System;
using System.IO;

namespace LionFire.Persistence.Persisters
{
    public class PersisterBase<TOptions>
    {
        #region PersistenceOptions

        public TOptions PersistenceOptions
        {
            get => persistenceOptions;
            set
            {
                if (persistenceOptions != null) throw new AlreadySetException();
                persistenceOptions = value;
            }
        }
        private TOptions persistenceOptions;

        #endregion


        public virtual bool AllowAutoRetryForThisException(Exception e)
        { 
            // ENH - better approach for this -- maybe register exception types
            return !(
                e is SerializationException
                || e is FileNotFoundException
                );
        }

        protected virtual void OnDeserialized(object obj) => (obj as INotifyDeserialized)?.OnDeserialized();
    }
}
