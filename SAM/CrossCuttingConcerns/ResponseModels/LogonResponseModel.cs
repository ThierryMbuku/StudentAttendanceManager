using System;

namespace SAM1.CrossCuttingConcerns.ResponseModels
{
    public class LogonResponseModel : ResponseModel
    {
        internal string Username { get; set; }
        public bool IsAuthorised { get; set; }
        public bool IsAuthenticated { get; set; }

        internal void SetErrorMessage(string errorMessage)
        {
            base.OnSetErrorMesage(errorMessage);
        }

        internal void SetRedirectUrl(User user)
        {
            OnSetRedirectUrl((user == null || user.IsAdmin) ? "/Home/Login" : "/Student/Student");
        }
       
        internal string GetRedirectUrl()
        {
            return OnGetRedirectUrl();
        }

        internal void SetUsername(string username)
        {
            Username = username;
        }        
    }
}