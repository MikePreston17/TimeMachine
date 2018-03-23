using System;

namespace TimeMachine
{

    public partial class TimeServiceManager
    {
        [Flags]
        public enum State
        {
            Boot = 0,
            System = 1,
            Automatic = 2,
            Manual = 3,
            Disabled = 4,
        }

    }
}
