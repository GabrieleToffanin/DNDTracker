using System.Runtime.CompilerServices;
using DNDTracker.Domain.Campaigns.DomainEvents;
using DNDTracker.Domain.Heroes;
using DNDTracker.SharedKernel.Primitives;
using DNDTracker.Vocabulary.Enums;
using DNDTracker.Vocabulary.Exceptions;
using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Domain.Campaigns;

public sealed class Campaign : AggregateRoot<CampaignId>
{
    private Campaign(
        CampaignId id,
        string campaignName,
        string campaignDescription,
        string campaignImage,
        bool isActive,
        DateTime createdDate,
        DateTime updatedDate,
        DateTime? deletedDate,
        List<Hero> heroes,
        List<MonsterStatBlock> monsterLibrary,
        CombatState? activeCombat,
        List<SessionLogEntry> sessionLogs,
        List<CampaignTimelineEntry> timelineEntries,
        List<NpcResource> npcs,
        List<LocationResource> locations,
        List<QuestResource> quests,
        List<LootResource> loot,
        List<CampaignMember> members) : base(id)
    {
        CampaignName = campaignName;
        CampaignDescription = campaignDescription;
        CampaignImage = campaignImage;
        IsActive = isActive;
        CreatedDate = DateTime.SpecifyKind(createdDate, DateTimeKind.Utc);
        UpdatedDate = DateTime.SpecifyKind(updatedDate, DateTimeKind.Utc);
        DeletedDate = deletedDate is not null ? DateTime.SpecifyKind(deletedDate.Value, DateTimeKind.Utc) : null;
        Heroes = heroes;
        MonsterLibrary = monsterLibrary;
        ActiveCombat = activeCombat;
        SessionLogs = sessionLogs;
        TimelineEntries = timelineEntries;
        Npcs = npcs;
        Locations = locations;
        Quests = quests;
        Loot = loot;
        Members = members;
    }

