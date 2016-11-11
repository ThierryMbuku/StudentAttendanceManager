using SAM1.Models;
using System.Data.Entity.Validation;
using System.Linq;
using SAM1.CrossCuttingConcerns.Extensions;
using SAM1.CrossCuttingConcerns.ResponseModels;
using System.Collections.Generic;
using System;
using WebGrease.Css.Extensions;

namespace SAM1.BusinessLayer
{
    public class BusinessFacade
    {
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
                            PostalCode = userModel.Address.PostalCode
                        }
                    });
                    db.SaveChanges();
                    return $"{userModel.FirstName} {userModel.LastName} Successfully registered";
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
                responseModel.SetRedirectUrl(usr);
                if (usr == null)
                {
                    responseModel.SetErrorMessage("Invalid username or password");
                }
                else
                {
                    responseModel.IsAuthorised = true;
                    responseModel.SetUsername(usr.Username);
                }

                return responseModel;
            }
        }
    }
}
