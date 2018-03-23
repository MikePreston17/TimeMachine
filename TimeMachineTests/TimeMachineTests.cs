using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Reflection;
using TimeMachine;

namespace TimeMachineTests
{
    [TestClass]
    public class TimeMachineTests
    {
        [TestMethod]
        public void FromWhenAndBackAgain()
        {
            //
            ///TODO:
            ///can change time
            ///can change time then change back.
            ///can wrap code in time machine.
            ///can run callback code in time machine.
            ////
            try
            {
                using (var timemachine = TimeMachine<WorkQueue>.Instance)
                {
                    var workQueue = new WorkQueue();
                    timemachine.RunAt(new DateTime(2017, 3, 20, 6, 15, 30), workQueue,
                    work:
                    (queue) =>
                    {
                        queue.Add("timey wimey");
                    },
                    callback:
                    (queue) =>
                    {
                        Debug.WriteLine("Done");
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0}: {1}", MethodBase.GetCurrentMethod().Name, ex.ToString()));
            }


        }
    }
}
