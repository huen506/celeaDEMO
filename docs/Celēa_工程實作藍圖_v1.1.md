# Celēa · 瑟列亞｜工程實作藍圖

**Engineering Blueprint · 系統百科全書**

版本：v1.1　｜　日期：2026-04-24　｜　對應進程紀錄最後更新：2026-04-24

> 本文件為工程實作分支的完整技術全覽。任何人閱讀此文件，即可理解瑟列亞 Demo 前傳的完整系統架構、接口用法、資料結構與當前狀態。內容從已驗收的程式碼掃描與執行企劃整合而來，與實際 CS 檔案同步。

---

## 一、專案基本資訊

| 項目 | 內容 |
|---|---|
| Unity 版本 | Unity 6.3 LTS（6000.3.11f1） |
| 專案名稱 | Celēa |
| 專案路徑 | C:\Users\a2677\Desktop\Celēa開發\Celēa |
| 開發語言 | C# |
| 命名空間 | namespace Celea（所有腳本統一使用） |
| 腳本根目錄 | Assets/Scripts/ |
| 版本控制 | Git　｜　第一個 commit：fb1c206（2026-04-18） |
| 渲染管線 | URP 2D |

---

## 二、系統總覽

共 **22 個系統**，分四批完成，全部通過結構驗收。

| 批次 | 系統名稱 | 主類別 | 狀態 |
|---|---|---|---|
| 第一批・地基層 | 事件管理系統 | EventManager | ✅ 完整可用 |
| 第一批・地基層 | UI 基礎框架 | UIManager | ✅ 完整可用（佔位素材） |
| 第一批・地基層 | 場景管理系統 | CeleaSceneManager | ✅ 完整可用 |
| 第二批・核心玩法層 | 對話與選擇系統 | DialogueManager | ✅ 完整可用 |
| 第二批・核心玩法層 | 旗標系統 | FlagManager | ✅ 完整可用（數值佔位） |
| 第二批・核心玩法層 | 時間與場景推進系統 | TimeManager | ✅ 完整可用 |
| 第二批・核心玩法層 | 儲存系統 | SaveManager | ✅ 完整可用 |
| 第三批・體驗深化層 | 對話履歷系統 | DialogueLogManager | ✅ 完整可用（佔位素材） |
| 第三批・體驗深化層 | 隱性任務系統 | HiddenQuestManager | ✅ 骨架完整（內容待填） |
| 第三批・體驗深化層 | 佈告欄任務系統 | NoticeBoardManager | ✅ 骨架完整（內容待填） |
| 第三批・體驗深化層 | 圖鑑系統 | GalleryManager | ✅ 骨架完整（內容待填） |
| 第四批・戰鬥層 | 戰鬥核心框架 | CombatManager | ✅ 骨架完整（數值佔位） |
| 第四批・戰鬥層 | 崩鳴系統 | ResonanceManager | ✅ 骨架完整（數值待校準） |
| 第四批・戰鬥層 | 傷勢系統 | InjuryManager | ✅ 骨架完整（數值佔位） |
| 第四批・戰鬥層 | 養成系統 | GrowthManager | ✅ 骨架完整（技能內容待填） |
| 第四批・戰鬥層 | 魔核系統 | CoreModuleManager | ✅ 骨架完整（科技線專屬，內容待填） |
| 第四批・戰鬥層 | 傭兵池 | MercenaryPool | ✅ 骨架完整（角色內容待填） |
| 輔助 | 戰鬥數值整合器 | BattleStatsBuilder | ✅ 完整可用 |
| 輔助 | 傷害計算器 | CombatCalculator | ✅ 完整可用 |
| 輔助 | 技能資料庫 | PerkDatabase | ✅ 骨架完整（內容待填） |
| 輔助 | 傭兵資料庫 | MercenaryDatabase | ✅ 骨架完整（內容待填） |
| 輔助 | 場景節點 | SceneNode / SceneStateData | ✅ 完整可用 |

---

## 三、資料夾結構

