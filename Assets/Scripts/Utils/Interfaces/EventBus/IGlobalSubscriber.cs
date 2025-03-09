using System.Threading.Tasks;
using UnityEngine;

public interface IGlobalSubscriber
{
}

public interface IChangeFOVAnimation : IGlobalSubscriber
{
    void OnStartChangeFOV();
    void OnEndChangeFOV();
}

public interface IDestroySphereSegment : IGlobalSubscriber
{
    void OnDestroySphereSegment(Color segmentColor, GameObject target);
}

public interface IDestroySphere : IGlobalSubscriber
{
    void OnDestroySphere(GameObject sphere);
}

public interface IAfterDestroySphereSegment : IGlobalSubscriber
{
    void OnAfterDestroySphereSegment();
}

public interface IDestroySphereLayer : IGlobalSubscriber
{
    void OnDestroySphereLayer(bool isLayerDestroyed);
}

public interface IReleaseBall : IGlobalSubscriber
{
    Task OnReleaseBall();
}

public interface IAfterReleaseBall : IGlobalSubscriber
{
    void OnAfterReleaseBall();
}

public interface IAnimation : IGlobalSubscriber
{
    Task DoAnimation();
}

public interface IStartAnimation : IAnimation
{
}