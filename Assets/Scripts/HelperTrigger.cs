using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperTrigger : TriggerVolume
{

    public string[] Infotext;

    private int iterations;

    public bool keyboard;

    public Riddle[] riddles;

    private void Awake()
    {
        iterations = 0;
    }

    public void Use()
    {
        iterations = ++iterations % Infotext.Length;
        
    }

    private bool CheckRiddles()
    {
        if (riddles.Length == 0) return true;
        int count = 0;
        foreach (Riddle r in riddles)
        {
            if (r.IsSolved) count++;
        }

        return riddles.Length > count;
    }

    private bool CheckInput()
    {
        return !keyboard || (keyboard && PlayerControls.Instance.CurrentType == ControlType.MouseKeyboard);
    }

    protected override void Activate()
    {
        if (CheckInput() && CheckRiddles())
        {
            Hint.instance.SetText(this, Infotext[iterations]);
        }
    }

    protected override void Check()
    {
        if (CheckInput() && CheckRiddles())
        {
            int count = 0;
            int result = 1;

            if (riddles.Length > 0)
            {

                foreach (Riddle r in riddles)
                {
                    if (r.IsSolved) count++;
                }
                result = riddles.Length - count;
            }

            if (result == 0) Hint.instance.SetText(this, "");
            else Hint.instance.SetText(this, Infotext[iterations].Replace("*", result.ToString()));
        }
        else if (Hint.instance.GetCurrentHelper() == this)
        {
            Deactivate();
        }
    }

    protected override void Deactivate()
    {
        Hint.instance.SetText(this, "");
    }
}
