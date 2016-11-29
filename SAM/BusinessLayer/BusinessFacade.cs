using SAM1.Models;
using System.Data.Entity.Validation;
using System.Linq;
using SAM1.CrossCuttingConcerns.Extensions;
using SAM1.CrossCuttingConcerns.ResponseModels;
using System.Collections.Generic;
using SAM1.CrossCuttingConcerns.EventLog;
using System;
using System.Globalization;
using System.IO;
using System.Web.Mvc;
using System.IO.Ports;
using SAM1.CrossCuttingConcerns.Enums;

namespace SAM1.BusinessLayer
{
    public class BusinessFacade
    {
        private readonly IEventLogger eventLogger;

        internal BusinessFacade()
        {
            eventLogger = new EventLogger();
        }

        //WORKS
        internal LogonResponseModel AuthoriseAccessCard()
        {
            var responseModel = new LogonResponseModel();

            // Read Student UID from Card
            var cardId = GetCardId();
            using (var db = new SAMEntities())
            {
                var scannedCard = db.AccessCards.FirstOrDefault(x => x.CardId == cardId && x.CardType == (int)AccessCardTypes.Administrator);
                if (scannedCard != null)
                {
                    var authorisedUser = db.Users.FirstOrDefault(u => u.Id == scannedCard.UserId && u.IsAdmin);
                    // Authenticate User
                    if (authorisedUser == null)
                    {
                        responseModel.IsAuthorised = false;
                        responseModel.SetErrorMessage("Invalid combination of card and userId");
                        eventLogger.LogEvent(scannedCard.CardId, CrossCuttingConcerns.EventLog.EventType.User_Authentication, CrossCuttingConcerns.EventLog.EventSeverity.Error);
                    }
                    else
                    {
                        responseModel.IsAuthorised = true;
                        responseModel.SetUserId(authorisedUser);
                    }

                    responseModel.SetAuthorisationUrl(responseModel.IsAuthorised, authorisedUser);
                }
                else
                {
                    throw new UnauthorizedAccessException("SAM does not recognize your card!");
                }
            }

            return responseModel;
        }
        //WORKS
        internal string RegisterStudent(UserModel userModel)
        {
            //perform businness rule validation
            // if (success)
            // write data by calling mmethod in Data Access Layer           
            using (var db = new SAMEntities())
            {
                try
                {
                    var user = new User
                    {
                        //Username = userModel.Username,

                        FirstName = userModel.FirstName,
                        LastName = userModel.LastName,
                        Email = userModel.Email,
                        CellPhone = userModel.CellPhone,
                        PasswordHash = userModel.Password.GenerateHash(),
                        IsAdmin = userModel.IsAdmin,
                        AuthenticationCode = userModel.AuthenticationCode,
                        RegistrationDate = DateTime.Now.ToUniversalTime(),
                        Address = new Address
                        {
                            Complex = userModel.Address.ComplexNumber,
                            Street = userModel.Address.Street,
                            Suburb = userModel.Address.Suburb,
                            City = userModel.Address.City,
                            PostalCode = Convert.ToInt32(userModel.Address.PostalCode),
                        }
                    };

                    db.Users.Add(user);
                    db.SaveChanges();

                    eventLogger.LogEvent(user.Id, CrossCuttingConcerns.EventLog.EventType.Admin_Register_Student, CrossCuttingConcerns.EventLog.EventSeverity.Informational);

                    return $"{userModel.FirstName} {userModel.LastName} Successfully registered";
                }
                catch (DbEntityValidationException ex)
                {
                    var errorMessages = ex.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.ErrorMessage + " " + x.PropertyName);

                    var fullErrorMessage = string.Join(" => ", errorMessages);
                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                    eventLogger.LogEvent(userModel.AdminUserId, CrossCuttingConcerns.EventLog.EventType.Admin_Register_Student, CrossCuttingConcerns.EventLog.EventSeverity.Error);
                    throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                }
            }
        }

        //WORKS
        internal LogonResponseModel AuthenticateUser(LogonUserModel user)
        {
            var responseModel = new LogonResponseModel();

            //1) Go to the db and get the userId            
            using (var db = new SAMEntities())
            {
                var authenticatedAccessCard = db.AccessCards.FirstOrDefault(x => x.UserId == user.UserId);
                var authenticatedUser = db.Users.FirstOrDefault(x => x.Id == authenticatedAccessCard.UserId);

                bool isAuthenticated = false;
                if (authenticatedAccessCard != null)
                {
                    //2) Go and check the challenge inputted == the stored challenge in the db
                    isAuthenticated = string.Equals(authenticatedUser.AuthenticationCode, user.ChallengeResponse);
                    responseModel.IsAuthenticated = isAuthenticated;
                }

                responseModel.SetAuthenticationUrl(isAuthenticated, authenticatedUser);
                responseModel.SetUserId(authenticatedUser);

                if (!isAuthenticated)
                {
                    responseModel.SetErrorMessage("Invalid username or password");
                    eventLogger.LogEvent(authenticatedUser.Id, CrossCuttingConcerns.EventLog.EventType.User_Authentication, CrossCuttingConcerns.EventLog.EventSeverity.Error);
                }
                else
                {
                    eventLogger.LogEvent(authenticatedUser.Id, CrossCuttingConcerns.EventLog.EventType.User_Authentication, CrossCuttingConcerns.EventLog.EventSeverity.Informational);
                }

                return responseModel;
            }
        }

