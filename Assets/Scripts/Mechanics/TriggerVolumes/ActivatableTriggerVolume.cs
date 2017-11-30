using System.Collections.Generic;

public class ActivatableTriggerVolume : TriggerVolume {
    public List<Activatable> Activatables;

    protected override void Activate() {
        foreach (Activatable activatable in Activatables) {
            activatable.Activate();
        }
    }

    protected override void Deactivte() {
        foreach (Activatable activatable in Activatables) {
            activatable.DeActivate();
        }
    }
}


