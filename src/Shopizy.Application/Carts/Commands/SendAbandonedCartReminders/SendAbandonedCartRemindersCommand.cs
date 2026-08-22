using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Carts.Commands.SendAbandonedCartReminders;

/// <summary>
/// Command to find inactive carts with items and dispatch recovery reminder emails.
/// </summary>
/// <param name="InactiveBeforeUtc">Carts with no activity since this time will be considered abandoned.</param>
/// <param name="MaxBatchSize">Maximum number of carts to process in one execution.</param>
public sealed record SendAbandonedCartRemindersCommand(
    DateTime InactiveBeforeUtc,
    int MaxBatchSize = 50
) : ICommand<ErrorOr<int>>;
