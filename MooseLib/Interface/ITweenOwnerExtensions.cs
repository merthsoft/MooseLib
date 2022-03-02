using MonoGame.Extended.Tweening;
using System.Linq.Expressions;

namespace Merthsoft.Moose.MooseEngine.Interface;
public static class ITweenOwnerExtensions
{
    public static Tween AddTween<TTarget, TMember>(
        this TTarget target,
        Expression<Func<TTarget, TMember>> expression,
        TMember toValue,
        float duration,
        float delay = 0f,
        Action<Tween>? onEnd = null,
        Action<Tween>? onBegin = null,
        int repeatCount = 0,
        float repeatDelay = 0f,
        bool autoReverse = false,
        Func<float, float>? easingFunction = null) 
        where TTarget : class, ITweenOwner
        where TMember : struct
    => target.ActiveTweens.AddItem(MooseGame.Instance.Tween(target, expression,
        toValue, duration, delay, onEnd, onBegin, repeatCount, repeatDelay,
        autoReverse, easingFunction));

    public static void ClearCompletedTweens(this ITweenOwner tweenOwner)
        => tweenOwner.ActiveTweens.RemoveAll(t => !t.IsAlive);

    public static void ClearTweens(this ITweenOwner tweenOwner, bool complete = false)
    {
        foreach (var tween in tweenOwner.ActiveTweens.Where(t => t.IsAlive))
        {
            if (complete)
                tween.CancelAndComplete();
            else
                tween.Cancel();
        }

        tweenOwner.ActiveTweens.Clear();
    }
}
