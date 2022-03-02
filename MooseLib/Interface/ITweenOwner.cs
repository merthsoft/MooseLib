using MonoGame.Extended.Tweening;

namespace Merthsoft.Moose.MooseEngine.Interface;

public interface ITweenOwner
{
    List<Tween> ActiveTweens { get; }
}
