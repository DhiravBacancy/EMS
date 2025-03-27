//using Newtonsoft.Json;

namespace EMS.Enums
{
    //[JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum LeaveTypeEnum
    {
        SickLeave = 1,
        CasualLeave = 2,
        Vacation = 3,
        UnpaidLeave = 4
    }
}

