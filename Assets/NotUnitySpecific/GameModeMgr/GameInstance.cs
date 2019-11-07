namespace GameModeMgr
{
    public interface IGameInstance
    {
        void Start();
        bool Loop(float frameSecs);
    }

    public class GameInstanceData
    {

    }

}