```
Assets/
├── Scripts/
│   ├── Core/               EventManager、GameEvents、EventData
│   ├── UI/                 UIManager、HUDController、DialogueUI、MenuController
│   ├── Scene/              CeleaSceneManager、SceneNode、SceneStateData、PlayerController
│   ├── Dialogue/           DialogueManager、DialogueStateMachine、DialogueFlowController
│   │                       TypewriterEffect、ChoiceController、SpeakerController
│   │                       DialogueLine、DialogueChoice、DialogueSegment、DialogueData
│   ├── Flag/               FlagManager、MoralSpectrum、SituationFlags、AffinitySystem
│   ├── Time/               TimeManager、TimeData、StepCounter
│   ├── Save/               SaveManager、SaveData
│   ├── DialogueLog/        DialogueLogManager、DialogueLogEntry、DialogueLogUI
│   ├── HiddenQuest/        HiddenQuestManager、HiddenQuestData、HiddenQuestPool
│   ├── NoticeBoard/        NoticeBoardManager、NoticeBoardQuestData、NoticeBoardUI
│   ├── Gallery/            GalleryManager、GalleryEntryData、GalleryUI
│   ├── Combat/             CombatManager、CombatUnit、CombatAction、CombatCalculator
│   │                       BattleStatsBuilder、CombatUI
│   │   ├── Resonance/      ResonanceManager、ResonanceData、ResonanceConfig
│   │   └── Injury/         InjuryManager、InjuryData
│   ├── Growth/             GrowthManager、PerkData、PerkDatabase、AffinityUnlockData
│   ├── CoreModule/         CoreModuleManager、CoreModuleData、EquipmentSlotData
│   └── Mercenary/          MercenaryPool、MercenaryData、MercenaryDatabase
│
└── Resources/
    ├── Dialogues/          D001.json、D002.json（測試用）
    ├── HiddenQuests/       HiddenQuestDatabase.json、liora_pool.json（佔位）
    ├── NoticeBoard/        sielovei.json（佔位）
    ├── Gallery/            GalleryDatabase.json（佔位）
    ├── Perks/              PerkDatabase_Virtue.json、PerkDatabase_Neutral.json、PerkDatabase_Sin.json（佔位）
    └── Mercenaries/        MercenaryDatabase.json（佔位）
```

---

## 四、Singleton 分級表

| 分級 | 系統 | 說明 |
|---|---|---|
| 全域持久（Singleton + DontDestroyOnLoad） | EventManager | 全域事件匯流排 |
| 全域持久（Singleton + DontDestroyOnLoad） | UIManager | UI 層管理 |
| 全域持久（Singleton + DontDestroyOnLoad） | CeleaSceneManager | 場景與節點管理 |
| 全域持久（Singleton + DontDestroyOnLoad） | FlagManager | 旗標總接口 |
| 全域持久（Singleton + DontDestroyOnLoad） | TimeManager | 時間推進 |
| 全域持久（Singleton + DontDestroyOnLoad） | SaveManager | 存讀檔 |
| 全域持久（Singleton + DontDestroyOnLoad） | DialogueLogManager | 對話履歷 |
| 全域持久（Singleton + DontDestroyOnLoad） | HiddenQuestManager | 隱性任務 |
| 全域持久（Singleton + DontDestroyOnLoad） | NoticeBoardManager | 佈告欄任務 |
| 全域持久（Singleton + DontDestroyOnLoad） | GalleryManager | 圖鑑 |
| 全域持久（Singleton + DontDestroyOnLoad） | InjuryManager | 傷勢（跨場景持續） |
| 全域持久（Singleton + DontDestroyOnLoad） | GrowthManager | 養成（跨場景持續） |
| 全域持久（Singleton + DontDestroyOnLoad） | CoreModuleManager | 魔核（科技線才掛載） |
| 全域持久（Singleton + DontDestroyOnLoad） | MercenaryPool | 傭兵池（跨場景持續） |
| 戰鬥場景 scoped（隨場景銷毀） | CombatManager | 戰鬥 runtime |
| 戰鬥場景 scoped（隨場景銷毀） | ResonanceManager | 崩鳴 runtime |
| 戰鬥場景 scoped（隨場景銷毀） | BattleStatsBuilder | 戰鬥數值整合 |

---

## 五、事件總表（GameEvents 所有常數）

所有系統間溝通透過 EventManager.Publish / Subscribe，不直接互相呼叫。

### 第一批定義

| 事件常數 | 觸發時機 | 發送方 | 主要監聽方 |
|---|---|---|---|
| ON_COMBAT_END | 戰鬥結束 | CombatManager | ResonanceManager、InjuryManager |

### 第二批定義

| 事件常數 | 觸發時機 | 發送方 | 主要監聽方 |
|---|---|---|---|
| ON_DIALOGUE_START | 對話開始 | DialogueManager | UIManager |
| ON_CHOICE_MADE | 玩家選擇完成 | DialogueFlowController | FlagManager |
| ON_TIME_PROGRESS | 時段推進 | TimeManager | HUDController |
| ON_DAY_END | 一天結束 | TimeManager | FlagManager、各系統 |
| ON_DAY_END_PROMPT_REQUESTED | 請求顯示跨日提示 | TimeManager | UIManager |
| ON_DAY_END_DECISION | 玩家決定繼續或休息 | UIManager | TimeManager |
| ON_MAIN_EVENT | 主線事件觸發 | TimeManager | 敘事端系統 |
| ON_PLAYER_STEP | 玩家移動一步 | PlayerController | StepCounter |

