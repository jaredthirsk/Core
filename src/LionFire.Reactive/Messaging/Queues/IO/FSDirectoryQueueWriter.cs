﻿using System;
using System.IO;
using LionFire.DependencyInjection;
using LionFire.Serialization;
using LionFire.Serialization.Contexts;

namespace LionFire.Messaging.Queues.IO
{

    public class FSDirectoryQueueWriter
    {

        #region QueueDir

        public string QueueDir
        {
            get { return queueDir; }
            set { queueDir = value;
                try
                {
                    if (queueDir != null && !Directory.Exists(queueDir)) Directory.CreateDirectory(queueDir);
                }
                catch { } // EMPTYCATCH
            }
        }
        private string queueDir;

        #endregion

        #region Relationships

        //[Inject]
        ISerializationService SerializationService
        {
            get
            {
                if (serializationService == null)
                {
                    serializationService = InjectionContext.Current.GetService<ISerializationService>();
                }
                return serializationService;
            }
        }
        ISerializationService serializationService;

        #endregion

        //[TryInject]
        public SerializationOptions SerializationOptions { get; set; } = new SerializationOptions();

        public void Enqueue(MessageEnvelope env)
        {
            Guid guid = Guid.NewGuid();

            var context = new FileSerializationContext
            {
                Flags = SerializationOptions.SerializationFlags
            };

            var bytes = SerializationService.ToBytes(env, context);
            var path = Path.Combine(QueueDir, guid + "." + context.FileExtension);

            using (var sw = new FileStream(path, FileMode.CreateNew))
            {
                sw.Write(bytes, 0, bytes.Length);
            }
        }
    }
}