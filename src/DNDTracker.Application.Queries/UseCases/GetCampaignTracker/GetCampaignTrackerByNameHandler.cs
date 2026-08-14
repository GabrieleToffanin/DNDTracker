using DNDTracker.Domain.Campaigns;
using DNDTracker.SharedKernel;
using DNDTracker.SharedKernel.Queries;
using DNDTracker.Vocabulary.Enums;
using DNDTracker.Vocabulary.Exceptions;

namespace DNDTracker.Application.Queries.UseCases.GetCampaignTracker;

public sealed class GetCampaignTrackerByNameHandler(ICampaignRepository campaignRepository)
    : IQueryHandler<GetCampaignTrackerByName, CampaignTrackerDto>
{
    public async Task<CampaignTrackerDto> Handle(GetCampaignTrackerByName request, CancellationToken cancellationToken)
    {
        var campaign = await campaignRepository.GetCampaignAsync(request.CampaignName, cancellationToken);
        if (campaign is null)
            throw new CampaignNotFoundException(request.CampaignName);

        var role = request.ViewerUserId is null
            ? CampaignMemberRole.DungeonMaster
            : campaign.GetRoleOrDefault(request.ViewerUserId.Value);

        var activeCombat = role == CampaignMemberRole.Player && campaign.ActiveCombat is not null
            ? campaign.ActiveCombat with
            {
                InitiativeOrder = campaign.ActiveCombat.InitiativeOrder
                    .Select(combatant => combatant.HideHitPointsFromPlayers
                        ? combatant with { CurrentHitPoints = -1, MaxHitPoints = -1, TemporaryHitPoints = 0 }
                        : combatant)
                    .ToList()
            }
            : campaign.ActiveCombat;

        var characters = campaign.Heroes
            .Select(hero => new CharacterSheetDto(
                hero.Id.Id,
                hero.Name,
                hero.Class,
                hero.Race,
                hero.Alignment,
                hero.Level,
                hero.Experience,
                hero.IsNonPlayerCharacter,
                hero.AbilityScores,
                hero.CurrentHitPoints,
                hero.MaxHitPoints,
                hero.TemporaryHitPoints,
                hero.ArmorClass,
                hero.Initiative,
                hero.Speed,
                hero.HitDice,
                hero.Inventory,
                hero.Equipment,
                hero.Spellbook,
                hero.SpellSlots,
                hero.Conditions,
                hero.Notes,
                hero.Background))
            .ToList();

        return new CampaignTrackerDto(
            campaign.CampaignName,
            campaign.CampaignDescription,
            characters,
            campaign.MonsterLibrary,
            activeCombat,
            campaign.SessionLogs,
            campaign.TimelineEntries,
            campaign.Npcs,
            campaign.Locations,
            campaign.Quests,
            campaign.Loot,
            campaign.Members);
    }
}
