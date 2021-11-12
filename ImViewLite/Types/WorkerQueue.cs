using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImViewLite.Misc
{
    public class WorkerQueue : IDisposable
    {
        public delegate void ProcessFileEvent(object name);
        public event ProcessFileEvent ProcessFile;

        private readonly object _Locker = new object();
        private EventWaitHandle _EventWaitHandle = new AutoResetEvent(false);
        private Queue<object> _FileNamesQueue = new Queue<object>();

        private Thread _Worker;
        private bool _Working = true;

        public WorkerQueue()
        {
            _Worker = new Thread(Work);
            _Worker.Start();
        }

        public void KillWorker()
        {
            _Working = false;
            EnqueueFileName(null);
        }

        public void EnqueueFileName(object FileName)
        {
            lock (_Locker)
            {
                _FileNamesQueue.Enqueue(FileName);
            }
            _EventWaitHandle.Set();
        }

        private void OnProcessFile(object filename)
        {
            if (ProcessFile != null)
            {
                ProcessFile.Invoke(filename);
            }
        }

        private void Work()
        {
            while (_Working)
            {
                object fileName = null;

                lock (_Locker)
                {
                    if (_FileNamesQueue.Count > 0)
                    {
                        fileName = _FileNamesQueue.Dequeue();
                    }
                }
                if (fileName != null)
                {
                    OnProcessFile(fileName);
                }
                else
                {
                    // No more file names - wait for a signal
                    _EventWaitHandle.WaitOne();
                }
            }
        }

        public void Dispose()
        {
            KillWorker();
            _Worker.Join();
            _EventWaitHandle.Close();
            GC.SuppressFinalize(this);
        }
    }
}
