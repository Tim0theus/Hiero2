using System.Collections.Generic;

public class ReadingDirection : RiddleAggregator, IActivatable {
    private readonly List<CompositeTouchGlyph2D> _glyphs = new List<CompositeTouchGlyph2D>();

    public void Activate() {
        _glyphs[0].Activate();
        enabled = true;
    }

    public void DeActivate() {
        foreach (CompositeTouchGlyph2D glyph in _glyphs) glyph.DeActivate();
        enabled = false;
    }

    private void Start() {
        foreach (Riddle riddle in Riddles) {
            _glyphs.Add(riddle as CompositeTouchGlyph2D);
        }
    }

    public override void UpdateStatus(IObservable observable) {
        base.UpdateStatus(observable);
        Riddle riddle = (Riddle)observable;

        if (riddle.IsFailed) {
            _glyphs[SolvedElements].DeActivate();
            Reset();
        }
        else if (riddle.IsSolved) {
            _glyphs[SolvedElements - 1].DeActivate();
        }

        if (SolvedElements < _glyphs.Count) {
            _glyphs[SolvedElements].Activate();
        }
    }
}