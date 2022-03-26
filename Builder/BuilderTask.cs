namespace Merthsoft.Moose.Builder;

public abstract class BuilderTask
{
    public int WorkLeft;
    public int WorkRequired;

    public float Percent => 1f - (float)WorkLeft / (float)WorkRequired;

    public bool Started;
    public bool Cancelled;
    public bool Completed;
    
    public BuilderUnit Owner = null!;

    public abstract string Name { get; }

    public int X;
    public int Y;

    public BuilderTask(int x, int y, int timeRequired)
    {
        WorkLeft = WorkRequired = timeRequired;
        X = x;
        Y = y;
    }

    public virtual void DoWork(int workAmount)
    {
        WorkLeft -= workAmount;
        if (WorkLeft <= 0)
            Complete();
    }
    
    public virtual void Start(BuilderUnit owner)
    {
        Owner = owner;
        Started = true;
    }

    public virtual void Complete()
    {
        Completed = true;
    }

    public virtual void Cancel()
    {
        Cancelled = true;
    }

    public Point GetCell() => new(X, Y);
}
