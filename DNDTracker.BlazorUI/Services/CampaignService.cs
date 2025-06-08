using System.Text;
using System.Text.Json;
using DNDTracker.SharedKernel;
using DNDTracker.Domain.Heroes;
using DNDTracker.Vocabulary.Enums;

namespace DNDTracker.BlazorUI.Services;

public class CampaignService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public CampaignService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<CampaignDto?> GetCampaignAsync(string campaignName)
    {
        var response = await _httpClient.GetAsync($"campaign?campaignName={campaignName}");
        
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CampaignDto>(json, _jsonOptions);
        }
        
        return null;
    }

    public async Task<bool> CreateCampaignAsync(CreateCampaignRequest request)
    {
        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync("campaign", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> AddHeroToCampaignAsync(string campaignName, HeroRequest hero)
    {
        var request = new AddHeroRequest(hero);
        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync($"campaign/{campaignName}/heroes", content);
        return response.IsSuccessStatusCode;
    }
}

public record CreateCampaignRequest(
    string CampaignName,
    string CampaignDescription,
    string CampaignImage,
    DateTime CreatedDate,
    bool IsActive = true);

public record AddHeroRequest(HeroRequest Hero);

public record HeroRequest(
    string Name,
    HeroClass Class,
    Race Race,
    Alignment Alignment,
    int Level,
    int Experience,
    int HitPoints,
    DiceType HitDice);