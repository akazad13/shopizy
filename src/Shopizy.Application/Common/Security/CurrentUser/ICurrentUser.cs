using Shopizy.SharedKernel.Application.Messaging;
namespace Shopizy.Application.Common.Security.CurrentUser;

public interface ICurrentUser
{
    Guid GetCurrentUserId();
}

