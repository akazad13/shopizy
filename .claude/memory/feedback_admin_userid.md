---
name: Admin endpoint UserId from JWT
description: Never use Guid.Empty for UserId in admin commands — always use ICurrentUser
type: feedback
---

Always inject `ICurrentUser` in admin endpoints and call `currentUser.GetCurrentUserId()` when populating `UserId` in commands. Never pass `Guid.Empty`.

**Why:** `Guid.Empty` was previously used as a placeholder in admin product endpoints (`CreateProduct`, `UpdateProduct`, `DeleteProduct`). This meant the acting admin's identity was never captured, making audit trails (ModifiedBy, audit log) useless.

**How to apply:** Any admin command that carries a `UserId` for audit purposes must receive it from `ICurrentUser.GetCurrentUserId()` injected in the endpoint — not from the route, body, or a hardcoded value.