### 第三批定義

| 事件常數 | 觸發時機 | 發送方 | 主要監聽方 |
|---|---|---|---|
| ON_DIALOGUE_LINE_PLAYED | 每一行對話播出 | DialogueFlowController | DialogueLogManager |
| ON_LOG_TOGGLE_REQUESTED | 玩家開關對話履歷 | DialogueLogUI | DialogueLogManager |
| ON_CHARACTER_INVITED | 角色受邀同行 | 敘事端 | HiddenQuestManager |
| ON_HIDDEN_QUEST_TRIGGERED | 隱性任務觸發 | HiddenQuestManager | DialogueLogManager |
| ON_HIDDEN_QUEST_COMPLETED | 隱性任務完成 | HiddenQuestManager | FlagManager |
| ON_HIDDEN_QUEST_FAILED | 隱性任務失敗 | HiddenQuestManager | FlagManager |
| ON_NOTICE_QUEST_ACCEPTED | 佈告欄任務接取 | NoticeBoardManager | — |
| ON_NOTICE_QUEST_STEP_REVEALED | 任務步驟顯現 | NoticeBoardManager | NoticeBoardUI |
| ON_NOTICE_QUEST_COMPLETED | 佈告欄任務完成 | NoticeBoardManager | FlagManager |
| ON_NOTICE_QUEST_FAILED | 佈告欄任務失敗 | NoticeBoardManager | FlagManager |
| ON_NOTICE_QUEST_CONSEQUENCE | 任務後果觸發 | NoticeBoardManager | 敘事端系統 |
| ON_GALLERY_UNLOCKED | CG 解鎖 | GalleryManager | GalleryUI |

### 第四批定義

| 事件常數 | 觸發時機 | 發送方 | 主要監聽方 |
|---|---|---|---|
| ON_COMBAT_START | 戰鬥開始 | CombatManager | ResonanceManager、BattleStatsBuilder |
| ON_TURN_START | 每個角色回合開始 | CombatManager | CombatUI |
| ON_TURN_END | 每個角色回合結束 | CombatManager | CombatUI |
| ON_ACTION_EXECUTED | 指令執行完成 | CombatManager | ResonanceManager、InjuryManager |
| ON_BATTLE_STATS_UPDATED | 戰鬥數值組裝完成 | BattleStatsBuilder | CombatCalculator |
| ON_RESONANCE_CHARGED | 崩鳴計量表變動 | ResonanceManager | CombatUI |
| ON_RESONANCE_TRIGGERED | 進入崩鳴狀態 | ResonanceManager | CombatUI |
| ON_RESONANCE_ENDED | 崩鳴狀態結束 | ResonanceManager | CombatUI |
| ON_INJURY_CHANGED | 傷勢狀態改變 | InjuryManager | CombatUI |
| ON_UNIT_DEFEATED | 單位倒地 | CombatManager | InjuryManager、MercenaryPool |
| ON_CHAPTER_CLEARED | 章節結算 | 敘事端 | GrowthManager |
| ON_PERK_SELECTED | 玩家選完加護技能 | GrowthManager | — |
| ON_AFFINITY_TIER_UNLOCKED | 好感度跨越門檻 | AffinitySystem | GrowthManager |
| ON_MERCENARY_RECRUITED | 傭兵招募成功 | MercenaryPool | — |
| ON_MERCENARY_DISMISSED | 傭兵離隊 | MercenaryPool | — |
| ON_MERCENARY_DIED | 傭兵永久死亡 | MercenaryPool | InjuryManager |

---

## 六、各系統接口索引

### Core

**EventManager**
```csharp
Subscribe(string eventName, Action<EventData> callback)   // 訂閱事件
Unsubscribe(string eventName, Action<EventData> callback) // 取消訂閱
Publish(string eventName, EventData data = null)          // 發送事件
```

**EventData**
```csharp
Set(string key, object value)  // 設定資料
Has(string key) → bool         // 確認鍵是否存在
```

---

### Flag

**FlagManager**（旗標總接口，查詢全部旗標狀態從這裡進）
```csharp
GetMoralTier() → int                        // 取得善惡光譜階段（-2 到 +2）
GetMoralValue() → int                       // 取得善惡光譜數值
GetAffinity(string characterId) → int       // 取得好感度數值
GetAffinityTier(string characterId) → int   // 取得好感度階段（0 到 3）
GetSituationValue(string flagId) → int      // 取得局勢旗標數值
ModifyAffinity(string characterId, int delta) // 修改好感度
CaptureState() → FlagSaveData
RestoreState(FlagSaveData saveData)
```

