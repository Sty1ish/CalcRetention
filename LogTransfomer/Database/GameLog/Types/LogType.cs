namespace LogTransfomer.Database.GameLog.Types
{
    public enum LogType
    {
        Login = 1,                              // 1. Login

        CreatePlayer = 10,                      // 10. CreatePlayer
        Init = 11,                              // 11. Init

        Episode_Start = 20,                     // 20. EpisodeStart
        Episode_End = 21,                       // 21. EpisodeEnd
        Episode_PlayUseItems = 22,              // 22. EpisodePlayUseItems

        VendingMachine_Summon = 40,             // 40. VendingMachine - Summon

        Oortball_ExteriorSlotAttach = 50,       // 50. Oortball - ExteriorSlotAttach
        Oortball_OortballSlotAttach = 51,       // 51. Oortball - OortballSlotAttach
        Oortball_DecoExpReward = 52,            // 52. Oortball - DecoExpReward
        Oortball_ExpReward = 53,                // 53. Oortball - ExpReward
        Oortball_ExpRewardList = 54,            // 54. Oortball - ExpRewardList

        Player_ChangePlayerName = 60,           // 60. Player - Change player name
        Player_ChangeProfile = 61,              // 61. Player - Change profile
        Player_HeartRefreshCheck = 62,          // 62. Player - Heart refresh check
        Player_RecoverPlayer = 63,              // 63. Player - Recover player
        Player_RemovePlayer = 64,               // 64. Player - Remove player

        Social_RandomOortball = 70,             // 70. Social - Random Oortball

        Status_Info = 80,                       // 80. Status - Info

        Tutorial_Complete = 90,                 // 90. Tutorial - Complete
        Button_Event = 91,                      // 91. BUttonEvent

        PointData_Take = 100,                   // 100. PointData - Take

        OpenContent_Complete = 110,             // 110. OpenContent - Complete

        Attendance_Check = 120,                 // 120. Attendance - Check

        ItemGift_Reward = 130,                  // 130. ItemGift - Reward

        Binding_Apple = 140,                    // 140. Binding - Apple
        Binding_Google = 141,                   // 140. Binding - Google
        Binding_CheckApple = 142,               // 140. Binding - CheckApple
        Binding_CheckGoogle = 143,              // 140. Binding - CheckGoogle

        Shop_BuyBillingApple = 996,             // 996. ShopBuy - Billing Apple
        Shop_BuyBillingGoogle = 997,            // 997. ShopBuy - Billing Google
        Shop_BuyCommon = 998,                   // 998. ShopBuy - Common
        Shop_BuyAddMove = 999,                  // 999. ShopBuy - AddMove
        Shop_BuyDefault = 1000,                 // 1000. ShopBuy - Default
        Shop_BuyPopup = 1001,                   // 1001. ShopBuy - Popup

        ShopCustom_BuyBillingApple = 1100,      // 1100. ShopCustomBuy - Billing Apple
        ShopCustom_BuyBillingGoogle = 1101,     // 1101. ShopCustomBuy - Billing Google
        ShopCustom_Buy = 1102,                  // 1102. ShopCustomBuy - Buy
        ShopCustom_Refresh = 1103,              // 1103. ShopCustomBuy - Refresh

        PlayPass_BuyBillingApple = 1200,        // 1200. PlayPassBuy - Billing Apple
        PlayPass_BuyBillingGoogle = 1201,       // 1201. PlayPassBuy - Billing Google
        PlayPass_Buy = 1202,                    // 1202. PlayPassBuy - Buy
        PlayPass_Refresh = 1203,                // 1203. PlayPassBuy - Refresh
    }
}
