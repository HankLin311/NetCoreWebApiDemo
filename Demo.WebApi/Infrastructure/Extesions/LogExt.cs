namespace Microsoft.Extensions.Logging
{
    public static class LogExt
    {
        public static void WriteError(this ILogger logger,
            string userEmail, string path, string errorMsg)
        {
            string result =
                $" \n" +
                $"user     : {userEmail} \n" +
                $"path     : {path} \n" +
                $"errorMsg : {errorMsg} \n" +
                $"========================================================== \n";

            logger.LogError(result);
        }

        public static void WriteCritical(this ILogger logger,
            string path, string MethodTrace, string erroMsg)
        {
            string result =
                $" \n" +
                $"path            : {path} \n" +
                $"MethodTrace     : {MethodTrace} \n" +
                $"erroMsg         : \n" +
                $"{erroMsg} \n" +
                $"========================================================== \n";


            logger.LogCritical(result);
        }
    }
}
