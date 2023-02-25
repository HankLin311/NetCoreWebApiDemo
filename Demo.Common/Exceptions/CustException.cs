using Demo.Common.ConstVariables;

namespace Demo.Common.Exceptions
{
    /// <summary>
    /// 自訂錯誤訊息
    /// </summary>
    public class CustException : Exception
    {
        public string Status { get; set; } = ApiResultStatus.CUST_ERROR;

        public CustException(string? message) : base(message)
        {

        }

        public CustException(string status, string? message) : base(message)
        {
            Status = status;
        }
    }
}
