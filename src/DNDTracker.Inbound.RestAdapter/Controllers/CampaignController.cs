using DNDTracker.Application.Queries.UseCases.GetCampaign;
using DNDTracker.Application.Queries.UseCases.GetCampaignTracker;
using DNDTracker.Application.Queries.UseCases.RollDice;
using DNDTracker.Application.UseCases.Campaigns.AddHero;
using DNDTracker.Application.UseCases.Campaigns.CreateCampaign;
using DNDTracker.Application.UseCases.Campaigns.Tracker;
using DNDTracker.Domain.Heroes;
using DNDTracker.Inbound.RestAdapter.Commands;
using DNDTracker.Inbound.RestAdapter.Dtos;
using DNDTracker.SharedKernel;
using DNDTracker.Vocabulary.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DNDTracker.Inbound.RestAdapter.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignController(
    IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CampaignDto>))]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        GetAllCampaigns getAllCampaigns = new();

        IEnumerable<CampaignDto> campaigns = await mediator.Send(getAllCampaigns, cancellationToken);

        return Ok(campaigns);
    }

    [HttpGet("{campaignName}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CampaignDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(
        string campaignName,
        CancellationToken cancellationToken)
    {
        GetCampaignByName getByName = new(campaignName);

        CampaignDto campaign = await mediator.Send(getByName, cancellationToken);

        return Ok(campaign);
    }

    [HttpGet("{campaignName}/tracker")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CampaignTrackerDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTracker(
        string campaignName,
        [FromQuery] Guid? viewerUserId,
        CancellationToken cancellationToken)
    {
        var query = new GetCampaignTrackerByName(campaignName, viewerUserId);
        var tracker = await mediator.Send(query, cancellationToken);
        return Ok(tracker);
    }

    [HttpPost("{campaignName}/heroes")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(void))]
    public async Task<IActionResult> AddHero(
        string campaignName,
        [FromBody] AddHeroToCampaignRequest command,
        CancellationToken cancellationToken)
    {
        var hero = ToDomain(command.Hero);

        var mappedRequest = new AddHeroToCampaignCommand(
            campaignName,
            hero);

        await mediator.Send(mappedRequest, cancellationToken);

        return CreatedAtAction(nameof(AddHero), new { campaignName }, null);
    }

    [HttpPatch("{campaignName}/characters/{characterId:guid}/hp")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateCharacterHitPoints(
        string campaignName,
        Guid characterId,
        [FromBody] UpdateCharacterHitPointsRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(
            new UpdateCharacterHitPointsCommand(campaignName, characterId, request.Damage, request.Healing, request.TemporaryHitPointsDelta),
            cancellationToken);

        return NoContent();
    }

    [HttpPost("{campaignName}/characters/{characterId:guid}/conditions")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddCharacterCondition(
        string campaignName,
        Guid characterId,
        [FromBody] AddCharacterConditionRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(
            new AddCharacterConditionCommand(campaignName, characterId, new CharacterCondition(request.Condition, request.RemainingRounds)),
            cancellationToken);

        return NoContent();
    }

    [HttpPost("{campaignName}/monsters")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddMonsterToLibrary(
        string campaignName,
        [FromBody] AddMonsterToLibraryRequest request,
        CancellationToken cancellationToken)
    {
        var monster = new MonsterStatBlock(
            Guid.Empty,
            request.Name,
            request.CreatureType,
            request.ArmorClass,
            request.HitPoints,
            request.ChallengeRating,
            request.ExperiencePoints,
            request.InitiativeModifier,
            request.Speed,
            request.Notes);

        await mediator.Send(new AddMonsterToLibraryCommand(campaignName, monster), cancellationToken);
        return NoContent();
    }

    [HttpPost("{campaignName}/combat/start")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> StartCombat(
        string campaignName,
        [FromBody] StartCombatRequest request,
        CancellationToken cancellationToken)
    {
        var combatants = request.Combatants
            .Select(c => new CombatantState(
                c.Id,
                c.Name,
                c.Type,
                c.Initiative,
                c.CurrentHitPoints,
                c.MaxHitPoints,
                c.TemporaryHitPoints,
                c.HideHitPointsFromPlayers,
                c.Conditions ?? []))
            .ToList();

        await mediator.Send(new StartCombatCommand(campaignName, combatants), cancellationToken);
        return NoContent();
    }

    [HttpPost("{campaignName}/combat/advance")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AdvanceCombatTurn(string campaignName, CancellationToken cancellationToken)
    {
        await mediator.Send(new AdvanceCombatTurnCommand(campaignName), cancellationToken);
        return NoContent();
    }

    [HttpPatch("{campaignName}/combat/reorder")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ReorderCombat(
        string campaignName,
        [FromBody] ReorderCombatRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new ReorderCombatCommand(campaignName, request.CombatantId, request.TargetIndex), cancellationToken);
        return NoContent();
    }

    [HttpPatch("{campaignName}/combat/hp")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateCombatHitPoints(
        string campaignName,
        [FromBody] UpdateCombatHitPointsRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateCombatantHitPointsCommand(campaignName, request.CombatantId, request.Damage, request.Healing, request.TemporaryHitPointsDelta), cancellationToken);
        return NoContent();
    }

    [HttpPost("{campaignName}/combat/conditions")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddCombatCondition(
        string campaignName,
        [FromBody] AddCombatConditionRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new AddCombatConditionCommand(campaignName, request.CombatantId, new CharacterCondition(request.Condition, request.RemainingRounds)), cancellationToken);
        return NoContent();
    }

    [HttpPost("{campaignName}/sessions")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddSessionLog(
        string campaignName,
        [FromBody] AddSessionLogRequest request,
        CancellationToken cancellationToken)
    {
        var sessionLog = new SessionLogEntry(Guid.NewGuid(), request.Date, request.DurationMinutes, request.Summary, request.DungeonMasterNotes);
        await mediator.Send(new AddSessionLogCommand(campaignName, sessionLog), cancellationToken);
        return NoContent();
    }

    [HttpPost("{campaignName}/resources/npcs")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddNpc(
        string campaignName,
        [FromBody] AddNpcRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new AddNpcCommand(campaignName, new NpcResource(Guid.NewGuid(), request.Name, request.Role, request.Notes)), cancellationToken);
        return NoContent();
    }

    [HttpPost("{campaignName}/resources/locations")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddLocation(
        string campaignName,
        [FromBody] AddLocationRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new AddLocationCommand(campaignName, new LocationResource(Guid.NewGuid(), request.Name, request.Description, request.MapUrl)), cancellationToken);
        return NoContent();
    }

    [HttpPost("{campaignName}/resources/quests")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddQuest(
        string campaignName,
        [FromBody] AddQuestRequest request,
        CancellationToken cancellationToken)
    {
        var quest = new QuestResource(request.Id ?? Guid.NewGuid(), request.Title, request.Status, request.Description);
        await mediator.Send(new AddQuestCommand(campaignName, quest), cancellationToken);
        return NoContent();
    }

    [HttpPost("{campaignName}/resources/loot")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddLoot(
        string campaignName,
        [FromBody] AddLootRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new AddLootCommand(campaignName, new LootResource(Guid.NewGuid(), request.Name, request.IsMagicItem, request.Notes)), cancellationToken);
        return NoContent();
    }

    [HttpPost("{campaignName}/members")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddMember(
        string campaignName,
        [FromBody] AddCampaignMemberRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new AddCampaignMemberCommand(campaignName, new CampaignMember(request.UserId, request.DisplayName, request.Role)), cancellationToken);
        return NoContent();
    }

    [HttpPost("{campaignName}/dice/roll")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DiceRollResult))]
    public async Task<IActionResult> RollDice(
        string campaignName,
        [FromBody] RollDiceRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RollDiceInCampaign(campaignName, request.Expression, request.Modifier, request.Context), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(void))]
    public async Task<IActionResult> CreateCampaign(
        [FromBody] CreateCampaignRequest command,
        CancellationToken cancellationToken)
    {
        var mappedRequest = new CreateCampaignCommand(
            command.CampaignName,
            command.CampaignDescription,
            command.CampaignImage,
            command.CreatedDate,
            command.IsActive);

        await mediator.Send(mappedRequest, cancellationToken);

        return CreatedAtAction(nameof(CreateCampaign), new { command.CampaignName }, null);
    }

    public Hero ToDomain(HeroDto dto)
    {
        var abilityScores = dto.AbilityScores ?? DNDTracker.Vocabulary.ValueObjects.AbilityScores.Default;
        var maxHitPoints = dto.MaxHitPoints ?? dto.HitPoints;
        var currentHitPoints = dto.CurrentHitPoints ?? maxHitPoints;

        return Hero.Create(
            null,
            dto.Name,
            dto.Class,
            dto.Race,
            dto.Alignment,
            dto.Level,
            dto.Experience,
            dto.HitPoints,
            dto.HitDice,
            dto.IsNonPlayerCharacter,
            abilityScores,
            currentHitPoints,
            maxHitPoints,
            dto.TemporaryHitPoints,
            dto.ArmorClass,
            dto.Initiative,
            dto.Speed,
            dto.Notes,
            dto.Background,
            dto.Inventory,
            dto.Equipment,
            dto.Spellbook,
            dto.SpellSlots,
            dto.Conditions);
    }
}
