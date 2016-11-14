using System;

namespace SAM1.CrossCuttingConcerns.Extensions
{
    public static class ObjectExtensions
    {
        public static int GetEnumValue(this Enum eventType)
        {
            return Convert.ToInt32(eventType);
        }
    }
}