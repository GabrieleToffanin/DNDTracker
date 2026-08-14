namespace DNDTracker.Vocabulary.Models;

public class CampaignModel
{
    public Guid Id { get; set; }
    public string CampaignName { get; set; }
    public string CampaignDescription { get; set; }
    public string CampaignImage { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
    public List<HeroModel> Heroes { get; private set; } = [];
    public List<MonsterStatBlockModel> MonsterLibrary { get; private set; } = [];
    public ActiveCombatModel? ActiveCombat { get; private set; }
    public List<SessionLogEntryModel> SessionLogs { get; private set; } = [];
    public List<CampaignTimelineEntryModel> TimelineEntries { get; private set; } = [];
    public List<NpcResourceModel> Npcs { get; private set; } = [];
    public List<LocationResourceModel> Locations { get; private set; } = [];
    public List<QuestResourceModel> Quests { get; private set; } = [];
    public List<LootResourceModel> Loot { get; private set; } = [];
    public List<CampaignMemberModel> Members { get; private set; } = [];

    public void SetActiveCombat(ActiveCombatModel? activeCombat)
    {
        ActiveCombat = activeCombat;
    }

    public void Apply(CampaignModel source)
    {
        CampaignName = source.CampaignName;
        CampaignDescription = source.CampaignDescription;
        CampaignImage = source.CampaignImage;
        IsActive = source.IsActive;
        CreatedDate = source.CreatedDate;
        UpdatedDate = source.UpdatedDate;
        DeletedDate = source.DeletedDate;

        Synchronize(Heroes, source.Heroes, hero => hero.Id, (current, update) => current.Apply(update));
        Synchronize(MonsterLibrary, source.MonsterLibrary, monster => monster.Id, (current, update) => current.Apply(update));
        Synchronize(SessionLogs, source.SessionLogs, entry => entry.Id, (current, update) => current.Apply(update));
        Synchronize(TimelineEntries, source.TimelineEntries, entry => entry.Id, (current, update) => current.Apply(update));
        Synchronize(Npcs, source.Npcs, npc => npc.Id, (current, update) => current.Apply(update));
        Synchronize(Locations, source.Locations, location => location.Id, (current, update) => current.Apply(update));
        Synchronize(Quests, source.Quests, quest => quest.Id, (current, update) => current.Apply(update));
        Synchronize(Loot, source.Loot, loot => loot.Id, (current, update) => current.Apply(update));
        Synchronize(Members, source.Members, member => member.Id, (current, update) => current.Apply(update));

        if (source.ActiveCombat is null)
            ActiveCombat = null;
        else if (ActiveCombat is null)
            ActiveCombat = source.ActiveCombat;
        else
            ActiveCombat.Apply(source.ActiveCombat);
    }

    private static void Synchronize<TEntity, TKey>(
        List<TEntity> current,
        IEnumerable<TEntity> updates,
        Func<TEntity, TKey> keySelector,
        Action<TEntity, TEntity> apply)
        where TKey : notnull
    {
        var existingById = current.ToDictionary(keySelector);
        var updateKeys = new HashSet<TKey>();

        foreach (var update in updates)
        {
            var key = keySelector(update);
            updateKeys.Add(key);

            if (existingById.TryGetValue(key, out var existing))
                apply(existing, update);
            else
                current.Add(update);
        }

        current.RemoveAll(item => !updateKeys.Contains(keySelector(item)));
    }
}
