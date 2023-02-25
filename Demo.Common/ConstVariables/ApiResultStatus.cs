namespace Demo.Common.ConstVariables
{
    /// <summary>
    /// 系統狀態碼
    /// </summary>
    public struct ApiResultStatus
    {
        // 成功 
        public const string SUCCESS = "000";

        // 自訂邏輯錯誤
        public const string CUST_ERROR = "001";

        // 系統或 Middleware 錯誤
        public const string SYS_ERROR = "999";
    }
}
