public interface IStateObserver
{
    public void AddListener(object listener);
    public void RemoveListener(object listener);
}