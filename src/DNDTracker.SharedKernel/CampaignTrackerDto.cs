using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.SharedKernel;

public sealed record CampaignTrackerDto(
    string CampaignName,
    string CampaignDescription,
    IReadOnlyCollection<CharacterSheetDto> Characters,
    IReadOnlyCollection<MonsterStatBlockDto> MonsterLibrary,
    CombatState? ActiveCombat,
    IReadOnlyCollection<SessionLogEntry> SessionLogs,
    IReadOnlyCollection<CampaignTimelineEntry> TimelineEntries,
    IReadOnlyCollection<NpcResource> Npcs,
    IReadOnlyCollection<LocationResource> Locations,
    IReadOnlyCollection<QuestResource> Quests,
    IReadOnlyCollection<LootResource> Loot,
    IReadOnlyCollection<CampaignMember> Members);
