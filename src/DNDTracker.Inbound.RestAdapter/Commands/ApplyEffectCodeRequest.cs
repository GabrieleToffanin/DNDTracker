namespace DNDTracker.Inbound.RestAdapter.Commands;

public record ApplyEffectCodeRequest(
    string RawEffectCode,
    int? DurationRounds = null);
