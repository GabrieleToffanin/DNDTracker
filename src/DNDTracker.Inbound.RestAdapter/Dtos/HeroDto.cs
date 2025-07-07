using DNDTracker.Vocabulary.Enums;

namespace DNDTracker.Inbound.RestAdapter.Dtos;

public record HeroDto(
    string Name,
    HeroClass Class,
    Race Race,
    Alignment Alignment,
    int Level,
    int Experience,
    int HitPoints,
    DiceType HitDice);