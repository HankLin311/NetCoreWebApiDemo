namespace System
{
    public static class ExceptionExt
    {
        /// <summary>
        /// 找出最底層的 Exception
        /// </summary>
        public static Exception GetInnerException(this Exception exception)
        {
            if (exception.InnerException == null)
            {
                return exception;
            }

            return exception.InnerException.GetInnerException();
        }
    }
}
