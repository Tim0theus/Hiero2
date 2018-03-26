using System;
using System.Collections.Generic;
using System.Linq;

public class RiddleChangesMode : RiddleAggregator {

    public List<RiddleToMode> Modes;

    private Dictionary<Riddle, ControlMode> _riddleToMode;
    private int _instanceId;

    private void Start() {
        _riddleToMode = Modes.ToDictionary(item => item.Riddle, item => item.Mode);
        _instanceId = GetInstanceID();
    }

    public override void UpdateStatus(IObservable observable) {
        base.UpdateStatus(observable);
        Riddle riddle = (Riddle)observable;

        if (riddle.IsSolved && _riddleToMode.ContainsKey(riddle)) {
            PlayerMechanics.Instance.SetControlMode(_instanceId, _riddleToMode[riddle]);
        }
    }
}

[Serializable]
public struct RiddleToMode {
    public Riddle Riddle;
    public ControlMode Mode;
}