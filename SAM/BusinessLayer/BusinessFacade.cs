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

namespace SAM1.BusinessLayer
{
    public class BusinessFacade
    {
        private readonly IEventLogger eventLogger;

        internal BusinessFacade()
        {
            eventLogger = new EventLogger();
        }

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

        internal LogonResponseModel AuthoriseStudent(LogonUserModel user, int studentId)
        {
            var responseModel = new LogonResponseModel();
            var hash = user.Password.GenerateHash();
            using (var db = new SAMEntities())
            {
                var authenticatedUser = db.Users.FirstOrDefault(u => u.Id == studentId && !u.IsAdmin && u.PasswordHash == hash);
                responseModel.SetUserId(authenticatedUser);
                responseModel.SetRedirectUrl(authenticatedUser);

                if (authenticatedUser == null)
                {
                    eventLogger.AddMetaData($"username: {user.Username} | Password: {user.Password}");
                    eventLogger.LogEvent(-1, CrossCuttingConcerns.EventLog.EventType.User_Authorisation, CrossCuttingConcerns.EventLog.EventSeverity.Error);

                    responseModel.SetErrorMessage("Invalid username or password");
                }
                else
                {
                    eventLogger.LogEvent(authenticatedUser.Id, CrossCuttingConcerns.EventLog.EventType.User_Authorisation, CrossCuttingConcerns.EventLog.EventSeverity.Informational);
                    eventLogger.LogEvent(authenticatedUser.Id, CrossCuttingConcerns.EventLog.EventType.User_Authentication, CrossCuttingConcerns.EventLog.EventSeverity.Informational);

                    responseModel.IsAuthorised = true;
                    //responseModel.SetUsername(authenticatedUser.Username);
                }
            }
            return responseModel;
        }

        internal string GetUserIdFromTag()
        {
            var studentNo = string.Empty;
            try
            {
                //File d = new DirectoryInfo("c:\\test.txt");
                if (File.Exists("c:\\test.txt"))
                {
                    const int tryCount = 3;
                    var counter = 0;
                    while (tryCount > counter)
                    {
                        var file = new StreamReader("c:\\test.txt");
                        if (!string.IsNullOrEmpty(file.ReadToEnd()))
                        {
                            var myfile = File.OpenText("c:\\test.txt");
                            studentNo = myfile.ReadLine().Trim();
                            myfile.Close();
                            file.Close();
                            File.Delete("c:\\test.txt");
                            break;
                        }
                        counter++;
                    }
                }
            }
            catch(Exception)
            {
                return studentNo;
            }
            return studentNo;
        }
        internal LogonResponseModel ScanTagToSignIn(LogonUserModel user)
        {
            var responseModel = new LogonResponseModel();
           
            // Read Student UID from File
            var studentNo = string.Empty;
            if (File.Exists("c:\\test.txt"))
            {
                studentNo = File.ReadAllText("c:\\test.txt");
                studentNo = studentNo.Replace(Environment.NewLine, string.Empty);
                studentNo = studentNo.Replace("\r", string.Empty);

                // Find Username in the database
                using (var db = new SAMEntities())
                {
                    //var authenthorisedUser = db.Users.FirstOrDefault(u => u.Id == (int)studentNo && !u.IsAdmin);
                    User authenthorisedUser = null;

                    // Authenticate User
                    if (authenthorisedUser == null)
                    {
                        responseModel.SetErrorMessage("Security Card error or the security card doesnt exist");
                        //eventLogger.LogEvent(authenthorisedUser.Username, CrossCuttingConcerns.EventLog.EventType.User_Authentication, EventSeverity.Error);
                    }
                    else
                    {
                        responseModel.IsAuthorised = true;
                        responseModel.SetUserId(authenthorisedUser);
                        responseModel.SetAuthenticationUrl(false, authenthorisedUser);
                    }
                    return responseModel;
                }
            }
            else
            {
                throw new Exception("SAM does not recognize your card!");
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
                responseModel.CreateProgressItem("Days Required Attendance",requiredSigninDayCount);

                return responseModel;
            }
        }

        internal LogonResponseModel AuthenticateUser(LogonUserModel user)
        {
            var responseModel = new LogonResponseModel();

            //1) Go to the db and get the userId            
            using (var db = new SAMEntities())
            {
                var authenticatedUser = db.Users.FirstOrDefault(u => u.Id == user.UserId);

                //2) Go and check the challenge inputted === the stored challenge in the db
                var isAuthenticated = string.Equals(authenticatedUser.AuthenticationCode, user.ChallengeResponse);
                responseModel.IsAuthenticated = isAuthenticated; //? Session based??

                responseModel.SetAuthenticationUrl(isAuthenticated, authenticatedUser);
                responseModel.SetUserId(authenticatedUser);

                if (!isAuthenticated)
                {
                    responseModel.SetErrorMessage("Invalid username or password");
                    eventLogger.LogEvent(authenticatedUser.Id, CrossCuttingConcerns.EventLog.EventType.User_Authentication, CrossCuttingConcerns.EventLog.EventSeverity.Error);
                }

                return responseModel;
            }
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
        internal LogonResponseModel LogOn(LogonUserModel userModel)
        {
            //Step 1 Generate hash for password
            var responseModel = new LogonResponseModel();
            var hash = userModel.Password.GenerateHash();

            //Step2 : validate against dbHash
            using (var db = new SAMEntities())
            {
                //var usr = db.Users.FirstOrDefault(u => u. == userModel.Username && u.PasswordHash == hash);
                User usr = null;
                responseModel.SetUserId(usr);
                responseModel.SetRedirectUrl(usr);
                if (usr == null)
                {
                    eventLogger.AddMetaData($"username: {userModel.Username} | Password: {userModel.Password}");
                    eventLogger.LogEvent(-1, CrossCuttingConcerns.EventLog.EventType.User_Authorisation, CrossCuttingConcerns.EventLog.EventSeverity.Error);

                    responseModel.SetErrorMessage("Invalid username or password");
                }
                else
                {
                    eventLogger.LogEvent(usr.Id, CrossCuttingConcerns.EventLog.EventType.User_Authorisation, CrossCuttingConcerns.EventLog.EventSeverity.Informational);
                    eventLogger.LogEvent(usr.Id, CrossCuttingConcerns.EventLog.EventType.User_Authentication, CrossCuttingConcerns.EventLog.EventSeverity.Informational);

                    responseModel.IsAuthorised = true;
                    //responseModel.SetUsername(usr.Username);
                }

                return responseModel;
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
    }
}
