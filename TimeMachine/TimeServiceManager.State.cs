using System;

namespace TimeMachine
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
