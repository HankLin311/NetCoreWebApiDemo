namespace Demo.WebApi.Infrastructure.Attributes
{
    /// <summary>
    /// 代表系統可以匿名登入
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class SysAllowAnonymous : Attribute
    { 
    }
}
