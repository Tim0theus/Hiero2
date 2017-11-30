using System.Collections.Generic;

public abstract class Mode : IMode {

    protected readonly HashSet<IActivatable> Activate = new HashSet<IActivatable>();
    protected readonly HashSet<IActivatable> DeActivate = new HashSet<IActivatable>();

    public void Enter() {
        foreach (IActivatable activ in Activate) {
            activ.Activate();
        }
        foreach (IActivatable deactive in DeActivate) {
            deactive.DeActivate();
        }
    }

    public void Leave() {
        foreach (IActivatable activ in Activate) {
            activ.DeActivate();
        }
        foreach (IActivatable deactive in DeActivate) {
            deactive.Activate();
        }
    }

}
