﻿using LionFire.Ontology;
using System;

namespace LionFire.Persistence
{
    // TODO: sort out nomenclature for Read vs Retrieve, propagate these interfaces to the right places

    [Flags]
    public enum PersistenceResultFlags
    {
        None = 0,

        /// <summary>
        /// True if operation was successful, even if retrieve did not find anything
        /// </summary>
        Success = 1 << 0,

        /// <summary>
        /// Exception
        /// </summary>
        Fail = 1 << 1, // RENAME to exception?

        Found = 1 << 2,
        
        NotFound = 1 << 3,

        Retrieved = 1 << 4,
        
        /// <summary>
        /// Retrieved null (for reference types) or default value (for value types)
        /// </summary>
        RetrievedNullOrDefault = 1 << 5,

        Indeterminate = 1 << 6,

        /// <summary>
        /// When checking for whether an operation is possible, this is set if the operation is expected to succeed.
        /// </summary>
        PreviewSuccess = 1 << 20,

        /// <summary>
        /// When checking for whether an operation is possible, this is set if the operation is expected to fail.
        /// </summary>
        PreviewFail = 1 << 21,
        
        PreviewIndeterminate = 1 << 22,

        //PreviewNotFound = 1 << 23, // Not used yet. Should it be?
        MountNotAvailable = 1 << 28,
        SerializerNotAvailable = 1 << 29,
        ProviderNotAvailable = 1 << 30,

        Noop = 1 << 31,
    }

    public static class PersistenceResultFlagsExtensions
    {
        public static bool? IsFound(this PersistenceResultFlags flags)
        {
            if (flags.HasFlag(PersistenceResultFlags.Found)) return true;
            if (flags.HasFlag(PersistenceResultFlags.NotFound)) return false;
            return null;
        }

        public static PersistenceResult ToResult(this PersistenceResultFlags flags) => new PersistenceResult { Flags = flags };
    }
}