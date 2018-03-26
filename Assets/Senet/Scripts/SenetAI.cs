using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SenetAI : MonoBehaviour {

    public bool figureType = true;

    private SenetMain senet;
    private int points;

    private List<SenetFigure> moves = new List<SenetFigure>();
    private List<int> priorities = new List<int>();

	void Awake () {
        senet = this.gameObject.GetComponent<SenetMain>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddPossibleMoveFigure(SenetFigure fig)
    {
        moves.Add(fig);

        SenetField target = fig.GetComponentInParent<SenetField>().id + points < 29 ? senet.positions[fig.GetComponentInParent<SenetField>().id + points].GetComponent<SenetField>() : null;

        int tmp = 0;
        if (fig.GetComponentInParent<SenetField>().id == 25 && points == 1) tmp = 9999;
        if (fig.GetComponentInParent<SenetField>().id + points == 25) tmp-=10;
        if (target && target.figure) tmp--;
        priorities.Add(tmp);
    }

    public IEnumerator StartMovement()
    {
        yield return new WaitForSecondsRealtime (2);
        int tmp = int.MaxValue, ind = 0;
        for (int i = 0; i < moves.Count; i++)
        {
            if (tmp > priorities[i])
            {
                tmp = priorities[i];
                ind = i;
            }
        }
        if (senet.OnPickup(moves[ind]))
            moves[ind].Item.PutdownNoInventory(senet.positions[moves[ind].GetComponentInParent<SenetField>().id+points].transform.GetChild(0), senet.positions[moves[ind].GetComponentInParent<SenetField>().id + points].GetComponent<SenetField>().FigurePlaced);
        moves.Clear();
        priorities.Clear();
    }

    public void TellPoints(int points)
    {
        this.points = points;
    }
}
