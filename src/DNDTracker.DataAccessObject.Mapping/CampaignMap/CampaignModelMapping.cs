using DNDTracker.DataAccessObject.Mapping.HeroMap;
using DNDTracker.Domain.Campaigns;
using DNDTracker.Vocabulary.Models;

namespace DNDTracker.DataAccessObject.Mapping.CampaignMap;

public static class CampaignModelMapping
{
    public static Campaign MapToDomain(this CampaignModel campaignModel)
    {
        return Campaign.Create(
            campaignModel.Id,
            campaignModel.CampaignName,
            campaignModel.CampaignDescription,
            campaignModel.CampaignImage,
            campaignModel.CreatedDate,
            campaignModel.IsActive,
            campaignModel.Heroes.Select(h => h.MapToDomain()).ToList(),
            campaignModel.MonsterLibrary.Select(MapToDomain).ToList(),
            campaignModel.ActiveCombat?.ToValueObject(),
            campaignModel.SessionLogs.Select(entry => entry.ToValueObject()).ToList(),
            campaignModel.TimelineEntries.Select(entry => entry.ToValueObject()).ToList(),
            campaignModel.Npcs.Select(npc => npc.ToValueObject()).ToList(),
            campaignModel.Locations.Select(location => location.ToValueObject()).ToList(),
            campaignModel.Quests.Select(quest => quest.ToValueObject()).ToList(),
            campaignModel.Loot.Select(loot => loot.ToValueObject()).ToList(),
            campaignModel.Members.Select(member => member.ToValueObject()).ToList()
        );
    }

    public static CampaignModel MapToModel(this Campaign campaign)
    {
        var campaignModel = new CampaignModel
        {
            Id = campaign.Id.Id,
            CampaignName = campaign.CampaignName,
            CampaignDescription = campaign.CampaignDescription,
            CampaignImage = campaign.CampaignImage,
            IsActive = campaign.IsActive,
            CreatedDate = campaign.CreatedDate,
            UpdatedDate = campaign.UpdatedDate,
            DeletedDate = campaign.DeletedDate
        };

        campaignModel.Heroes.AddRange(campaign.Heroes.Select(h => h.MapToModel()));
        campaignModel.MonsterLibrary.AddRange(campaign.MonsterLibrary.Select(MapToModel));
        if (campaign.ActiveCombat is not null)
            campaignModel.SetActiveCombat(ActiveCombatModel.From(campaign.ActiveCombat));
        campaignModel.SessionLogs.AddRange(campaign.SessionLogs.Select(SessionLogEntryModel.From));
        campaignModel.TimelineEntries.AddRange(campaign.TimelineEntries.Select(CampaignTimelineEntryModel.From));
        campaignModel.Npcs.AddRange(campaign.Npcs.Select(NpcResourceModel.From));
        campaignModel.Locations.AddRange(campaign.Locations.Select(LocationResourceModel.From));
        campaignModel.Quests.AddRange(campaign.Quests.Select(QuestResourceModel.From));
        campaignModel.Loot.AddRange(campaign.Loot.Select(LootResourceModel.From));
        campaignModel.Members.AddRange(campaign.Members.Select(CampaignMemberModel.From));

        return campaignModel;
    }

    private static MonsterStatBlock MapToDomain(MonsterStatBlockModel monsterModel)
    {
        return MonsterStatBlock.Create(
            monsterModel.Id,
            monsterModel.Name,
            monsterModel.CreatureType,
            monsterModel.ArmorClass,
            monsterModel.HitPoints,
            monsterModel.ChallengeRating,
            monsterModel.ExperiencePoints,
            monsterModel.InitiativeModifier,
            monsterModel.Speed,
            monsterModel.Alignment,
            monsterModel.Statistics,
            monsterModel.Actions,
            monsterModel.Notes,
            monsterModel.Description,
            monsterModel.BonusActions,
            monsterModel.Reactions,
            monsterModel.LegendaryActions,
            monsterModel.LairActions,
            monsterModel.Spells);
    }

    private static MonsterStatBlockModel MapToModel(MonsterStatBlock monster)
    {
        return new MonsterStatBlockModel
        {
            Id = monster.Id,
            Name = monster.Name,
            CreatureType = monster.CreatureType,
            ArmorClass = monster.ArmorClass,
            HitPoints = monster.HitPoints,
            ChallengeRating = monster.ChallengeRating,
            ExperiencePoints = monster.ExperiencePoints,
            InitiativeModifier = monster.InitiativeModifier,
            Speed = monster.Speed,
            Alignment = monster.Alignment,
            Notes = monster.Notes,
            Description = monster.Description,
            Statistics = monster.Statistics,
            Actions = monster.Actions,
            BonusActions = monster.BonusActions,
            Reactions = monster.Reactions,
            LegendaryActions = monster.LegendaryActions,
            LairActions = monster.LairActions,
            Spells = monster.Spells
        };
    }
}
