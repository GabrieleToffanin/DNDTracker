using DNDTracker.Domain.Campaigns;
using DNDTracker.SharedKernel.Commands;
using DNDTracker.Vocabulary.Exceptions;
using DNDTracker.Vocabulary.ValueObjects;

namespace DNDTracker.Application.UseCases.Campaigns.Tracker;

public sealed record AddMonsterToLibraryCommand(string CampaignName, MonsterStatBlock Monster) : ICommand;
public sealed record StartCombatCommand(string CampaignName, IReadOnlyCollection<CombatantState> Combatants) : ICommand;
public sealed record ReorderCombatCommand(string CampaignName, Guid CombatantId, int TargetIndex) : ICommand;
public sealed record AdvanceCombatTurnCommand(string CampaignName) : ICommand;
public sealed record UpdateCombatantHitPointsCommand(string CampaignName, Guid CombatantId, int Damage, int Healing, int TemporaryHitPointsDelta) : ICommand;
public sealed record AddCombatConditionCommand(string CampaignName, Guid CombatantId, CharacterCondition Condition) : ICommand;
public sealed record AddSessionLogCommand(string CampaignName, SessionLogEntry SessionLog) : ICommand;
public sealed record AddNpcCommand(string CampaignName, NpcResource Npc) : ICommand;
public sealed record AddLocationCommand(string CampaignName, LocationResource Location) : ICommand;
public sealed record AddQuestCommand(string CampaignName, QuestResource Quest) : ICommand;
public sealed record AddLootCommand(string CampaignName, LootResource Loot) : ICommand;
public sealed record AddCampaignMemberCommand(string CampaignName, CampaignMember Member) : ICommand;
public sealed record UpdateCharacterHitPointsCommand(string CampaignName, Guid CharacterId, int Damage, int Healing, int TemporaryHitPointsDelta) : ICommand;
public sealed record AddCharacterConditionCommand(string CampaignName, Guid CharacterId, CharacterCondition Condition) : ICommand;

public sealed class AddMonsterToLibraryCommandHandler(ICampaignRepository campaignRepository) : CampaignTrackerCommandHandlerBase(campaignRepository), ICommandHandler<AddMonsterToLibraryCommand>
{
    public Task Handle(AddMonsterToLibraryCommand request, CancellationToken cancellationToken) =>
        UpdateCampaignAsync(request.CampaignName, campaign => campaign.AddMonsterToLibrary(request.Monster), cancellationToken);
}

public sealed class StartCombatCommandHandler(ICampaignRepository campaignRepository) : CampaignTrackerCommandHandlerBase(campaignRepository), ICommandHandler<StartCombatCommand>
{
    public Task Handle(StartCombatCommand request, CancellationToken cancellationToken) =>
        UpdateCampaignAsync(request.CampaignName, campaign => campaign.StartCombat(request.Combatants), cancellationToken);
}

public sealed class ReorderCombatCommandHandler(ICampaignRepository campaignRepository) : CampaignTrackerCommandHandlerBase(campaignRepository), ICommandHandler<ReorderCombatCommand>
{
    public Task Handle(ReorderCombatCommand request, CancellationToken cancellationToken) =>
        UpdateCampaignAsync(request.CampaignName, campaign => campaign.ReorderCombat(request.CombatantId, request.TargetIndex), cancellationToken);
}

public sealed class AdvanceCombatTurnCommandHandler(ICampaignRepository campaignRepository) : CampaignTrackerCommandHandlerBase(campaignRepository), ICommandHandler<AdvanceCombatTurnCommand>
{
    public Task Handle(AdvanceCombatTurnCommand request, CancellationToken cancellationToken) =>
        UpdateCampaignAsync(request.CampaignName, campaign => campaign.AdvanceCombatTurn(), cancellationToken);
}

public sealed class UpdateCombatantHitPointsCommandHandler(ICampaignRepository campaignRepository) : CampaignTrackerCommandHandlerBase(campaignRepository), ICommandHandler<UpdateCombatantHitPointsCommand>
{
    public Task Handle(UpdateCombatantHitPointsCommand request, CancellationToken cancellationToken) =>
        UpdateCampaignAsync(request.CampaignName, campaign => campaign.ApplyCombatantHitPointDelta(request.CombatantId, request.Damage, request.Healing, request.TemporaryHitPointsDelta), cancellationToken);
}

public sealed class AddCombatConditionCommandHandler(ICampaignRepository campaignRepository) : CampaignTrackerCommandHandlerBase(campaignRepository), ICommandHandler<AddCombatConditionCommand>
{
    public Task Handle(AddCombatConditionCommand request, CancellationToken cancellationToken) =>
        UpdateCampaignAsync(request.CampaignName, campaign => campaign.AddCombatCondition(request.CombatantId, request.Condition), cancellationToken);
}

