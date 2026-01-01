using Content.Server.Administration.Logs;
using Content.Server.Popups;
using Content.Shared.Database;
using Content.Shared.Implants;
using Content.Shared.Mindshield.Components;
using Content.Shared._Forge.Overlord.Components;
using Robust.Shared.Containers;

namespace Content.Server._Forge.Overlord;

/// <summary>
/// System used for adding or removing components with a overlord implant
/// </summary>
public sealed class MindShieldSystem : EntitySystem
{
    [Dependency] private readonly IAdminLogManager _adminLogManager = default!;
    [Dependency] private readonly PopupSystem _popupSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<OverlordImplantComponent, ImplantImplantedEvent>(OnImplantImplanted);
        SubscribeLocalEvent<OverlordImplantComponent, EntGotRemovedFromContainerMessage>(OnImplantDraw);
    }

    private void OnImplantImplanted(Entity<OverlordImplantComponent> ent, ref ImplantImplantedEvent ev)
    {
        var mob = ev.Implanted;
        if (mob == null)
            return;

        if (HasComp<MindShieldComponent>(mob.Value))
        {
            RemCompDeferred<MindShieldComponent>(mob.Value);
            _popupSystem.PopupEntity(Loc.GetString("head-rev-break-mindshield"), mob.Value);
        }

        EnsureComp<OverlordComponent>(mob.Value);
        _adminLogManager.Add(LogType.Mind, LogImpact.Medium, $"{ToPrettyString(mob)} was converted using OverlordImplant.");
    }

    private void OnImplantDraw(Entity<OverlordImplantComponent> ent, ref EntGotRemovedFromContainerMessage args)
    {
        RemComp<OverlordComponent>(args.Container.Owner);
    }
}