        //WORKS
        private string GetCardId()
        {
            SerialPort myport = new SerialPort();
            myport.BaudRate = 9600;
            myport.PortName = "COM3";
            myport.Close();
            myport.Open();
            bool whileCondition = true;
            string cardId = "";
            string scannedCardId = "";

            while (whileCondition)
            {
                scannedCardId = myport.ReadLine();
                if (scannedCardId.Contains("Card UID"))
                {
                    var cardInfo = scannedCardId.Split(':');
                    if (cardInfo.Any())
                    {
                        cardId = cardInfo[1].Replace(Environment.NewLine, string.Empty);
                        cardId = cardInfo[1].Replace("\r", string.Empty);
                        cardId = cardId.Replace(" ", string.Empty);
                        whileCondition = false;
                    }
                }
            }
            myport.Close();
            return cardId;
        }

        internal List<UserModel> StudentList(string studentid = "")
        {
            var students = new List<UserModel>();

            using (var db = new SAMEntities())
            {
                var users = db.Users.Where(x => !x.IsAdmin).ToList();
                var eventLogs = db.EventLogs.Distinct().ToList();

                foreach (var user in users)
                {
                    var registrationEventLog = eventLogs.FirstOrDefault(x => x.UserId == user.Id && x.EventTypeId == CrossCuttingConcerns.EventLog.EventType.Admin_Register_Student.GetEnumValue());
                    var signinEventLog = eventLogs.LastOrDefault(x => x.UserId == user.Id && x.EventTypeId == CrossCuttingConcerns.EventLog.EventType.User_Authorisation.GetEnumValue());
                    students.Add(new UserModel
                    {
                        //Username = user.AccessCards.ToString(), -- this maps to the access cards table
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        CellPhone = user.CellPhone,
                        RegistrationDate = registrationEventLog.CreateDate.ToShortDateString(),
                        SigninDate = signinEventLog == null ? "Not Logged In" : signinEventLog.CreateDate.ToShortDateString(),
                        SigninTime = signinEventLog == null ? "Not Logged In" : signinEventLog.CreateDate.ToShortTimeString(),
                    });
                }
                return students;
            }
        }

        internal StudentModel FindStudent(int studentId)
        {
            using (var db = new SAMEntities())
            {
                var dbStudent = db.Users.FirstOrDefault(x => x.Id == studentId && !x.IsAdmin);

                eventLogger.LogEvent(studentId, CrossCuttingConcerns.EventLog.EventType.User_Find_Student, CrossCuttingConcerns.EventLog.EventSeverity.Informational);

                return dbStudent == null ? null : new StudentModel
                {
                    Id = dbStudent.Id,
                    FirstName = dbStudent.FirstName,
                    CellPhone = dbStudent.CellPhone,
                    LastName = dbStudent.LastName,
                    Email = dbStudent.Email,
                    //Username = dbStudent.Username,
                };
            }
        }

        internal SigninProgressResponseModel CalculateStudentProgess(int studentId)
        {
            using (var db = new SAMEntities())
            {

                //Sign In

                var eventTypeId = CrossCuttingConcerns.EventLog.EventType.User_Authentication.GetEnumValue();

                //Get the current week
                var currentDateTimeFormatInfo = DateTimeFormatInfo.CurrentInfo;
                var calendar = currentDateTimeFormatInfo.Calendar;
                var weekNumber = calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday); //46

                var eventSeverityId = (int)CrossCuttingConcerns.EventLog.EventSeverity.Informational;
                var authenticationEvents = db.EventLogs.Where(x => x.UserId == studentId && x.EventTypeId == eventTypeId && x.EventSeverityId == eventSeverityId).ToList();

                //HINT: Check that the number of records is not greater than 5 - try removing duplicates --> student might sign in more than once on the same day. LINQ: Distinct
                var signedInDates = authenticationEvents.Count(x => calendar.GetWeekOfYear(x.CreateDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday) == weekNumber);
                const int requiredSigninDayCount = 5;

                var responseModel = new SigninProgressResponseModel();

                responseModel.CreateProgressItem("Days Attended", signedInDates);
                responseModel.CreateProgressItem("Days Required Attendance", requiredSigninDayCount);

                return responseModel;
            }
        }
    }
}