**MoralSpectrum**
```csharp
ApplyChoice(string payloadKey, ChoiceCategory category) // 套用選擇結果
GetTier() → int                                         // 取得當前階段
CaptureState() → MoralSpectrumSaveData
RestoreState(MoralSpectrumSaveData saveData)
```

善惡光譜門檻（已確認）：

| 數值範圍 | 階段 | GetTier() 回傳值 |
|---|---|---|
| < -60 | 惡第二 | -2 |
| -60 到 -25 | 惡第一 | -1 |
| -25 到 +25 | 中性 | 0 |
| +25 到 +60 | 善第一 | +1 |
| > +60 | 善第二 | +2 |

**SituationFlags**
```csharp
TriggerWorldEvent(string flagId, int delta)   // 世界自行發生的事件
TriggerDravenEvent(string flagId, int delta)  // 因德拉文存在才發生的事件
TriggerPlayerEvent(string flagId, int delta)  // 玩家主動挖出的事件
GetValue(string flagId) → int
EvaluateChapterEnd()                          // 章末結算，讀取傾向推入線路
CaptureState() / RestoreState()
```

**AffinitySystem**
```csharp
AdjustAffinity(string characterId, int delta) // 調整好感度
GetValue(string characterId) → int            // 取得數值
GetTier(string characterId) → int             // 取得階段（0=已知 1=熟識 2=信任 3=親密）
CaptureState() / RestoreState()
```

好感度階段對照：

| GetTier() | 階段 | 解鎖內容 |
|---|---|---|
| 0 | 已知 | 無 |
| 1 | 熟識 | 1 個德拉文特殊技能 + 能力值提升 |
| 2 | 信任 | 2 個德拉文特殊技能 + 能力值提升 |
| 3 | 親密 | 2 個德拉文特殊技能 + 連繫技 + 能力值提升 |

---

### Time

**TimeManager**
```csharp
AdvanceSlot()                     // 推進一個時段
ConsumeSlotForTravel()            // 外出移動消耗時段
TriggerMainEvent(string eventId)  // 觸發主線事件
CaptureState() / RestoreState()
```

**StepCounter**
```csharp
Reset() // 重置步數計數
```
（步數由 PlayerController 發出 ON_PLAYER_STEP，StepCounter 監聽累計）

---

### Scene

**CeleaSceneManager**
```csharp
RegisterNode(SceneNode node)                        // 註冊場景節點
GetNode(string nodeId) → SceneNode                  // 取得節點
EnterNode(string targetNodeId)                      // 進入節點（含過場）
TryFastTravel(string targetNodeId) → bool           // 嘗試快速移動
GetOrCreateStateData(string nodeId) → SceneStateData
GetStateData(string nodeId) → SceneStateData
ForceUnlockNode(string nodeId)                      // 強制解鎖節點
CaptureState() / RestoreState()
```

**SceneNode**
```csharp
Unlock()                            // 解鎖節點
AddConnection(string targetNodeId)  // 新增連結
```

**SceneStateData**
```csharp
MarkEventTriggered(string eventId)         // 標記事件已觸發
IsEventTriggered(string eventId) → bool   // 查詢事件是否已觸發
```

**PlayerController**
```csharp
MoveTo(string targetNodeId, SceneNode.NodeType nodeType) // 移動到目標節點
```

---

### Dialogue

**DialogueManager**（外部呼叫入口，不直接碰底層）
```csharp
StartDialogue(string dialogueId) // 用 ID 載入並開始對話
OnPlayerTap()                    // 玩家點擊推進對話
```

**DialogueFlowController**
```csharp
StartDialogue(DialogueData data)            // 開始對話流程
Advance()                                   // 推進對話
OnTypewriterComplete()                      // 打字機完成回調
OnChoiceSelected(DialogueChoice choice)     // 玩家選擇回調
GetCurrentLine() → DialogueLine             // 取得當前行
```

**DialogueStateMachine**
```csharp
TransitionTo(DialogueState newState) // 切換狀態
Is(DialogueState state) → bool       // 確認當前狀態
```

**TypewriterEffect**
```csharp
Play(string text)       // 開始播放打字機效果
TryInterrupt() → bool   // 嘗試中斷（回傳是否成功中斷）
```

**ChoiceController**
```csharp
ShowChoices(List<DialogueChoice> choices) // 顯示選項
Select(int index)                         // 選擇
```

**SpeakerController**
```csharp
UpdateFocus(int focusSlot) // 更新說話者亮度
ResetAll()                 // 重置所有立繪
```

---

### Combat

