using System.Threading.Tasks;

namespace FH.Cache.Core.Dashboard
{
    public interface IDashboardDispatcher
    {
        Task Dispatch(DashboardContext context);
    }
}