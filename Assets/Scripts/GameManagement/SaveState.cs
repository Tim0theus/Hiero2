using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Assertions;

public class SaveState : MonoBehaviour {

    public string uniqueID;

    public int saveSeconds = 60;
    private float elapsedTime;

    public bool useRigidbody;
    private bool wasMoving = false;

    private Vector3 _lastposition;

    private Item _item;
    private Rigidbody _rigidbody;
    private Riddle _riddle;

    public void Awake()
    {
        Assert.AreNotEqual(uniqueID, "", "No ID: " + (transform.parent != null ? transform.parent.name + " " : "") + gameObject.name);
        _item = GetComponentInParent<Item>();

        _rigidbody = GetComponent<Rigidbody>();

        _riddle = GetComponent<Riddle>();

        GameControl.DataLoaded += OnLoaded;

    }

    private void OnDestroy()
    {
        GameControl.DataLoaded -= OnLoaded;
    }

    public void Start()
    {
        if (_item) _item.Placed += OnDone;
        if (_riddle)
        {
            _riddle.onSolved += OnDone;
            _riddle.onFailed += OnDone;
            _riddle.onReset += OnDone;
        }

    }

    public void Update()
    {
        if (!_item && !_riddle)
        {
            if (useRigidbody && _rigidbody)
            {
                if (_rigidbody.IsSleeping() && wasMoving)
                {
                    Save();
                    wasMoving = false;
                }
                else
                {
                    wasMoving = !_rigidbody.IsSleeping();
                }
            }
            else if (saveSeconds < elapsedTime)
            {
                elapsedTime = 0;
                if ((transform.position - _lastposition).magnitude != 0)
                {
                    _lastposition = transform.position;
                    Save();
                }
            }
            elapsedTime += Time.deltaTime;
        }
    }

    public void OnLoaded(object o, EventArgs e)
    {
        Stream s;

        if (GameControl.instance.objectData.TryGetValue(uniqueID, out s) && s != null)
        {
            BinaryFormatter bf = new BinaryFormatter();
            s.Seek(0, SeekOrigin.Begin);

            if (_item)
            {
                ItemSave v = (ItemSave)bf.Deserialize(s);
                v.Apply(gameObject, _item.gameObject);
            }
            else if (_riddle)
            {
                RiddleSave rv = (RiddleSave)bf.Deserialize(s);
                rv.Apply(gameObject);
            }
            else
            {
                Standard v = (Standard)bf.Deserialize(s);
                v.Apply(gameObject);
            }
        }
        _lastposition = transform.position;

    }

    public void OnDone(object o, EventArgs e)
    {
        Save();
    }

    void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        Stream s = new MemoryStream();

        if (_item)
        {
            ItemSave v = new ItemSave(gameObject, _item.gameObject);
            bf.Serialize(s, v);
        }
        else if (_riddle)
        {
            RiddleSave rv = new RiddleSave(gameObject);
            bf.Serialize(s, rv);
        }
        else
        {
            Standard v = new Standard(gameObject);
            bf.Serialize(s, v);
        }

        GameControl.instance.objectData[uniqueID] = s;

    }

    [Serializable]
    class Standard
    {
        myTransform t;

        public Standard(GameObject g)
        {
            t = new myTransform(g.transform);
        }

        public void Apply(GameObject g) {
            t.setTransform(g.transform);
        }
    }

    [Serializable]
    class ItemSave
    {
        myTransform t;
        myTransform itemT;

        public ItemSave(GameObject g, GameObject itemg)
        {
            t = new myTransform(g.transform);
            itemT = new myTransform(itemg.transform);
        }

        public void Apply(GameObject g, GameObject itemg)
        {
            itemT.setTransform(itemg.transform);
            t.setTransform(g.transform);
            g.GetComponent<RigidBodyPickUp>().PutDown();
            g.GetComponent<RigidBodyPickUp>().DeActivate();
        }
    }

    [Serializable]
    class RiddleSave
    {
        bool solved;

        public RiddleSave(GameObject g)
        {
            solved =  g.GetComponent<Riddle>().IsSolved;
        }

        public void Apply(GameObject g)
        {
            if (solved)
            {
                g.GetComponent<Riddle>().Solve();

            }
        }
    }

}