public sealed class AddSessionLogCommandHandler(ICampaignRepository campaignRepository) : CampaignTrackerCommandHandlerBase(campaignRepository), ICommandHandler<AddSessionLogCommand>
{
    public Task Handle(AddSessionLogCommand request, CancellationToken cancellationToken) =>
        UpdateCampaignAsync(request.CampaignName, campaign => campaign.AddSessionLog(request.SessionLog), cancellationToken);
}

public sealed class AddNpcCommandHandler(ICampaignRepository campaignRepository) : CampaignTrackerCommandHandlerBase(campaignRepository), ICommandHandler<AddNpcCommand>
{
    public Task Handle(AddNpcCommand request, CancellationToken cancellationToken) =>
        UpdateCampaignAsync(request.CampaignName, campaign => campaign.AddNpc(request.Npc), cancellationToken);
}

public sealed class AddLocationCommandHandler(ICampaignRepository campaignRepository) : CampaignTrackerCommandHandlerBase(campaignRepository), ICommandHandler<AddLocationCommand>
{
    public Task Handle(AddLocationCommand request, CancellationToken cancellationToken) =>
        UpdateCampaignAsync(request.CampaignName, campaign => campaign.AddLocation(request.Location), cancellationToken);
}

public sealed class AddQuestCommandHandler(ICampaignRepository campaignRepository) : CampaignTrackerCommandHandlerBase(campaignRepository), ICommandHandler<AddQuestCommand>
{
    public Task Handle(AddQuestCommand request, CancellationToken cancellationToken) =>
        UpdateCampaignAsync(request.CampaignName, campaign => campaign.AddQuest(request.Quest), cancellationToken);
}

public sealed class AddLootCommandHandler(ICampaignRepository campaignRepository) : CampaignTrackerCommandHandlerBase(campaignRepository), ICommandHandler<AddLootCommand>
{
    public Task Handle(AddLootCommand request, CancellationToken cancellationToken) =>
        UpdateCampaignAsync(request.CampaignName, campaign => campaign.AddLoot(request.Loot), cancellationToken);
}

public sealed class AddCampaignMemberCommandHandler(ICampaignRepository campaignRepository) : CampaignTrackerCommandHandlerBase(campaignRepository), ICommandHandler<AddCampaignMemberCommand>
{
    public Task Handle(AddCampaignMemberCommand request, CancellationToken cancellationToken) =>
        UpdateCampaignAsync(request.CampaignName, campaign => campaign.AddMember(request.Member), cancellationToken);
}

public sealed class UpdateCharacterHitPointsCommandHandler(ICampaignRepository campaignRepository) : CampaignTrackerCommandHandlerBase(campaignRepository), ICommandHandler<UpdateCharacterHitPointsCommand>
{
    public Task Handle(UpdateCharacterHitPointsCommand request, CancellationToken cancellationToken)
    {
        return UpdateCampaignAsync(request.CampaignName, campaign =>
        {
            var hero = campaign.Heroes.FirstOrDefault(h => h.Id.Id == request.CharacterId)
                ?? throw new CharacterNotFoundException($"Character {request.CharacterId} not found.");
            hero.ApplyHitPointDelta(request.Damage, request.Healing, request.TemporaryHitPointsDelta);
        }, cancellationToken);
    }
}

public sealed class AddCharacterConditionCommandHandler(ICampaignRepository campaignRepository) : CampaignTrackerCommandHandlerBase(campaignRepository), ICommandHandler<AddCharacterConditionCommand>
{
    public Task Handle(AddCharacterConditionCommand request, CancellationToken cancellationToken)
    {
        return UpdateCampaignAsync(request.CampaignName, campaign =>
        {
            var hero = campaign.Heroes.FirstOrDefault(h => h.Id.Id == request.CharacterId)
                ?? throw new CharacterNotFoundException($"Character {request.CharacterId} not found.");
            hero.AddCondition(request.Condition);
        }, cancellationToken);
    }
}

public abstract class CampaignTrackerCommandHandlerBase(ICampaignRepository campaignRepository)
{
    protected async Task UpdateCampaignAsync(string campaignName, Action<Campaign> updateAction, CancellationToken cancellationToken)
    {
        var campaign = await campaignRepository.GetCampaignAsync(campaignName, cancellationToken);
        if (campaign is null)
            throw new CampaignNotFoundException(campaignName);

        updateAction(campaign);
        await campaignRepository.UpdateAsync(campaign, cancellationToken);
    }
}
