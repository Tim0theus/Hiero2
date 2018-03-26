public class SlideTileAggregator : RiddleAggregator {
    private SlideTile[] _slideTiles;
    private PutDown _putDown;

    private new void Awake() {
        base.Awake();

        _putDown = GetComponentInChildren<PutDown>();
        _putDown.enabled = false;

        _slideTiles = GetComponentsInChildren<SlideTile>();
    }

    public override void Solve()
    {
        base.Solve();
        GetComponent<SlideTileMechanic>().Solve();
    }


    public override void UpdateStatus(IObservable observable) {
        base.UpdateStatus(observable);
        if (SolvedElements == Riddles.Count - 1) {
            foreach (SlideTile slideTile in _slideTiles) {
                slideTile.enabled = false;
            }
            _putDown.enabled = true;
        }
    }
}