**CombatManager**（場景 scoped，不 DontDestroyOnLoad）
```csharp
PlayerSelectAction(CombatAction action)     // 玩家提交指令
RegisterUnit(CombatUnit unit, bool isPlayer) // 註冊戰鬥單位
GetCurrentState() → CombatState             // 取得當前狀態機狀態
```

CombatState 列舉：Idle / TurnStart / SelectAction / ExecuteAction / TurnEnd / CombatEnd

**CombatUnit**
```csharp
Initialize(string id, string name, float hp, InjuryData injury, ResonanceData resonance)
TakeDamage(float amount)  // 受傷
Heal(float amount)        // 回復
ResetTurnState()          // 回合結束重置
```

注意：CombatUnit 對 InjuryData 與 ResonanceData 只有唯讀引用（private set），不可直接修改。

**CombatAction 資料格式**
```csharp
ActionType actionType       // Attack / Skill / Ultimate / LinkSkill / SpecialAbility / Item / Guard
string sourceUnitId
List<string> targetUnitIds
string skillId
List<ActionEffect> effects  // 可為空清單
Dictionary<string, object> metadata // 可為空字典
```

**ActionEffect 資料格式**
```csharp
EffectType effectType  // Damage / Heal / ApplyBuff / ApplyDebuff / ConsumeResonance
float value
string targetUnitId
```

**BattleStatsBuilder**（場景 scoped，不 DontDestroyOnLoad）
```csharp
Build()                             // 組裝數值（戰鬥中 isLocked=true 時拒絕）
GetStats(string unitId) → BattleStats // 取得指定單位最終數值
```

組裝順序：基礎數值 → GrowthManager.GetActivePerks() → AffinitySystem.GetStatBonuses() → CoreModuleManager.GetActiveModuleEffects()

**CombatCalculator**
```csharp
CalculateDamage(CombatUnit attacker, CombatUnit target, ActionEffect effect) → float
CalculateHeal(CombatUnit source, ActionEffect effect) → float
```

---

### Resonance

**ResonanceManager**（場景 scoped，不 DontDestroyOnLoad）
```csharp
RegisterUnit(string unitId) → ResonanceData        // 戰鬥開始時註冊，初始化為 0
AddResonance(string unitId, ResonanceGainType gainType) // 累積崩鳴
ConsumeResonancePoints(string unitId, int amount)  // 消耗崩鳴點數
GetData(string unitId) → ResonanceData             // 取得崩鳴資料
BothResonating(string unitIdA, string unitIdB) → bool // 確認雙方是否同時在崩鳴狀態
```

ResonanceGainType 列舉：Hit（高效率）/ DealDamage（中效率）/ Support（低效率）

崩鳴佔位常數（待校準）：

| 常數 | 當前值 | 說明 |
|---|---|---|
| RESONANCE_MAX | 100 | 觸發閾值 |
| RESONANCE_GAIN_HIT | 15 | 受傷累積 |
| RESONANCE_GAIN_DEAL | 10 | 造成傷害累積 |
| RESONANCE_GAIN_SUPPORT | 3 | 補血/buff 累積 |

---

### Injury

**InjuryManager**（Singleton + DontDestroyOnLoad）
```csharp
ApplyInjury(string unitId)               // 推進傷勢一個階段（唯一可寫入點）
GetInjuryData(string unitId) → InjuryData
CaptureState() / RestoreState()
```

InjuryState 列舉：Normal / Injured（重傷）/ Critical（瀕死）/ Dead / Terminated

注意：InjuryData.state 無公開 setter，外部禁止直接賦值，只能透過 ApplyInjury() 修改。傭兵死亡時 InjuryManager 監聽 ON_MERCENARY_DIED，自動將該單位標記為 Terminated。

---

### Growth

**GrowthManager**（Singleton + DontDestroyOnLoad）
```csharp
GetActivePerks() → List<PerkData>                           // 取得所有已獲加護技能
GetUnlockedAbilities(string characterId) → List<AffinityUnlockData> // 取得好感度解鎖技能
CaptureState() / RestoreState()
```

**PerkDatabase**
```csharp
Load()                                          // 從 Resources 載入三個池
GetPool(PerkPoolType poolType) → List<PerkData> // 取得指定光譜池
```

PerkPoolType 列舉：Virtue（善端）/ Neutral（中性）/ Sin（惡端）

加護抽取邏輯：監聽 ON_CHAPTER_CLEARED → 讀取 MoralSpectrum.GetTier() → 從對應池隨機抽三個 → 玩家選一個 → 發出 ON_PERK_SELECTED

---

### CoreModule（科技線專屬）

