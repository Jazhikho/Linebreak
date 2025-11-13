// -----------------------------------------------------------------------------
// File Responsibility: Describes lifecycle states for scheduled events so the
// scheduler can manage execution and cancellation semantics clearly.
// Key Members: ScheduledEventStatus enum values (Pending, Executed, Cancelled).
// -----------------------------------------------------------------------------
namespace Linebreak.Core.Scheduling;

/// <summary>
/// Status of a scheduled event.
/// </summary>
public enum ScheduledEventStatus
{
    /// <summary>
    /// Event is waiting to be triggered.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Event has been executed.
    /// </summary>
    Executed = 1,

    /// <summary>
    /// Event has been cancelled.
    /// </summary>
    Cancelled = 2
}

