namespace BI.Core.Common.GameLog.Types
{
    public enum GameLogType
    {
        Login = 1,                              // 1. Login

        CreatePlayer = 10,                      // 10. CreatePlayer
        Init = 11,                              // 11. Init

        Episode_Start = 20,                     // 20. EpisodeStart
        Episode_End = 21,                       // 21. EpisodeEnd
        Episode_PlayUseItems = 22,              // 22. EpisodePlayUseItems

        // Hidden Enum
    }
}
