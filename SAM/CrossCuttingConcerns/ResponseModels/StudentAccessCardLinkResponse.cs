
namespace SAM1.CrossCuttingConcerns.ResponseModels
{
    public class StudentAccessCardLinkResponse : ResponseModel
    {
        private int userId;

        public bool WasOperationSuccessful { get; internal set; }

        internal void SetErrorMessage(string errorMessage)
        {
            OnSetErrorMesage(errorMessage);
        }

        internal string GetErrorMessage()
        {
            return OnGetErrorMessage();
        }

        internal void SetUserId(int studentId)
        {
            userId = studentId;
        }

        internal int GetUserId()
        {
            return userId;
        }
    }
}