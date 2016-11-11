using System;
using SAM1.Models;

namespace SAM1.CrossCuttingConcerns.ResponseModels
{
    public class LogonResponseModel : ResponseModel
    {
        internal int UserId;
        internal string Username { get; set; }
        public bool IsAuthorised { get; set; }
        public bool IsAuthenticated { get; set; }

        internal void SetErrorMessage(string errorMessage)
        {
            base.OnSetErrorMesage(errorMessage);
        }

        internal string GetErrorMessage()
        {
            return OnGetErrorMessage();
        }

        internal void SetRedirectUrl(User user)
        {
            OnSetRedirectUrl((user == null || user.IsAdmin) ? "/Home/Login" : $"/Student/Student?studentId={user.ID}");
        }
       
        internal string GetRedirectUrl()
        {
            return OnGetRedirectUrl();
        }

        internal void SetUsername(string username)
        {
            Username = username;
        }

        internal void SetAuthenticationUrl(bool isAuthenticated)
        {
            OnSetRedirectUrl(isAuthenticated ? "/Admin/Admin" : "/Home/Login");
        }

        internal int GetUserId()
        {
            return UserId;
        }

        internal void SetUserId(User user)
        {
            UserId = user == null ? 0 : user.ID;
        }
    }
}