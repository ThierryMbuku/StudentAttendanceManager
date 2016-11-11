using System;

namespace SAM1.CrossCuttingConcerns.ResponseModels
{
    public class ResponseModel
    {
        private string errorMessage;
        private string redirectUrl;

        protected void OnSetErrorMesage(string message)
        {
            errorMessage = message;
        }
        protected string OnGetErrorMessage() { return errorMessage; }

        protected void OnSetRedirectUrl(string url)
        {
            redirectUrl = url;
        }
        protected string OnGetRedirectUrl() { return redirectUrl; }
    }
}