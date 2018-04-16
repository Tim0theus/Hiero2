using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SenetMain : Riddle {

    const bool ROUND = false;
    const bool FLAT = true;

    private SenetField previousField;
    private SenetFigure figToMove;
    private SenetFigure swapFig;

    public SenetAI ai1;
    public SenetAI ai2;

    public GameObject[] positions;
    public GameObject figureRound;
    public GameObject figureFlat;

    public GameObject[] outFields;

    public GameObject particleHint;
    public AudioClip solved;

    public SenetSticks sticks;



    private List<GameObject> flatFigures = new List<GameObject>();
    private List<GameObject> roundFigures = new List<GameObject>();

    private int roundOut = 0;
    private int flatOut = 0;

    private List<GameObject> particles = new List<GameObject>();

    private bool turn = ROUND;
    private int points;

    // Use this for initialization
    void Awake()
    {
        Assert.AreEqual(positions.Length, 30, "positions missing");

        Assert.AreEqual(outFields.Length, 10, "outFields missing");

        for (int i = 0; i < 10; i++)
        {
            outFields[i].AddComponent<AudioSource>();
            outFields[i].GetComponent<AudioSource>().clip = solved;
            outFields[i].AddComponent<SenetField>();
            outFields[i].GetComponent<SenetField>().outField = true;
        }

        for (int i = 0; i < 30; i++)
        {
            positions[i].AddComponent<AudioSource>();
            positions[i].GetComponent<AudioSource>().clip = solved;
            positions[i].AddComponent<SenetField>();
            positions[i].GetComponent<SenetField>().RiddleCode = "senet";
        }

        positions[0].GetComponent<SenetField>().id = 0;
        positions[0].GetComponent<SenetField>().after = positions[1].GetComponent<SenetField>();
        for (int i = 1; i < 29; i++)
        {
            SenetField tmp = positions[i].GetComponent<SenetField>();
            tmp.id = i;
            tmp.before = positions[i - 1].GetComponent<SenetField>();
            tmp.after = positions[i + 1].GetComponent<SenetField>();
        }
        positions[29].GetComponent<SenetField>().id = 29;
        positions[29].GetComponent<SenetField>().before = positions[28].GetComponent<SenetField>();
    }

    void Start() { 
        StartGame();
	}

    // Clean up figures and set figures on field
    public void StartGame()
    {
        TidyUp();

        for (int i = 0; i < 10; i++)
        {
            if (i % 2 == 0)
            {
                GameObject tmp = Instantiate(figureRound, positions[i].transform.GetChild(0).position, Quaternion.identity, positions[i].transform.GetChild(0));
                roundFigures.Add(tmp);
                positions[i].GetComponent<SenetField>().figure = tmp.GetComponentInChildren<SenetFigure>();
            }
            else
            {
                GameObject tmp = Instantiate(figureFlat, positions[i].transform.GetChild(0).position, Quaternion.identity, positions[i].transform.GetChild(0));
                flatFigures.Add(tmp);
                positions[i].GetComponent<SenetField>().figure = tmp.GetComponentInChildren<SenetFigure>();
            }
        }

        DeactivateFiguresAndFields();
        if (IsAI()) sticks.ThrowSticks();
    }

    // Check if it' AIs turn
    private bool IsAI()
    {
        if (ai1 && ai1.figureType == turn) return true;
        if (ai2 && ai2.figureType == turn) return true;
        return false;
    }


    // Show Ai all movable figures
    private void AssignFigureToAI(GameObject fig)
    {
        SenetFigure tmp = fig.GetComponentInChildren<SenetFigure>();
        if (ai1.figureType == tmp.flat) ai1.AddPossibleMoveFigure(tmp);
        else ai2.AddPossibleMoveFigure(tmp);
    }

    private void TellAIPoints()
    {
        if (ai1.figureType == turn) ai1.TellPoints(points);
        else ai2.TellPoints(points);
    }

    private void AIStart()
    {
        if (ai1.figureType == turn) StartCoroutine(ai1.StartMovement());
        else StartCoroutine(ai2.StartMovement());
    }

    private void TidyUp()
    {
        roundOut = 0;
        flatOut = 0;

        foreach (GameObject fig in roundFigures)
        {
            fig.transform.GetComponentInParent<SenetField>().figure = null;
            Destroy(fig);
        }
        roundFigures.Clear();

        foreach (GameObject fig in flatFigures)
        {
            fig.transform.GetComponentInParent<SenetField>().figure = null;
            Destroy(fig);
        }
    }

    // Activate movable Figures
    private void ActivateFigures()
    {
        bool active = false;
        if (IsAI())
        {
            TellAIPoints();
            if (turn == ROUND)
                foreach (GameObject o in roundFigures)
                {
                    if (o.transform.GetComponentInParent<SenetField>().canMove(points))
                    {
                        AssignFigureToAI(o);
                        active = true;
                    }
                }
            else
                foreach (GameObject o in flatFigures)
                {
                    if (o.transform.GetComponentInParent<SenetField>().canMove(points))
                    {
                        AssignFigureToAI(o);
                        active = true;
                    }
                }
        }
        else
        {
            if (turn == ROUND)
                foreach (GameObject o in roundFigures)
                {
                    if (o.transform.GetComponentInParent<SenetField>().canMove(points))
                    {
                        o.GetComponentInChildren<SenetFigure>().ActivateFigure();
                        active = true;
                    }
                }
            else
                foreach (GameObject o in flatFigures)
                {
                    if (o.transform.GetComponentInParent<SenetField>().canMove(points))
                    {
                        o.GetComponentInChildren<SenetFigure>().ActivateFigure();
                        active = true;
                    }
                }
        }

        if (!active)
        {
            turn = !turn;
            if (!IsAI()) sticks.Activate();
            else sticks.ThrowSticks();
        }
        else
        {
            if (IsAI()) AIStart();
        }
    }

    public override void Solve()
    {
        base.Solve();
        sticks.DeActivate();
        foreach(GameObject o in roundFigures)
        {
            o.GetComponentInChildren<SenetFigure>().Item.PutdownNoInventory(outFields[roundOut++].transform.GetChild(0));
        }
    }

    // Determine if figure is out of field and game is won or lost on pickup. Or let the figure to be placed on possible fields.
    public bool OnPickup(SenetFigure fig)
    {
        DeactivateFigures();
        figToMove = fig;
        previousField = figToMove.GetComponentInParent<SenetField>();
        previousField.figure = null;
        SenetField target = figToMove.GetComponentInParent<SenetField>().GetTarget(points);
        if (target == null)
        {
            figToMove.GetComponentInParent<SenetField>().figure = null;

            fig.Item.PutdownNoInventory(outFields[figToMove.flat ? flatOut +5 : roundOut].transform.GetChild(0));

            if (figToMove.flat) flatOut++;
                else roundOut++;

            if (roundOut == 5)
            {
                Solved();
                return false;
            }
            if (flatOut == 5)
            {
                StartGame();
                SoundController.instance.Play("error");
                GameControl.instance.SubtractPoint(null, null);

                return false;
            }

            ChangeTurns();

            return false;
        }
        previousField.Activate();
        target.Activate();

        particles.Add(Instantiate(particleHint, previousField.transform.GetChild(0).position, Quaternion.identity));
        particles.Add(Instantiate(particleHint, target.transform.GetChild(0).position, Quaternion.identity));

        swapFig = target.figure;
        return true;
    }

    // Determine what happens on PutDown of figure, Switch, etc.
    public void OnPutDown()
    {
        foreach (GameObject particle in particles)
        {
            Destroy(particle);
        }
        particles.Clear();

        figToMove.GetComponentInParent<SenetField>().figure = figToMove;

        if (figToMove.GetComponentInParent<SenetField>().id == 26)
        {
            figToMove.GetComponentInParent<SenetField>().figure = null;
            int pos = 14;
            while (positions[pos].GetComponent<SenetField>().figure) pos--;
            figToMove.Item.PutdownNoInventory(positions[pos].transform.GetChild(0));
            positions[pos].GetComponent<SenetField>().figure = figToMove;
            ChangeTurns();
        }
        else if (previousField.id != figToMove.GetComponentInParent<SenetField>().id)
        {
            if (swapFig)
            {
                previousField.figure = swapFig;
                swapFig.Item.PutdownNoInventory(previousField.transform.GetChild(0));
            }
            ChangeTurns();
        }
        else
        {
            ActivateFigures();
            DeactivateFields();
        }
    }

    private void ChangeTurns()
    {
        DeactivateFiguresAndFields();
        if (points == 2 || points == 3) turn = !turn;
        if (!IsAI()) sticks.Activate();
        else sticks.ThrowSticks();
    }

    public void OnPointsCalculated(int points)
    {
        this.points = points;
        ActivateFigures();
    }

    private void DeactivateFiguresAndFields()
    {

        DeactivateFigures();

        DeactivateFields();

    }

    private void DeactivateFigures()
    {
        foreach (GameObject o in roundFigures)
        {
            o.GetComponentInChildren<SenetFigure>().DeactivateFigure();
        }

        foreach (GameObject o in flatFigures)
        {
            o.GetComponentInChildren<SenetFigure>().DeactivateFigure();
        }

    }

    private void DeactivateFields()
    {
        foreach (GameObject o in positions)
        {
            o.GetComponent<SenetField>().Deactivate();
        }
    }

}
