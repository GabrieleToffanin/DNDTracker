using System.ComponentModel.DataAnnotations;

namespace DNDTracker.BlazorUI.Models;

// Campaign DTOs
public record CampaignDto(
    string CampaignName,
    string CampaignDescription,
    string CampaignImage,
    DateTime CreatedDate,
    bool IsActive = true);

// Request DTOs  
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

// Enums (copied from Vocabulary to remove dependency)
public enum HeroClass
{
    Barbarian,
    Bard,
    Cleric,
    Druid,
    Fighter,
    Monk,
    Paladin,
    Ranger,
    Rogue,
    Sorcerer,
    Warlock,
    Wizard
}

public enum Race
{
    Human,
    Elf,
    Dwarf,
    Halfling,
    Dragonborn,
    Gnome,
    HalfElf,
    HalfOrc,
    Tiefling
}

public enum Alignment
{
    LawfulGood,
    NeutralGood,
    ChaoticGood,
    LawfulNeutral,
    TrueNeutral,
    ChaoticNeutral,
    LawfulEvil,
    NeutralEvil,
    ChaoticEvil
}

public enum DiceType
{
    D4 = 4,
    D6 = 6,
    D8 = 8,
    D10 = 10,
    D12 = 12,
    D20 = 20
}

// View Models for forms
public class CreateCampaignModel
{
    [Required(ErrorMessage = "Campaign name is required")]
    [StringLength(100, ErrorMessage = "Campaign name must be less than 100 characters")]
    public string? CampaignName { get; set; }

    [Required(ErrorMessage = "Campaign description is required")]
    [StringLength(1000, ErrorMessage = "Description must be less than 1000 characters")]
    public string? CampaignDescription { get; set; }

    [Required(ErrorMessage = "Campaign image URL is required")]
    [Url(ErrorMessage = "Please enter a valid URL")]
    public string? CampaignImage { get; set; }

    public bool IsActive { get; set; } = true;
}

public class AddHeroModel
{
    [Required(ErrorMessage = "Hero name is required")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Please select a class")]
    public HeroClass? Class { get; set; }

    [Required(ErrorMessage = "Please select a race")]
    public Race? Race { get; set; }

    [Required(ErrorMessage = "Please select an alignment")]
    public Alignment? Alignment { get; set; }

    [Range(1, 20, ErrorMessage = "Level must be between 1 and 20")]
    public int Level { get; set; } = 1;

    [Range(0, int.MaxValue, ErrorMessage = "Experience must be non-negative")]
    public int Experience { get; set; } = 0;

    [Range(1, int.MaxValue, ErrorMessage = "Hit points must be at least 1")]
    public int HitPoints { get; set; } = 8;

    [Required(ErrorMessage = "Please select hit dice")]
    public DiceType? HitDice { get; set; }
}