**CoreModuleManager**（Singleton + DontDestroyOnLoad，非科技線不掛載）
```csharp
GetActiveModuleEffects() → List<object>         // 取得所有有效魔核效果（非科技線回傳空清單）
TryEquip(CoreModuleData module) → bool          // 嵌入魔核（超過洞位數回傳 false）
TryUnequip(string moduleId) → bool             // 拆除魔核
AcquireModule(CoreModuleData module)            // 取得魔核（戰鬥獎勵、商店呼叫此接口）
ConsumeRedModuleUsage(string moduleId)          // 消耗紅色魔核使用次數（後台，不對玩家顯示）
CaptureState() / RestoreState()
```

注意：紅色魔核使用次數 `_usageCount` 為 private，外部無 getter，只能透過 `IsActive` 布林判斷是否仍有效。

魔核三色系統：

| 顏色 | 功能 | 特殊規則 |
|---|---|---|
| 紅色 | 給技能，有使用次數 | 次數絕對不顯示給玩家；拆除後報廢 |
| 藍色 | 強化所有紅色魔核技能效果 | 全體適用，無位置限制 |
| 綠色 | 整體素質提升 | 具體效果待設計端定義 |

**EquipmentSlotData**
```csharp
TryEquip(CoreModuleData module) → bool  // 嵌入（超過洞位回傳 false）
Unequip(string moduleId) → bool        // 拆除
```

初始洞位：2 洞，佔位規則每兩章 +1，上限 6 洞。

---

### Mercenary

**MercenaryPool**（Singleton + DontDestroyOnLoad）
```csharp
GetCurrentParty() → List<MercenaryData>       // 取得當前同行傭兵
GetAvailableMercenaries() → List<MercenaryData> // 取得當前光譜下可招募的傭兵
TryRecruit(string mercenaryId) → bool         // 招募傭兵（有機率拒絕）
Dismiss(string mercenaryId)                   // 傭兵離隊
CaptureState() / RestoreState()
```

注意：REFUSAL_CHANCE = 0.3f（佔位常數，待設計端定義）。死亡傭兵永久從可用清單移除，isAlive = false 後不再出現在 GetAvailableMercenaries()。

**MercenaryDatabase**
```csharp
Load()                              // 從 Resources 載入
GetAll() → List<MercenaryData>      // 取得全部傭兵
FindById(string id) → MercenaryData // 依 ID 查詢
```

---

### Save

**SaveManager**
```csharp
Save()           // 存檔（戰鬥中不響應）
Load() → bool    // 讀檔（回傳是否成功）
DeleteSave()     // 刪除存檔
HasSave() → bool // 確認存檔是否存在
```

---

### HiddenQuest

**HiddenQuestManager**
```csharp
CompleteQuest(string questId)                  // 完成任務
ForceTrigger(string characterId)              // 強制觸發指定角色的隱性任務（測試用）
GetActiveQuests() → List<ActiveHiddenQuest>   // 取得進行中任務
CaptureState() / RestoreState()
```

---

### NoticeBoard

**NoticeBoardManager**
```csharp
UnlockQuest(string questId)                          // 解鎖任務（Unavailable → Available）
AcceptQuest(string questId)                          // 接取任務（Available → Active）
CompleteStep(string questId, int stepIndex)          // 完成任務步驟
GetQuestsByTown(string townId) → List<NoticeBoardQuestData> // 取得城鎮委託清單
GetActiveQuests() → List<NoticeBoardQuestData>       // 取得進行中任務
CaptureState() / RestoreState()
```

QuestState 列舉：Unavailable / Available / Active / Completed / Failed
StepState 列舉：Locked / Revealed / Completed

---

### Gallery

**GalleryManager**
```csharp
GetAllEntries() → List<GalleryEntryData>      // 取得全部 CG 條目
GetUnlockedEntries() → List<GalleryEntryData> // 取得已解鎖 CG
CaptureState() / RestoreState()
```

---

### DialogueLog

**DialogueLogManager**
```csharp
GetAllEntries() → List<DialogueLogEntry>     // 取得全部對話紀錄
GetMarkedEntries() → List<DialogueLogEntry>  // 取得玩家標記的對話
ToggleMark(string entryId)                   // 切換標記狀態
SetDisplayMode(LogDisplayMode mode)          // 切換一般版/沉浸版
CaptureState() / RestoreState()
```

LogDisplayMode 列舉：Normal（一般版，預設）/ Immersive（沉浸版）

---

### UI

**UIManager**
```csharp
ApplyMode(UIMode mode) // 切換 UI 模式
OpenMenu()
CloseMenu()
```

UIMode 列舉：FreeRoam / Dialogue

**HUDController**
```csharp
UpdateTimeAndLocation(string time, string date, string location)
UpdateCharacterPortrait(float hpPercent)
```

