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
        this.CampaignName = campaignName;
        this.CampaignDescription = campaignDescription;
        this.CampaignImage = campaignImage;
        this.IsActive = isActive;
        this.CreatedDate = DateTime.SpecifyKind(createdDate, DateTimeKind.Utc);
        this.UpdatedDate = DateTime.SpecifyKind(updatedDate, DateTimeKind.Utc);
        this.DeletedDate = deletedDate is not null ? DateTime.SpecifyKind(deletedDate.Value, DateTimeKind.Utc) : null;
        this.Heroes = heroes;
        this.MonsterLibrary = monsterLibrary;
        this.ActiveCombat = activeCombat;
        this.SessionLogs = sessionLogs;
        this.TimelineEntries = timelineEntries;
        this.Npcs = npcs;
        this.Locations = locations;
        this.Quests = quests;
        this.Loot = loot;
        this.Members = members;
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
        ArgumentNullException.ThrowIfNull(hero);

        this.Heroes.AddRange(hero);

        HeroAddedDomainEvent heroAddedEvent = new(Guid.NewGuid(), DateTime.UtcNow);
        this.AddDomainEvent(heroAddedEvent);
    }

    public void AddMonsterToLibrary(MonsterStatBlock monster)
    {
        ArgumentNullException.ThrowIfNull(monster);
        monster.EnsureIdentity();
        this.MonsterLibrary.Add(monster);
        this.UpdatedDate = DateTime.UtcNow;
    }

    public void StartCombat(IEnumerable<CombatantState> combatants)
    {
        ArgumentNullException.ThrowIfNull(combatants);

        var ordered = combatants
            .OrderByDescending(c => c.Initiative)
            .ThenBy(c => c.Name)
            .ToList();

        this.ActiveCombat = new CombatState(
            Round: ordered.Count > 0 ? 1 : 0,
            TurnIndex: 0,
            InitiativeOrder: ordered);
        this.UpdatedDate = DateTime.UtcNow;
    }

    public void ReorderCombat(Guid combatantId, int targetIndex)
    {
        if (this.ActiveCombat is null)
            throw new InvalidOperationException("Combat is not active.");

        var initiative = this.ActiveCombat.InitiativeOrder.ToList();
        int currentIndex = initiative.FindIndex(c => c.Id == combatantId);
        if (currentIndex < 0)
            throw new InvalidOperationException("Combatant not found.");

        targetIndex = Math.Clamp(targetIndex, 0, initiative.Count - 1);
        var combatant = initiative[currentIndex];
        initiative.RemoveAt(currentIndex);
        initiative.Insert(targetIndex, combatant);

        this.ActiveCombat = this.ActiveCombat with { InitiativeOrder = initiative };
        this.UpdatedDate = DateTime.UtcNow;
    }

    public void AdvanceCombatTurn()
    {
        if (this.ActiveCombat is null || this.ActiveCombat.InitiativeOrder.Count == 0)
            throw new InvalidOperationException("Combat is not active.");

        int nextTurn = this.ActiveCombat.TurnIndex + 1;
        int nextRound = this.ActiveCombat.Round;

        if (nextTurn >= this.ActiveCombat.InitiativeOrder.Count)
        {
            nextTurn = 0;
            nextRound++;
        }

        this.ActiveCombat = this.ActiveCombat with
        {
            Round = nextRound,
            TurnIndex = nextTurn,
            InitiativeOrder = this.ActiveCombat.InitiativeOrder
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

        this.UpdatedDate = DateTime.UtcNow;
    }

    public void ApplyCombatantHitPointDelta(Guid combatantId, int damage, int healing, int temporaryHitPointsDelta)
    {
        if (this.ActiveCombat is null)
            throw new InvalidOperationException("Combat is not active.");

        if (damage < 0 || healing < 0)
            throw new ArgumentOutOfRangeException(nameof(damage), "Damage and healing must be non-negative.");

        var updatedOrder = this.ActiveCombat.InitiativeOrder.Select(combatant =>
        {
            if (combatant.Id != combatantId)
                return combatant;

            int temporaryHitPoints = Math.Max(0, combatant.TemporaryHitPoints + temporaryHitPointsDelta);
            int remainingDamage = damage;

            if (temporaryHitPoints > 0 && remainingDamage > 0)
            {
                int absorbed = Math.Min(temporaryHitPoints, remainingDamage);
                temporaryHitPoints -= absorbed;
                remainingDamage -= absorbed;
            }

            int currentHp = Math.Max(0, combatant.CurrentHitPoints - remainingDamage);
            currentHp = Math.Min(combatant.MaxHitPoints, currentHp + healing);

            return combatant with
            {
                CurrentHitPoints = currentHp,
                TemporaryHitPoints = temporaryHitPoints
            };
        }).ToList();

        this.ActiveCombat = this.ActiveCombat with { InitiativeOrder = updatedOrder };
        this.UpdatedDate = DateTime.UtcNow;
    }

    public void AddCombatCondition(Guid combatantId, CharacterCondition condition)
    {
        if (this.ActiveCombat is null)
            throw new InvalidOperationException("Combat is not active.");

        var updatedOrder = this.ActiveCombat.InitiativeOrder.Select(combatant =>
        {
            if (combatant.Id != combatantId)
                return combatant;

            var updatedConditions = combatant.Conditions
                .Where(c => !c.Name.Equals(condition.Name, StringComparison.OrdinalIgnoreCase))
                .Append(condition)
                .ToList();

            return combatant with { Conditions = updatedConditions };
        }).ToList();

        this.ActiveCombat = this.ActiveCombat with { InitiativeOrder = updatedOrder };
        this.UpdatedDate = DateTime.UtcNow;
    }

    public void AddSessionLog(SessionLogEntry entry)
    {
        this.SessionLogs.Add(entry);
        this.TimelineEntries.Add(new CampaignTimelineEntry(Guid.NewGuid(), DateTime.UtcNow, $"Session logged: {entry.Summary}"));
        this.UpdatedDate = DateTime.UtcNow;
    }

    public void AddTimelineEntry(CampaignTimelineEntry entry)
    {
        this.TimelineEntries.Add(entry);
        this.UpdatedDate = DateTime.UtcNow;
    }

    public void AddNpc(NpcResource npc)
    {
        this.Npcs.Add(npc);
        this.UpdatedDate = DateTime.UtcNow;
    }

    public void AddLocation(LocationResource location)
    {
        this.Locations.Add(location);
        this.UpdatedDate = DateTime.UtcNow;
    }

    public void AddQuest(QuestResource quest)
    {
        int existing = this.Quests.FindIndex(q => q.Id == quest.Id);
        if (existing >= 0)
            this.Quests[existing] = quest;
        else
            this.Quests.Add(quest);

        this.UpdatedDate = DateTime.UtcNow;
    }

    public void AddLoot(LootResource lootItem)
    {
        this.Loot.Add(lootItem);
        this.UpdatedDate = DateTime.UtcNow;
    }

    public void AddMember(CampaignMember member)
    {
        int existing = this.Members.FindIndex(m => m.UserId == member.UserId);
        if (existing >= 0)
            this.Members[existing] = member;
        else
            this.Members.Add(member);

        this.UpdatedDate = DateTime.UtcNow;
    }

    public CampaignMemberRole GetRoleOrDefault(Guid userId)
    {
        return this.Members.FirstOrDefault(x => x.UserId == userId)?.Role ?? CampaignMemberRole.Player;
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
