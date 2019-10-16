namespace FH.Cache.Core.Dashboard
{
    public interface IDashboardAuthorizationFilter
    {
        bool Authorize(DashboardContext context);
    }
}