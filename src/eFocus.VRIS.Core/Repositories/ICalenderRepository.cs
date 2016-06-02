using System.Threading.Tasks;
using eFocus.VRIS.Core.Models;

namespace eFocus.VRIS.Core.Repositories
{
    public interface ICalenderRepository
    {
        Task<AuthToken> Authorize();
    }
}
