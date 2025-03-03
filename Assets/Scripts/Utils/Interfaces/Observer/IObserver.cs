using System;

public interface IObserver
{
    void Update(ISubject subject);
}

public interface ISubject
{
    void Add(IObserver observer);

    void Remove(IObserver observer);

    void Notify();
}

public interface IEventSubject<T> : ISubject
{
    void AddEvent(Action<T> newAction);
}



public interface IAnimationObserver
{
}