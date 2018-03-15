using System.ComponentModel;

namespace TimeMachine
{
    //[TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum ServiceOption
    {
        [Description("N/a")]
        Default,
        [Description("Start Windows Time Service")]
        Start,
        [Description("Stop Windows Time Service")]
        Stop,
        [Description("Restart Windows Time Service")]
        Restart,
        [Description("Disable Windows Time Service")]
        Disable,
        [Description("Enable Windows Time Service")]
        Enable,
    }
}
