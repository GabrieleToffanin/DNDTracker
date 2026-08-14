using DNDTracker.DataAccessObject.Mapping.HeroMap;
using DNDTracker.DataAccessObject.Mapping.Json;
using DNDTracker.Domain.Campaigns;
using DNDTracker.Vocabulary.ValueObjects;
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
            JsonCollectionMapper.DeserializeCollection<MonsterStatBlock>(campaignModel.MonsterLibraryJson).ToList(),
            JsonCollectionMapper.Deserialize<CombatState>(campaignModel.ActiveCombatJson),
            JsonCollectionMapper.DeserializeCollection<SessionLogEntry>(campaignModel.SessionLogsJson).ToList(),
            JsonCollectionMapper.DeserializeCollection<CampaignTimelineEntry>(campaignModel.TimelineEntriesJson).ToList(),
            JsonCollectionMapper.DeserializeCollection<NpcResource>(campaignModel.NpcsJson).ToList(),
            JsonCollectionMapper.DeserializeCollection<LocationResource>(campaignModel.LocationsJson).ToList(),
            JsonCollectionMapper.DeserializeCollection<QuestResource>(campaignModel.QuestsJson).ToList(),
            JsonCollectionMapper.DeserializeCollection<LootResource>(campaignModel.LootJson).ToList(),
            JsonCollectionMapper.DeserializeCollection<CampaignMember>(campaignModel.MembersJson).ToList()
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
            DeletedDate = campaign.DeletedDate,
            MonsterLibraryJson = JsonCollectionMapper.Serialize(campaign.MonsterLibrary),
            ActiveCombatJson = campaign.ActiveCombat is null ? null : JsonCollectionMapper.SerializeObject(campaign.ActiveCombat),
            SessionLogsJson = JsonCollectionMapper.Serialize(campaign.SessionLogs),
            TimelineEntriesJson = JsonCollectionMapper.Serialize(campaign.TimelineEntries),
            NpcsJson = JsonCollectionMapper.Serialize(campaign.Npcs),
            LocationsJson = JsonCollectionMapper.Serialize(campaign.Locations),
            QuestsJson = JsonCollectionMapper.Serialize(campaign.Quests),
            LootJson = JsonCollectionMapper.Serialize(campaign.Loot),
            MembersJson = JsonCollectionMapper.Serialize(campaign.Members)
        };

        campaignModel.Heroes.AddRange(campaign.Heroes.Select(h => h.MapToModel()));

        return campaignModel;
    }
}
