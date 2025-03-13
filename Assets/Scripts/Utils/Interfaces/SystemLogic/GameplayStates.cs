public interface IStartGame : IGlobalSubscriber
{
    public void StartGame();
}

public interface IPauseGame : IGlobalSubscriber
{
    public void PauseGame();
}

public interface IResumeGame : IGlobalSubscriber
{
    public void ResumeGame();
}

public interface IFinishGame : IGlobalSubscriber
{
    public void FinishGame();
}

public interface ILoseGame : IGlobalSubscriber
{
    public void LoseGame();
}