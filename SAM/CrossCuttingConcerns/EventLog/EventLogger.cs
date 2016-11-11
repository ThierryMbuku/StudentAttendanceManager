using System;
using System.Data.Entity.Validation;
using System.Linq;

namespace SAM1.CrossCuttingConcerns.EventLog
{
    public class EventLogger : IEventLogger
    {
        private string metaData;
        public void AddMetaData(string extraInfo)
        {
            metaData = extraInfo;
        }

        public void LogEvent(int userId, EventType eventType, EventSeverity severity)
        {
            using (var db = new SAMEntities())
            {
                try
                {
                    var eventSeverity = severity.ToString();
                    var eventTypeDescription = eventType.ToString();
                    var eventLog = new SAM1.EventLog
                    {
                        CreateDate = DateTime.Now,
                        UserId = userId,
                        Severity = eventSeverity,
                        EventType = new SAM1.EventType
                        {
                            Description = eventTypeDescription,
                        },
                        MetaData = metaData
                    };

                    db.EventLogs.Add(eventLog);
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    var errorMessages = ex.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.ErrorMessage + " " + x.PropertyName);

                    var fullErrorMessage = string.Join(" => ", errorMessages);
                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                    throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                }
            }
        }
    }
}