namespace Shopizy.Contracts.Returns;

public record RequestReturnItemRequest(Guid OrderItemId, int Quantity);

public record RequestReturnRequest(string Reason, IReadOnlyList<RequestReturnItemRequest> Items);

public record RejectReturnRequest(string AdminNote);

public record ReturnItemDto(Guid ReturnItemId, Guid OrderItemId, int Quantity);

public record ReturnRequestDto(
    Guid ReturnRequestId,
    Guid OrderId,
    Guid UserId,
    string Reason,
    string? AdminNote,
    string Status,
    IReadOnlyList<ReturnItemDto> Items,
    DateTime CreatedOn,
    DateTime? ModifiedOn
);
