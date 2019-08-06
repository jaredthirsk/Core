﻿using LionFire.Collections;
using LionFire.ObjectBus.Handles;
using LionFire.Persistence;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectBus.Filesystem
{
    public class FsOBoc : FsOBoc<object>
    {

        #region Construction

        public FsOBoc() { }
        public FsOBoc(FileReference reference) : base(reference)
        {
        }

        #endregion

    }

    public class FsOBoc<TObject> : SyncableOBoc<TObject, FsListEntry>
    {

        #region Construction

        public FsOBoc() { }
        public FsOBoc(FileReference reference) : base(reference)
        {
        }

        #endregion

        public override bool IsReadSyncEnabled { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public override IEnumerator<TObject> GetEnumerator() => throw new System.NotImplementedException();

        public override async Task<IRetrieveResult<INotifyingReadOnlyCollection<FsListEntry>>> RetrieveObject()
        {
            var dir = Reference.Path;

            return await Task.Run(async () =>
            {
                if (!Directory.Exists(dir))
                {
                    if (HasObject)
                    {
                        var fsList = (FsList)Object;
                        fsList.OnDirectoryDoesNotExist();
                    }
                    // OPTIMIZE: Use RetrieveResult<T>.NotFound
                    return (IRetrieveResult<INotifyingReadOnlyCollection<FsListEntry>>)new RetrieveResult<INotifyingReadOnlyCollection<FsListEntry>>
                    {
                        Flags = PersistenceResultFlags.NotFound
                    };
                }

                if (HasObject)
                {
                    var fsList = (FsList)Object;
                    await fsList.Refresh();
                    OnRetrievedObjectInPlace();
                }
                else
                {
                    var obj = new FsList(dir);
                    await obj.Refresh();
                    OnRetrievedObject(obj);
                }

                return (IRetrieveResult<INotifyingReadOnlyCollection<FsListEntry>>)new RetrieveResult<INotifyingReadOnlyCollection<FsListEntry>>
                {
                    Object = Object,
                };
            });

        }
    }
}
