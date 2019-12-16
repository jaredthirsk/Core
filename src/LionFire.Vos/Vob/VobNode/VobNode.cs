﻿using LionFire.Collections;
using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Vos
{
    /// <summary>
    /// VobNodes contain the data for heavier Vobs that have complex things such as mounts or other metadata
    /// </summary>
    public partial class VobNode : IVobNodeFacade
    {

        #region Identity

        public bool IsVobNode => true;
        public Vob Vob { get; }

        #endregion

        #region Relationships

        #region ParentBaseVob

        public VobNode ParentBaseVob
        {
            get
            {
                Vob ancestor = Vob.Parent;
                while (ancestor != null && ancestor.VobNode == null) ancestor = ancestor.Parent;

                return ancestor?.VobNode;
            }
        }

        #endregion

        #endregion

        #region Options

        public VobNodeOptions Options { get; set; }

        #endregion

        #region Construction

        public VobNode(Vob vob)
        {
            Vob = vob;
            VobDepth = LionPath.GetAbsolutePathDepth(vob.Path);
        }

        #endregion

        #region Derived

        internal int VobDepth { get; }

        #endregion


        #region OLD / REVIEW - VobsWithMounts



        //internal MultiBindableCollection<Vob> VobsWithMounts => vobsWithMounts;
        //private readonly MultiBindableCollection<Vob> vobsWithMounts = new MultiBindableCollection<Vob>();

        internal void OnMountsChangedFor(Vob changedVob, INotifyCollectionChangedEventArgs<Mount> e)
        {
            throw new NotImplementedException(); // Still needed?
            //foreach (var vobWithMounts in VobsWithMounts.ToArray())
            //{
            //    if (changedVob.IsAncestorOf(vobWithMounts))
            //    {
            //        vobWithMounts.OnAncestorMountsChanged(changedVob, e);
            //    }
            //}
        }

        #endregion

        #region Mounts

        #region HasMounts

        public bool HasLocalReadMounts => localReadMount != null || localReadMounts?.Any() == true;
        public bool HasLocalWriteMounts => localWriteMount != null || localWriteMounts?.Any() == true;

        public bool HasLocalMounts => HasLocalReadMounts || HasLocalWriteMounts;

        //public bool HasMounts
        //{
        //    get => hasMounts;
        //    private set
        //    {
        //        if (hasMounts == value)
        //        {
        //            return;
        //        }

        //        hasMounts = value;

        //        if (hasMounts)
        //        {
        //            NextVobNode.VobsWithMounts.Add(this);
        //        }
        //        else
        //        {
        //            NextVobNode.VobsWithMounts.Remove(this);
        //        }
        //        InitializeEffectiveMounts();
        //    }
        //}
        //private bool hasMounts;

        //private void UpdateHasMounts() => HasMounts = mounts != null && mounts.Count > 0;

        #endregion

        internal bool InitializeEffectiveMounts() => throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mount"></param>
        /// <param name="force">
        ///  - If there are existing mount(s) and mount is exclusive, replace existing mount(s)
        /// Does not force:
        ///  - Overriding sealed on parent
        /// </param>
        public void Mount(Mount mount, bool force = false)
        {
            bool changedState = false;
            if (mount.MountOptions.ReadPriority.HasValue && MountRead(mount, force)) changedState = true;
            if (mount.MountOptions.WritePriority.HasValue && MountWrite(mount, force)) changedState = true;
            if (changedState) OnMountStateChanged();
        }
        protected virtual void OnMountStateChanged()
        {
            MountStateVersion++;
            // FUTURE: Proactively invalidate and recalculate (at low thread priority) mount resolutions for child VobNodes.
        }
        public bool CanHaveMultiReadMounts { get { throw new NotImplementedException(); } }
        public bool CanHaveMultiWriteMounts { get { throw new NotImplementedException(); } }

        private bool MountRead(Mount mount, bool force = false)
        {
            #region Validation

            if (localReadMounts == null && localReadMount == null)
            {
                localReadMount = mount;
                return true;
            }
            if (localWriteMount != null && (localWriteMount.MountOptions.IsExclusiveWithReadAndWrite || mount.MountOptions.IsExclusiveWithReadAndWrite))
            {
                throw new VosException("Already has write mount, but either this mount or existing write mount has IsExclusiveWithReadAndWrite == true.");
            }
            if (localReadMount != null)
            {
                if (!CanHaveMultiReadMounts)
                {
                    throw new VosException("Already has read mount, but not allowed to have multiple read mounts.  First unmount the existing read mount.");
                }
                if (localReadMount.MountOptions.IsExclusive || mount.MountOptions.IsExclusive)
                {
                    throw new VosException("Already has read mount, but one or both mounts has IsExclusive == true.  First unmount one of the mounts, or disable the IsExclusive flag(s) before mounting.");
                }
            }

            #endregion


            if (localReadMount == null)
            {
                if (localReadMounts == null || !localReadMounts.Any())
                {
                    localReadMounts = null;
                    localReadMount = mount;
                }
            }

            throw new NotImplementedException();
        }

        protected IEnumerable<Mount> AllEffectiveReadMounts
        {
            get
            {
                if (localReadMount != null) yield return localReadMount;
                if (localReadMounts != null) foreach(var m in localReadMounts) yield return m;
            }
        }
        protected IEnumerable<Mount> AllEffectiveWriteMounts
        {
            get
            {
                if (localWriteMount != null) yield return localWriteMount;
                if (localWriteMounts != null) foreach (var m in localWriteMounts) yield return m;
            }
        }

        private bool MountWrite(Mount mount, bool force = false)
        {
            throw new NotImplementedException();
        }

        public void Unmount(string mountKey)
        {
            throw new NotImplementedException();
        }
        public int MountStateVersion { get; protected set; }

        public Mount localReadMount;
        public List<Mount> localReadMounts;

        public Mount localWriteMount;
        public List<Mount> localWriteMounts;

        #endregion

        #region ReadMountsCache

        public int ReadMountsVersion { get; set; } = 0;
        public IEnumerable<KeyValuePair<int, Mount>> ReadMounts
        {
            get
            {
                var cache = ReadMountsCache;
                if (cache == null)
                {
                    var list = AllEffectiveReadMounts.ToList();
                    switch (list.Count)
                    {
                        case 1:
                            readMountsCache = new SingleMountResolutionCache(Vob, ReadMountsVersion++, list[0]);
                            break;
                        case 0:
                            readMountsCache = null;
                            break;
                        default:
                            readMountsCache = new MultiMountResolutionCache(Vob, ReadMountsVersion++, AllEffectiveReadMounts, PersistenceDirection.Read);
                            break;
                    }
                }
                return readMountsCache.Mounts;
            }
        }
        IMountResolutionCache ReadMountsCache => (readMountsCache != null && (ParentBaseVob == null || readMountsCache.Version == ParentBaseVob.MountStateVersion)) ? readMountsCache : null;
        IMountResolutionCache readMountsCache;

        #endregion

        #region Write Mounts Cache

        public int WriteMountsVersion { get; set; } = 0;


        public IEnumerable<KeyValuePair<int, Mount>> WriteMounts
        {
            get
            {
                throw new NotImplementedException();
                //if (writeMountsCache == null)
                //{

                //}
                //return writeMountsCache;
            }
        }
        //IMountResolutionCache writeMountsCache;

        #endregion

    }

}
