using System;
using System.Diagnostics;
using DesignPatterns;

namespace TimeMachine
{
    public class TimeMachine<T> : IDisposable, ISingleton
    {
        public static TimeMachine<T> Instance => Singleton<TimeMachine<T>>.Instance;

        private static TimeChanger timechanger = new TimeChanger();
        private static WindowsTimeService windowsTime = WindowsTimeService.Instance;

        private T lastItem;
        private Action<T> lastAction;
        private Action<T> lastCallback;

        private TimeMachine() { }

        public void RunAt(DateTime time, T item, Action<T> work, Action<T> callback)
        {
            try
            {
                lastItem = item;
                lastAction = work;
                lastCallback = callback;

                windowsTime.Disable();
                if (windowsTime.IsDisabled)
                {
                    timechanger.ChangeTime(time);
                    work(item);
                    callback(item);
                }

                windowsTime.Enable();
                windowsTime.Start();

                Debug.WriteLine(!windowsTime.IsDisabled);
            }
            catch (Exception)
            {
                throw;
            }

        }

        private bool isDisposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing) { }

                lastAction = null;
                lastCallback = null;

                if (lastItem is IDisposable disposableItem)
                {
                    disposableItem.Dispose();
                }
                else
                {
                    lastItem = default(T);
                }

                isDisposed = true;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }
    }
}