HUDController 額外功能（2026-04-24 新增）：

**選單按鈕**
- Inspector 插槽：`[SerializeField] private Button menuButton`
- 觸發方式：點擊按鈕 或 鍵盤 ESC，兩者皆可開啟選單
- 位置：右上角地圖縮圖旁
- 實作：`OpenMenuSafely()` 含 null 防護，UIManager.Instance 為 null 時 LogError 並 return

**時段換圖**
- Inspector 插槽：`timeFrameImage`（Image）+ 五個 Sprite（`spriteDawn` / `spriteNoon` / `spriteDusk` / `spriteNight` / `spriteMidnight`）
- 監聽事件：`ON_TIME_PROGRESS`，EventData key 為 `"timePeriod"`
- 對應關係：Dawn → spriteDawn、Noon → spriteNoon、Dusk → spriteDusk、Night → spriteNight、Midnight → spriteMidnight
- 注意：`spriteMidnight` 預設拖入夜晚素材，之後可獨立替換，不需改程式碼
- 美術素材：早上／中午／傍晚／晚上 四張已提供，深夜沿用晚上

**DialogueUI**
```csharp
Show() / Hide()
SetDialogue(string speaker, string body)          // 設定說話者與台詞
SetCharacterSprite(int slotIndex, Sprite sprite)  // 設定立繪（最多四個位置）
SetBackground(Sprite bg)
```

**MenuController**
```csharp
SetGameState(GameState state)
OnResumeClicked() / OnSaveClicked() / OnQuitClicked()
```

**GalleryUI**
```csharp
OpenGallery() / CloseGallery() / CloseFullscreen()
```

**NoticeBoardUI**
```csharp
OpenBoard(string townId) / CloseBoard()
OpenQuestLog() / CloseQuestLog()
```

**DialogueLogUI**
```csharp
TogglePanel() / ToggleOverlay() / CloseOverlay()
```

---

## 七、SaveData 全覽

### 欄位清單

| 欄位 | 資料來源 | 性質 |
|---|---|---|
| saveVersion | SaveManager | 版本號，不相容時清除存檔 |
| timeData | TimeManager | 持久 |
| flagData | FlagManager | 持久（含光譜、局勢旗標、好感度） |
| injuryData | InjuryManager | 持久，跨場景累積 |
| growthData | GrowthManager | 持久，已獲技能與解鎖進度 |
| coreModuleData | CoreModuleManager | 持久（科技線），插槽裝配與次數後台 |
| mercenaryData | MercenaryPool | 持久，存活與傷勢狀態 |
| hiddenQuestData | HiddenQuestManager | 持久 |
| noticeBoardData | NoticeBoardManager | 持久 |
| logData | DialogueLogManager | 持久 |
| galleryData | GalleryManager | 持久 |
| sceneData | CeleaSceneManager | 持久 |

不存入存檔的 runtime 資料：CombatManager 戰鬥流程狀態、ResonanceManager 崩鳴計量值（戰鬥 scoped，結束後清除）。

### RestoreState 固定順序

硬編碼在 SaveManager.RestoreState()，不透過事件，順序不可更改：

| 順序 | 系統 | 原因 |
|---|---|---|
| 1 | TimeManager | 最基礎，無跨系統依賴 |
| 2 | FlagManager | Quest 依賴好感度，需先還原 |
| 3 | InjuryManager | 依賴角色身分（Flag 後確定） |
| 4 | GrowthManager | 依賴好感度階段（Flag 後） |
| 5 | CoreModuleManager | 依賴線路資訊（Growth 後） |
| 6 | MercenaryPool | 依賴傷勢資料（Injury 後） |
| 7 | HiddenQuestManager | 依賴好感度（Flag 後） |
| 8 | NoticeBoardManager | 依賴時間（Time 後） |
| 9 | DialogueLogManager | 無特殊依賴 |
| 10 | GalleryManager | 無特殊依賴 |
| 11 | CeleaSceneManager | 最後還原，確保所有狀態到位 |

---

## 八、佔位常數清單

以下數值為佔位常數，待設計端校準後填入，工程端接口已預留：

