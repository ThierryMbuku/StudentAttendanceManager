
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
            OnSetRedirectUrl((user != null && user.IsAdmin) ? "/Admin/Admin/Index" : "/Home/Indx");
        }

        internal string GetRedirectUrl()
        {
            return OnGetRedirectUrl();
        }

        internal void SetUsername(string username)
        {
            Username = username;
        }

        internal void SetAuthorisationUrl(bool isAuthorised, User authorisedUser)
        {
            var url = "";
            if (authorisedUser.IsAdmin)
            {
                url = isAuthorised ? "/Admin/Admin/Signin" : "/Admin/Admin/Signin";
            }
            else
            {
                url = isAuthorised ? "/Student/Student/Index" : "/Student/Student/Signin";
            }

            OnSetRedirectUrl(url);
        }
        internal void SetAuthenticationUrl(bool isAuthenticated, User user)
        {
            if (user.IsAdmin)
            {
                OnSetRedirectUrl(isAuthenticated ? "/Admin/Admin" : "/Admin/Signin");
            }
            else
            {
                OnSetRedirectUrl(isAuthenticated ? "/Student/Index" : "/Student/SignIn");
            }

        }

        internal int GetUserId()
        {
            return UserId;
        }

        internal void SetUserId(User user)
        {
            UserId = user == null ? 0 : user.Id;
        }
    }
}