    public string CampaignName { get; private set; }
    public string CampaignDescription { get; private set; }
    public string CampaignImage { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime UpdatedDate { get; private set; }
    public DateTime? DeletedDate { get; private set; }

    public List<Hero> Heroes { get; private set; } = [];
    public List<MonsterStatBlock> MonsterLibrary { get; private set; } = [];
    public CombatState? ActiveCombat { get; private set; }
    public List<SessionLogEntry> SessionLogs { get; private set; } = [];
    public List<CampaignTimelineEntry> TimelineEntries { get; private set; } = [];
    public List<NpcResource> Npcs { get; private set; } = [];
    public List<LocationResource> Locations { get; private set; } = [];
    public List<QuestResource> Quests { get; private set; } = [];
    public List<LootResource> Loot { get; private set; } = [];
    public List<CampaignMember> Members { get; private set; } = [];

    public static Campaign Create(
        Guid? id,
        string campaignName,
        string campaignDescription,
        string campaignImage,
        DateTime createdDate,
        bool isActive,
        List<Hero> heroes,
        List<MonsterStatBlock>? monsterLibrary = null,
        CombatState? activeCombat = null,
        List<SessionLogEntry>? sessionLogs = null,
        List<CampaignTimelineEntry>? timelineEntries = null,
        List<NpcResource>? npcs = null,
        List<LocationResource>? locations = null,
        List<QuestResource>? quests = null,
        List<LootResource>? loot = null,
        List<CampaignMember>? members = null)
    {
        ThrowIfInvalidName(campaignName);
        ThrowIfInvalidDescription(campaignDescription);
        ThrowIfInvalidImage(campaignImage);

        var currentId = id is not null ? CampaignId.Create(id.Value) : CampaignId.Create();

        return new Campaign(
            currentId,
            campaignName,
            campaignDescription,
            campaignImage,
            isActive,
            createdDate,
            createdDate,
            null,
            heroes,
            monsterLibrary ?? [],
            activeCombat,
            sessionLogs ?? [],
            timelineEntries ?? [],
            npcs ?? [],
            locations ?? [],
            quests ?? [],
            loot ?? [],
            members ?? []);
    }

    public static Campaign Create(
        string requestCampaignName,
        string requestCampaignDescription,
        string requestCampaignImage,
        DateTime requestCreatedDate,
        bool requestIsActive)
    {
        return Create(
            null,
            requestCampaignName,
            requestCampaignDescription,
            requestCampaignImage,
            requestCreatedDate,
            requestIsActive,
            []);
    }

    public void AddHero(params Hero[] hero)
    {
        AddCharacter(hero);
    }

    public void AddCharacter(params Hero[] heroes)
    {
        ArgumentNullException.ThrowIfNull(heroes);

        Heroes.AddRange(heroes);
        UpdatedDate = DateTime.UtcNow;

        HeroAddedDomainEvent heroAddedEvent = new(Guid.NewGuid(), DateTime.UtcNow);
        AddDomainEvent(heroAddedEvent);
    }

    public void AddMonsterToLibrary(MonsterStatBlock monster)
    {
        ArgumentNullException.ThrowIfNull(monster);
        MonsterLibrary.Add(monster);
        UpdatedDate = DateTime.UtcNow;
    }

    public void StartCombat(IEnumerable<CombatantState> combatants)
    {
        ArgumentNullException.ThrowIfNull(combatants);

        var ordered = combatants
            .OrderByDescending(c => c.Initiative)
            .ThenBy(c => c.Name)
            .ToList();

        ActiveCombat = new CombatState(
            Round: ordered.Count > 0 ? 1 : 0,
            TurnIndex: 0,
            InitiativeOrder: ordered);
        UpdatedDate = DateTime.UtcNow;
    }

    public void ReorderCombat(Guid combatantId, int targetIndex)
    {
        if (ActiveCombat is null)
            throw new InvalidOperationException("Combat is not active.");

        var initiative = ActiveCombat.InitiativeOrder.ToList();
        var currentIndex = initiative.FindIndex(c => c.Id == combatantId);
        if (currentIndex < 0)
            throw new InvalidOperationException("Combatant not found.");

        targetIndex = Math.Clamp(targetIndex, 0, initiative.Count - 1);
        var combatant = initiative[currentIndex];
        initiative.RemoveAt(currentIndex);
        initiative.Insert(targetIndex, combatant);

        ActiveCombat = ActiveCombat with { InitiativeOrder = initiative };
        UpdatedDate = DateTime.UtcNow;
    }

    public void AdvanceCombatTurn()
    {
        if (ActiveCombat is null || ActiveCombat.InitiativeOrder.Count == 0)
            throw new InvalidOperationException("Combat is not active.");

        var nextTurn = ActiveCombat.TurnIndex + 1;
        var nextRound = ActiveCombat.Round;

        if (nextTurn >= ActiveCombat.InitiativeOrder.Count)
        {
            nextTurn = 0;
            nextRound++;
        }

        ActiveCombat = ActiveCombat with
        {
            Round = nextRound,
            TurnIndex = nextTurn,
            InitiativeOrder = ActiveCombat.InitiativeOrder
                .Select(c => c with
                {
                    Conditions = c.Conditions
                        .Select(condition => condition.RemainingRounds is > 0
                            ? condition with { RemainingRounds = condition.RemainingRounds - 1 }
                            : condition)
                        .Where(condition => condition.RemainingRounds is null || condition.RemainingRounds > 0)
                        .ToList()
                })
                .ToList()
        };

        UpdatedDate = DateTime.UtcNow;
    }

    public void ApplyCombatantHitPointDelta(Guid combatantId, int damage, int healing, int temporaryHitPointsDelta)
    {
        if (ActiveCombat is null)
            throw new InvalidOperationException("Combat is not active.");

        var updatedOrder = ActiveCombat.InitiativeOrder.Select(combatant =>
        {
            if (combatant.Id != combatantId)
                return combatant;

            var temporaryHitPoints = Math.Max(0, combatant.TemporaryHitPoints + temporaryHitPointsDelta);
            var remainingDamage = Math.Max(0, damage);

            if (temporaryHitPoints > 0 && remainingDamage > 0)
            {
                var absorbed = Math.Min(temporaryHitPoints, remainingDamage);
                temporaryHitPoints -= absorbed;
                remainingDamage -= absorbed;
            }

            var currentHp = Math.Max(0, combatant.CurrentHitPoints - remainingDamage);
            currentHp = Math.Min(combatant.MaxHitPoints, currentHp + Math.Max(0, healing));

            return combatant with
            {
                CurrentHitPoints = currentHp,
                TemporaryHitPoints = temporaryHitPoints
            };
        }).ToList();

        ActiveCombat = ActiveCombat with { InitiativeOrder = updatedOrder };
        UpdatedDate = DateTime.UtcNow;
    }

    public void AddCombatCondition(Guid combatantId, CharacterCondition condition)
    {
        if (ActiveCombat is null)
            throw new InvalidOperationException("Combat is not active.");

        var updatedOrder = ActiveCombat.InitiativeOrder.Select(combatant =>
        {
            if (combatant.Id != combatantId)
                return combatant;

            var updatedConditions = combatant.Conditions
                .Where(c => !c.Name.Equals(condition.Name, StringComparison.OrdinalIgnoreCase))
                .Append(condition)
                .ToList();

            return combatant with { Conditions = updatedConditions };
        }).ToList();

        ActiveCombat = ActiveCombat with { InitiativeOrder = updatedOrder };
        UpdatedDate = DateTime.UtcNow;
    }

    public void AddSessionLog(SessionLogEntry entry)
    {
        SessionLogs.Add(entry);
        TimelineEntries.Add(new CampaignTimelineEntry(Guid.NewGuid(), DateTime.UtcNow, $"Session logged: {entry.Summary}"));
        UpdatedDate = DateTime.UtcNow;
    }

    public void AddTimelineEntry(CampaignTimelineEntry entry)
    {
        TimelineEntries.Add(entry);
        UpdatedDate = DateTime.UtcNow;
    }

    public void AddNpc(NpcResource npc)
    {
        Npcs.Add(npc);
        UpdatedDate = DateTime.UtcNow;
    }

    public void AddLocation(LocationResource location)
    {
        Locations.Add(location);
        UpdatedDate = DateTime.UtcNow;
    }

    public void AddQuest(QuestResource quest)
    {
        var existing = Quests.FindIndex(q => q.Id == quest.Id);
        if (existing >= 0)
            Quests[existing] = quest;
        else
            Quests.Add(quest);

        UpdatedDate = DateTime.UtcNow;
    }

    public void AddLoot(LootResource lootItem)
    {
        Loot.Add(lootItem);
        UpdatedDate = DateTime.UtcNow;
    }

    public void AddMember(CampaignMember member)
    {
        var existing = Members.FindIndex(m => m.UserId == member.UserId);
        if (existing >= 0)
            Members[existing] = member;
        else
            Members.Add(member);

        UpdatedDate = DateTime.UtcNow;
    }

    public CampaignMemberRole GetRoleOrDefault(Guid userId)
    {
        return Members.FirstOrDefault(x => x.UserId == userId)?.Role ?? CampaignMemberRole.DungeonMaster;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowIfInvalidName(string campaignName)
    {
        if (!HasValidCampaignName(campaignName))
            throw new InvalidCampaignDataException($"Invalid campaign name. {campaignName}");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowIfInvalidDescription(string campaignDescription)
    {
        if (!HasValidCampaignDescription(campaignDescription))
            throw new InvalidCampaignDataException($"Invalid campaign description. {campaignDescription}");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowIfInvalidImage(string campaignImage)
    {
        if (!HasValidCampaignImage(campaignImage))
            throw new InvalidCampaignDataException($"Invalid campaign image. {campaignImage}");
    }

    private static bool HasValidCampaignName(string campaignName)
    {
        return !string.IsNullOrWhiteSpace(campaignName);
    }

    private static bool HasValidCampaignDescription(string campaignDescription)
    {
        return !string.IsNullOrWhiteSpace(campaignDescription);
    }

    private static bool HasValidCampaignImage(string campaignImage)
    {
        string extension = Path.GetExtension(campaignImage).ToLower();
        return extension == ".jpg" || extension == ".jpeg" || extension == ".png";
    }
}
