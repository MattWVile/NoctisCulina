using System;
using JetBrains.Annotations;

public interface IGameEventArgs { }

public abstract class BaseGameEvent : IGameEventArgs
{
    public DateTime TimeStamp { get; } = DateTime.Now;

    // Sender is virtual so that it can be overridden in derived classes
    // This is useful when you want to statically type the sender in the event
    [CanBeNull] public virtual object Sender { get; set; }
}

public class GenericGameEvent : BaseGameEvent
{
    public string Message { get; set; }
}

public class ZombossDiedEvent : BaseGameEvent
{
    public new Zomboss Sender { get; set; } // overridden Sender to specify the sender type
}
