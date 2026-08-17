using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace DNDTracker.Blazor.Api;

public sealed class DndTrackerApi(HttpClient client)
{
    public async Task<IReadOnlyList<CampaignSummary>> GetCampaignsAsync(CancellationToken cancellationToken = default) =>
        await client.GetFromJsonAsync<List<CampaignSummary>>("api/campaign", cancellationToken) ?? [];

    public Task<CampaignTracker?> GetTrackerAsync(string campaignName, CancellationToken cancellationToken = default) =>
        client.GetFromJsonAsync<CampaignTracker>(
            $"api/campaign/{Uri.EscapeDataString(campaignName)}/tracker",
            cancellationToken);

    public async Task CreateCampaignAsync(CreateCampaignRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await client.PostAsJsonAsync("api/campaign", request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task AddMonsterAsync(
        string campaignName,
        MonsterEditorModel monster,
        CancellationToken cancellationToken = default)
    {
        using var response = await client.PostAsJsonAsync(
            $"api/campaign/{Uri.EscapeDataString(campaignName)}/monsters",
            monster,
            cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<DiceRollResult> RollDiceAsync(
        string campaignName,
        int modifier,
        string context,
        CancellationToken cancellationToken = default)
    {
        using var response = await client.PostAsJsonAsync(
            $"api/campaign/{Uri.EscapeDataString(campaignName)}/dice/roll",
            new RollDiceRequest("1d20", modifier, context),
            cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DiceRollResult>(cancellationToken)
            ?? throw new InvalidOperationException("The dice roll response was empty.");
    }
}

public sealed record CampaignSummary(string CampaignName, string CampaignDescription);

public sealed class CampaignTracker
{
    public string CampaignName { get; set; } = string.Empty;
    public string CampaignDescription { get; set; } = string.Empty;
    public List<CharacterSummary> Characters { get; set; } = [];
    public List<MonsterStatBlock> MonsterLibrary { get; set; } = [];
    public CombatSummary? ActiveCombat { get; set; }
    public List<JournalEntry> SessionLogs { get; set; } = [];
    public List<NamedResource> Npcs { get; set; } = [];
    public List<NamedResource> Locations { get; set; } = [];
    public List<NamedResource> Quests { get; set; } = [];
    public List<NamedResource> Loot { get; set; } = [];
}

public sealed class CharacterSummary
{
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; }
    public int CurrentHitPoints { get; set; }
    public int MaxHitPoints { get; set; }
    public int ArmorClass { get; set; }
}

public sealed class CombatSummary
{
    public int Round { get; set; }
    public int TurnIndex { get; set; }
    public List<CombatantSummary> InitiativeOrder { get; set; } = [];
}

public sealed class CombatantSummary
{
    public string Name { get; set; } = string.Empty;
    public int Initiative { get; set; }
    public int CurrentHitPoints { get; set; }
    public int MaxHitPoints { get; set; }
}

public sealed class JournalEntry
{
    public DateTime Date { get; set; }
    public string Summary { get; set; } = string.Empty;
}

public sealed class NamedResource
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string DisplayName => string.IsNullOrWhiteSpace(Name) ? Title : Name;
}

public sealed class MonsterStatBlock
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CreatureType { get; set; } = string.Empty;
    public int ArmorClass { get; set; }
    public int HitPoints { get; set; }
    public int ChallengeRating { get; set; }
    public int ExperiencePoints { get; set; }
    public int InitiativeModifier { get; set; }
    public int Speed { get; set; }
    public string Alignment { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Statistics { get; set; } = string.Empty;
    public string Actions { get; set; } = string.Empty;
    public string BonusActions { get; set; } = string.Empty;
    public string Reactions { get; set; } = string.Empty;
    public string LegendaryActions { get; set; } = string.Empty;
    public string LairActions { get; set; } = string.Empty;
    public string Spells { get; set; } = string.Empty;
}

public sealed class MonsterEditorModel
{
    [Required(ErrorMessage = "Il nome è obbligatorio")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Il tipo / taglia è obbligatorio")]
    public string CreatureType { get; set; } = "Bestia";

    public int ArmorClass { get; set; } = 12;
    public int HitPoints { get; set; } = 20;
    public int ChallengeRating { get; set; } = 1;
    public int ExperiencePoints { get; set; } = 200;
    public int InitiativeModifier { get; set; }
    public int Speed { get; set; } = 30;

    [Required(ErrorMessage = "L'allineamento è obbligatorio")]
    public string Alignment { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le statistiche e i tratti sono obbligatori")]
    public string Statistics { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le azioni sono obbligatorie")]
    public string Actions { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string BonusActions { get; set; } = string.Empty;
    public string Reactions { get; set; } = string.Empty;
    public string LegendaryActions { get; set; } = string.Empty;
    public string LairActions { get; set; } = string.Empty;
    public string Spells { get; set; } = string.Empty;
}

public sealed record CreateCampaignRequest(
    string CampaignName,
    string CampaignDescription,
    string CampaignImage,
    DateTime CreatedDate,
    bool IsActive);

public sealed record RollDiceRequest(string Expression, int Modifier, string Context);

public sealed class DiceRollResult
{
    public string Expression { get; set; } = string.Empty;
    public int Total { get; set; }
    public List<int> Rolls { get; set; } = [];
    public int Modifier { get; set; }
    public string? Context { get; set; }
}