| 常數 | 當前值 | 位置 | 待確認對象 |
|---|---|---|---|
| RESONANCE_MAX | 100 | ResonanceConfig.cs | 設計端校準 |
| RESONANCE_GAIN_HIT | 15 | ResonanceConfig.cs | 設計端校準 |
| RESONANCE_GAIN_DEAL | 10 | ResonanceConfig.cs | 設計端校準 |
| RESONANCE_GAIN_SUPPORT | 3 | ResonanceConfig.cs | 設計端校準 |
| 傷勢 debuff 下降幅度 | 佔位 | InjuryData.cs | 設計端定義 |
| 各傷勢自然恢復時間 | 佔位 | InjuryData.cs | 跑流程後校準 |
| 善惡光譜選項 delta | ±10 | MoralSpectrum.cs | 設計端定義 |
| SpeakerController 暗化比例 | 0.5f | SpeakerController.cs | 美術端定義 |
| 隱性任務觸發機率 | 0.3f | HiddenQuestManager.cs | 設計端定義 |
| 隱性任務好感度增減 | +1 / -1 | HiddenQuestDatabase.json | 設計端定義 |
| 傭兵拒絕機率 | 0.3f | MercenaryPool.cs | 設計端定義 |
| 魔核洞位解鎖曲線 | 2洞起每兩章+1 | CoreModuleManager.cs | 設計端定義 |

---

## 九、待填入內容清單

以下 JSON 為佔位結構，系統可讀取但內容為空，等待敘事端或設計端提供：

| 檔案 | 路徑 | 待提供方 |
|---|---|---|
| D001.json、D002.json | Resources/Dialogues/ | 現為測試用，待敘事端提供正式劇情 |
| HiddenQuestDatabase.json | Resources/HiddenQuests/ | 敘事端提供任務內容 |
| liora_pool.json | Resources/HiddenQuests/ | 敘事端提供莉歐拉任務池 |
| sielovei.json | Resources/NoticeBoard/ | 敘事端提供希洛葳委託清單 |
| GalleryDatabase.json | Resources/Gallery/ | 敘事端提供 CG 列表與 unlockEventId |
| PerkDatabase_Virtue.json | Resources/Perks/ | 設計端提供善端池五個技能 |
| PerkDatabase_Neutral.json | Resources/Perks/ | 設計端提供中性池五個技能 |
| PerkDatabase_Sin.json | Resources/Perks/ | 設計端提供惡端池五個技能 |
| MercenaryDatabase.json | Resources/Mercenaries/ | 敘事端提供 12 位傭兵角色資料 |

---

## 十、待替換的佔位素材

以下 UI 元件目前為 Unity 佔位符，等美術端定義後替換：

| 項目 | 位置 | 替換條件 |
|---|---|---|
| 過場動畫（yield return null 佔位） | CeleaSceneManager.cs TransitionToNode() | 美術端定義過場規格後 |
| SceneManager.LoadScene()（已註解） | CeleaSceneManager.cs TransitionToNode() | Unity 場景實際建立後取消註解 |
| DialogueUI 插圖區、立繪區、對話框 | DialogueUI.cs | 美術端提供素材後 |
| HUDController 四角介面 | HUDController.cs | 美術端定義字體、色彩、動畫後 |
| MenuController 離開確認對話框 | MenuController.cs | UI 規格定義後 |
| SpeakerController 暗化比例（0.5f） | SpeakerController.cs | 美術端定義後 |
| DialogueLogUI 說話者頭像 | DialogueLogUI.cs | 美術端提供頭像素材後 |
| DialogueLogUI 標色樣式 | DialogueLogUI.cs | 美術端定義隱性後果標色規格後 |
| NoticeBoardUI 步驟顯現動畫 | NoticeBoardUI.cs | 美術端定義隱形墨水動畫規格後 |
| GalleryUI 未解鎖格 | GalleryUI.cs | 美術端定義遮蔽格樣式後 |
| 崩鳴視覺（善端金色/惡端紅黑） | ResonanceManager.cs | 美術端定義後 |

---

## 十一、架構設計原則（禁止違反）

1. **事件單向原則**：所有系統透過 EventManager 溝通，不直接互相呼叫
2. **資料主權原則**：每份資料只有一個唯一來源（InjuryData = InjuryManager、ResonanceData = ResonanceManager）
3. **CombatUnit 邊界**：只持有即時戰鬥資料，對 InjuryData / ResonanceData 只有唯讀引用
4. **UI 邊界**：CombatManager 不持有任何 UI 引用，不直接呼叫 UI 方法
5. **BattleStatsBuilder 鎖定**：戰鬥中 isLocked=true，禁止重新組裝數值
6. **崩鳴初始化**：每場戰鬥從 0 開始，不繼承前一場數值
7. **紅色魔核次數封閉**：_usageCount 為 private，外部無 getter，絕對不顯示給玩家
8. **RestoreState 順序寫死**：不透過事件，不讓各系統自行決定還原時機
9. **GrowthSystem / CoreModuleSystem 邊界**：Growth = 不可移除的本質成長；CoreModule = 可替換的裝備型變化

---

*Celēa · Demo 前傳　工程實作藍圖　v1.0　2026-04-18*
