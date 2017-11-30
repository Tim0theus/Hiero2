public class EasyDifficulty : Difficulty {
    public EasyDifficulty() : base("EasyDifficulty") { }
}

public class MediumDifficulty : Difficulty {
    public MediumDifficulty() : base("MediumDifficulty") { }
}

public class HardDifficulty : Difficulty {

    public HardDifficulty() : base("HardDifficulty") { }

    public override void Enter() {
        PlayerControls.Instance.MoveMultiplier = 2;
        base.Enter();
    }

    public override void Leave() {
        PlayerControls.Instance.MoveMultiplier = 1;
        base.Leave();
    }
}
