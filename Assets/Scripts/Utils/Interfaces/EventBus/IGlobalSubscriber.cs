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

public interface IAnimation : IGlobalSubscriber
{
    Task DoAnimation();
}

public interface IStartAnimation : IAnimation
{
}