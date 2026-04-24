namespace Celea
{
    /// <summary>
    /// 所有事件名稱的統一定義清單。
    /// 新增事件時在此處新增常數，避免字串散落各處造成難以維護的問題。
    /// </summary>
    public static class GameEvents
    {
        // 對話 / 劇情
        public const string ON_DIALOGUE_END                 = "ON_DIALOGUE_END";
        public const string ON_DIALOGUE_START               = "ON_DIALOGUE_START";
        public const string ON_CHOICE_MADE                  = "ON_CHOICE_MADE";

        // 時間
        public const string ON_TIME_PROGRESS                = "ON_TIME_PROGRESS";
        public const string ON_DAY_END                      = "ON_DAY_END";
        public const string ON_DAY_END_PROMPT_REQUESTED     = "ON_DAY_END_PROMPT_REQUESTED";
        public const string ON_DAY_END_DECISION             = "ON_DAY_END_DECISION";
        public const string ON_MAIN_EVENT                   = "ON_MAIN_EVENT";

        // 玩家
        public const string ON_PLAYER_STEP                  = "ON_PLAYER_STEP";

        // 場景
        public const string ON_SCENE_ENTER                  = "ON_SCENE_ENTER";


        // 戰鬥
        public const string ON_COMBAT_START                 = "ON_COMBAT_START";
        public const string ON_TURN_START                   = "ON_TURN_START";
        public const string ON_TURN_END                     = "ON_TURN_END";
        public const string ON_ACTION_EXECUTED              = "ON_ACTION_EXECUTED";
        public const string ON_BATTLE_STATS_UPDATED         = "ON_BATTLE_STATS_UPDATED";
        public const string ON_RESONANCE_CHARGED            = "ON_RESONANCE_CHARGED";
        public const string ON_RESONANCE_TRIGGERED          = "ON_RESONANCE_TRIGGERED";
        public const string ON_RESONANCE_ENDED              = "ON_RESONANCE_ENDED";
        public const string ON_INJURY_CHANGED               = "ON_INJURY_CHANGED";
        public const string ON_UNIT_DEFEATED                = "ON_UNIT_DEFEATED";
        public const string ON_COMBAT_END                   = "ON_COMBAT_END";
        public const string ON_CHAPTER_CLEARED              = "ON_CHAPTER_CLEARED";
        public const string ON_PERK_SELECTED                = "ON_PERK_SELECTED";
        public const string ON_AFFINITY_TIER_UNLOCKED       = "ON_AFFINITY_TIER_UNLOCKED";
        public const string ON_MERCENARY_RECRUITED          = "ON_MERCENARY_RECRUITED";
        public const string ON_MERCENARY_DISMISSED          = "ON_MERCENARY_DISMISSED";
        public const string ON_MERCENARY_DIED               = "ON_MERCENARY_DIED";

        // 存檔
        public const string ON_SAVE_REQUESTED               = "ON_SAVE_REQUESTED";

        // UI
        public const string ON_UI_MODE_CHANGE               = "ON_UI_MODE_CHANGE";

        // 對話履歷
        public const string ON_DIALOGUE_LINE_PLAYED         = "ON_DIALOGUE_LINE_PLAYED";
        public const string ON_LOG_TOGGLE_REQUESTED         = "ON_LOG_TOGGLE_REQUESTED";

        // 角色互動
        public const string ON_CHARACTER_INVITED            = "ON_CHARACTER_INVITED";

        // 隱性任務
        public const string ON_HIDDEN_QUEST_TRIGGERED       = "ON_HIDDEN_QUEST_TRIGGERED";
        public const string ON_HIDDEN_QUEST_COMPLETED       = "ON_HIDDEN_QUEST_COMPLETED";
        public const string ON_HIDDEN_QUEST_FAILED          = "ON_HIDDEN_QUEST_FAILED";

        // 佈告欄任務
        public const string ON_NOTICE_QUEST_ACCEPTED        = "ON_NOTICE_QUEST_ACCEPTED";
        public const string ON_NOTICE_QUEST_STEP_REVEALED   = "ON_NOTICE_QUEST_STEP_REVEALED";
        public const string ON_NOTICE_QUEST_COMPLETED       = "ON_NOTICE_QUEST_COMPLETED";
        public const string ON_NOTICE_QUEST_FAILED          = "ON_NOTICE_QUEST_FAILED";
        public const string ON_NOTICE_QUEST_CONSEQUENCE     = "ON_NOTICE_QUEST_CONSEQUENCE";

        // 圖鑑
        public const string ON_GALLERY_UNLOCKED             = "ON_GALLERY_UNLOCKED";
    }
}
