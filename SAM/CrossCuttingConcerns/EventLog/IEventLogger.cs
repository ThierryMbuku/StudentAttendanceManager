using System;

namespace SAM1.CrossCuttingConcerns.EventLog
{
    public enum EventType
    {
        User_Authorisation = 1,
        User_Authentication,
        User_Logout,
        Admin_Register_Student,
        Admin_Update_Student,
        Admin_Create_Report,
        User_Find_Student
    }

    public enum EventSeverity
    {
        Informational = 1,
        Error
    }

    public interface IEventLogger
    {
        void LogEvent(int userId, EventType eventType, EventSeverity severity);
        void AddMetaData(string metadata);
        void LogEvent(string studentNo, EventType user_Find_Student, EventSeverity informational);
    }
}