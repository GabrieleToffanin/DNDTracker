using DNDTracker.Domain;
using DNDTracker.Domain.Campaigns;
using DNDTracker.SharedKernel.Commands;
using DNDTracker.Vocabulary.Exceptions;

namespace DNDTracker.Application.UseCases.Campaigns.Tracker;

public sealed class CastSpellCommandHandler(
    IEventPublisher eventPublisher,
    ICampaignRepository campaignRepository)
    : ICommandHandler<CastSpellCommand>
{
    public async Task Handle(CastSpellCommand request, CancellationToken cancellationToken)
    {
        var campaign = await campaignRepository.GetCampaignAsync(request.CampaignName, cancellationToken);
        if (campaign is null)
            throw new CampaignNotFoundException(request.CampaignName);

        var caster = campaign.Heroes.FirstOrDefault(h => h.Id.Id == request.CasterHeroId)
            ?? throw new CharacterNotFoundException($"Caster hero {request.CasterHeroId} not found.");

        var target = campaign.Heroes.FirstOrDefault(h => h.Id.Id == request.TargetHeroId)
            ?? throw new CharacterNotFoundException($"Target hero {request.TargetHeroId} not found.");

        var spell = caster.Spells.FirstOrDefault(s => s.Id == request.SpellId)
            ?? throw new InvalidOperationException($"Spell {request.SpellId} not found in caster's spellbook.");

        caster.UseSpellSlot(request.SlotLevel);
        caster.ApplySpellEffectTo(target, spell, request.DiceRoll);

        foreach (var domainEvent in caster.DomainEvents)
            await eventPublisher.PublishAsync(domainEvent, cancellationToken);

        await campaignRepository.UpdateAsync(campaign, cancellationToken);
    }
}
