public class FaderActivatable : Activatable {

    protected Fader Fader;

    public override void Activate() {
        if (Fader) Fader.Activate();
    }

    public override void DeActivate() {
        if (Fader) Fader.DeActivate();
    }
}

