using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IGlobalSubscriber
{
}

public interface IChangeFOVAnimation : IGlobalSubscriber
{
    void OnStartChangeFOV();
    void OnEndChangeFOV();
}

public interface IDestroySphereSegmentOnHit : IGlobalSubscriber
{
    void OnDestroySphereSegmentOnHit(Color segmentColor, GameObject target, int currentShotsNumber);
}

public interface IDestroySphereSegment : IGlobalSubscriber
{
    void OnDestroySphereSegment();
}

public interface IDestroySphere : IGlobalSubscriber
{
    void OnDestroySphere(GameObject sphere);
}

public interface IAfterDestroySphereSegment : IGlobalSubscriber
{
    void OnAfterDestroySphereSegment(int currentShotsNumber);
}

public interface IDestroySphereLayer : IGlobalSubscriber
{
    void OnDestroySphereLayer(int destroyedSphereLayers);
}

public interface IReleaseBall : IGlobalSubscriber
{
    void OnReleaseBall();
}

public interface IAfterReleaseBall : IGlobalSubscriber
{
    void OnAfterReleaseBall();
}

public interface IAnimation : IGlobalSubscriber
{
    UniTask DoAnimation();
}

public interface IStartAnimation : IAnimation
{
}

public interface ISpawnSphereSegment : IGlobalSubscriber
{
    void OnSpawnSphereSegment();
}