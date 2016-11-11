using SAM1.Models;
using System.Data.Entity.Validation;
using System.Linq;
using SAM1.CrossCuttingConcerns.Extensions;
using SAM1.CrossCuttingConcerns.ResponseModels;
using System.Collections.Generic;
using SAM1.CrossCuttingConcerns.EventLog;
using System;

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
                    db.Users.Add(new User
                    {
                        Username = userModel.Username,
                        FirstName = userModel.FirstName,
                        LastName = userModel.LastName,
                        Email = userModel.Email,
                        CellPhone = userModel.CellPhone,
                        PasswordHash = userModel.Password.GenerateHash(),
                        IsAdmin = userModel.IsAdmin,
                        Address = new Address
                        {
                            ComplexNumber = int.Parse(userModel.Address.ComplexNumber),
                            Street = userModel.Address.Street,
                            Suburb = userModel.Address.Suburb,
                            City = userModel.Address.City,
                            PostalCode = userModel.Address.PostalCode,
                        }
                    });
                    db.SaveChanges();
                    eventLogger.LogEvent(userModel.AdminUserId, CrossCuttingConcerns.EventLog.EventType.Admin_Register_Student, EventSeverity.Informational);
                    return $"{userModel.FirstName} {userModel.LastName} Successfully registered";
                }
                catch (DbEntityValidationException ex)
                {
                    var errorMessages = ex.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.ErrorMessage + " " + x.PropertyName);

                    var fullErrorMessage = string.Join(" => ", errorMessages);
                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                    eventLogger.LogEvent(userModel.AdminUserId, CrossCuttingConcerns.EventLog.EventType.Admin_Register_Student, EventSeverity.Error);
                    throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                }
            }
        }

        internal LogonResponseModel AuthenticateUser(LogonUserModel user)
        {
            var responseModel = new LogonResponseModel();

            //1) Go to the db and get the userId            
            using (var db = new SAMEntities())
            {
                var authenticatedUser = db.Users.FirstOrDefault(u => u.ID == user.UserId);

                //2) Go and check the challenge inputted === the stored challenge in the db
                var isAuthenticated = string.Equals(authenticatedUser.SecurityChallenge, user.ChallengeResponse);
                responseModel.IsAuthenticated = isAuthenticated; //? Session based??

                responseModel.SetAuthenticationUrl(isAuthenticated);

                if (!isAuthenticated)
                {
                    responseModel.SetErrorMessage("Invalid username or password");
                    eventLogger.LogEvent(authenticatedUser.ID, CrossCuttingConcerns.EventLog.EventType.User_Authentication, EventSeverity.Error);
                }

                return responseModel;
            }
        }

        // List of students
        //            if (string.IsNullOrEmpty(studentid))
        //{
        //}
        //else
        //{
        //}
        internal List<UserModel> StudentList(string studentid = "")
        {
            var modeluser = new List<UserModel>();
            using (var db = new SAMEntities())
            {
                var users = db.Users.ToList();

                users.ForEach(user =>
                {

                    modeluser.Add(new UserModel
                    {
                        Username = user.Username.ToString(),
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        CellPhone = user.CellPhone,
                    });
                });
            }
            return modeluser;
        }
        //Individual records/Details
        //internal UserModel StudentDetail()
        //{

        //    var modeluser = new UserModel();
        //    using (var db = new SAMEntities())
        //    {
        //        var users = db.Users;

        //        users.(user =>
        //        {

        //            modeluser.Add(new UserModel
        //            {
        //                Username = user.Username.ToString(),
        //                FirstName = user.FirstName,
        //                LastName = user.LastName,
        //                Email = user.Email,
        //                CellPhone = user.CellPhone,
        //            });
        //        });
        //    }
        //    return modeluser;
        //}
        internal LogonResponseModel LogOn(LogonUserModel userModel)
        {
            //Step 1 Generate hash for password
            var responseModel = new LogonResponseModel();
            var hash = userModel.Password.GenerateHash();

            //Step2 : validate against dbHash
            using (var db = new SAMEntities())
            {
                var usr = db.Users.FirstOrDefault(u => u.Username == userModel.Username && u.PasswordHash == hash);
                responseModel.SetUserId(usr);
                responseModel.SetRedirectUrl(usr);
                if (usr == null)
                {
                    eventLogger.AddMetaData($"username: {userModel.Username} | Password: {userModel.Password}");
                    eventLogger.LogEvent(-1, CrossCuttingConcerns.EventLog.EventType.User_Authorisation, EventSeverity.Error);

                    responseModel.SetErrorMessage("Invalid username or password");
                }
                else
                {
                    eventLogger.LogEvent(usr.ID, CrossCuttingConcerns.EventLog.EventType.User_Authorisation, EventSeverity.Informational);
                    eventLogger.LogEvent(usr.ID, CrossCuttingConcerns.EventLog.EventType.User_Authentication, EventSeverity.Informational);

                    responseModel.IsAuthorised = true;
                    responseModel.SetUsername(usr.Username);
                }

                return responseModel;
            }
        }

        internal StudentModel FindStudent(int studentId)
        {
            using (var db = new SAMEntities())
            {
                var dbStudent = db.Users.FirstOrDefault(x => x.ID == studentId && !x.IsAdmin);

                eventLogger.LogEvent(studentId, CrossCuttingConcerns.EventLog.EventType.User_Find_Student, EventSeverity.Informational);

                return dbStudent == null ? null : new StudentModel
                {
                    FirstName = dbStudent.FirstName,
                    CellPhone = dbStudent.CellPhone,
                    LastName = dbStudent.LastName,
                    Email = dbStudent.Email,
                    Username = dbStudent.Username,
                };
            }
        }
    }
}
