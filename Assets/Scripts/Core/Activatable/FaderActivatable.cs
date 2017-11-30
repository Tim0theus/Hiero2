public class FaderActivatable : Activatable {

    protected Fader Fader;

    public override void Activate() {
        Fader.Activate();
    }

    public override void DeActivate() {
        Fader.DeActivate();
    }
}

