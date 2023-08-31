namespace Merthsoft.Moose.MooseEngine.PathFinding;

public struct Duration : IComparable<Duration>, IEquatable<Duration>
{
    public static Duration Zero => new Duration(0);

    private Duration(float seconds) => Seconds = seconds;

    public float Seconds { get; }

    public static Duration FromSeconds(float seconds) => new Duration(seconds);

    public static Duration operator +(Duration a, Duration b)
        => new Duration(a.Seconds + b.Seconds);

    public static Duration operator -(Duration a, Duration b)
        => new Duration(a.Seconds - b.Seconds);

    public static bool operator >(Duration a, Duration b)
        => a.Seconds > b.Seconds;

    public static bool operator <(Duration a, Duration b)
        => a.Seconds < b.Seconds;

    public static bool operator >=(Duration a, Duration b)
        => a.Seconds >= b.Seconds;

    public static bool operator <=(Duration a, Duration b)
        => a.Seconds <= b.Seconds;

    public static bool operator ==(Duration a, Duration b)
        => a.Equals(b);

    public static bool operator !=(Duration a, Duration b)
        => !a.Equals(b);

    public override string ToString() => $"{Seconds:F2}s";

    public override bool Equals(object obj) => obj is Duration duration && Equals(duration);

    public bool Equals(Duration other) => Seconds == other.Seconds;

    public int CompareTo(Duration other) => Seconds.CompareTo(other.Seconds);

    public override int GetHashCode() => -1609761766 + Seconds.GetHashCode();
}
