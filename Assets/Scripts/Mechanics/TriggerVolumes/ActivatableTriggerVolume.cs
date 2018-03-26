using System.Collections.Generic;

public class ActivatableTriggerVolume : TriggerVolume {
    public List<Activatable> Activatables;

    protected override void Activate() {
        foreach (Activatable activatable in Activatables) {
            activatable.Activate();
        }
    }

    protected override void Check()
    {
    }

    protected override void Deactivate() {
        foreach (Activatable activatable in Activatables) {
            activatable.DeActivate();
        }
    }
}


