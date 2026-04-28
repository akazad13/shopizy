namespace Shopizy.Contracts.V1;

/// <summary>
/// Marker placeholder reserving the <c>Shopizy.Contracts.V1</c> namespace.
/// </summary>
/// <remarks>
/// <para>
/// All current contracts in <c>Shopizy.Contracts</c> are implicitly v1 — they map to routes under
/// <c>/api/v1.0/...</c> served by ASP.NET API versioning. This placeholder exists so that any future
/// breaking change can introduce a sibling <c>Shopizy.Contracts.V2</c> namespace without retroactive
/// renames.
/// </para>
/// <para>
/// <b>Versioning rules</b> (see <c>docs/Api.md</c> for the full policy):
/// <list type="bullet">
///   <item><description>Adding optional fields → no version bump.</description></item>
///   <item><description>Removing/renaming a field, changing semantics, or tightening validation → introduce <c>V2</c> sibling.</description></item>
///   <item><description>Both versions live side-by-side until the v1 deprecation window closes.</description></item>
///   <item><description>Endpoints version their route (<c>/api/v2.0/...</c>) and bind the matching <c>V2</c> contract.</description></item>
/// </list>
/// </para>
/// </remarks>
internal static class PlaceholderV1 { }
