using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameControl : MonoBehaviour {

    public static GameControl instance;

    public static event EventHandler DataLoaded;

    public Transform spawn;

    public GameObject player;
    public GameObject canvas;

    public Menu tutorial;

    public volatile Dictionary<string, Stream> objectData;

    private GameObject _options;

    private Timer _timer;

    private Text _points;

    private Coroutine _autosave;

    private FullscreenOverlay _mainOverlay;

    private int points;

    public int GetPoints()
    {
        return points;
    }

    private void Awake() {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Assert.IsNotNull(player, "player missing");

        _options = GameObject.FindGameObjectWithTag("Options");
        _mainOverlay = GameObject.FindGameObjectWithTag("MainOverlay").GetComponent<FullscreenOverlay>();
        _timer = canvas.GetComponentInChildren<Timer>();
        _points = canvas.transform.GetChild(1).GetChild(10).GetComponent<Text>();
        objectData = new Dictionary<string, Stream>();
        points = 0;
    }

    private void Start() {
        _mainOverlay.DeActivate();

        _options.transform.GetChild(1).GetComponentInChildren<Slider>().value = PlayerPrefs.GetFloat("Music Volume", 0.5f);
        _options.transform.GetChild(2).GetComponentInChildren<Toggle>().isOn = PlayerPrefs.GetInt("Invert Camera Y", 0) ==  0 ? false : true;
        _options.transform.GetChild(3).GetComponentInChildren<Toggle>().isOn = PlayerPrefs.GetInt("Invert Scrollwheel", 0) == 0 ? false : true;
        _options.transform.GetChild(4).GetComponentInChildren<Slider>().value = PlayerPrefs.GetFloat("Camera Sensitivity", 1.0f);
        _options.transform.GetChild(5).GetComponentInChildren<Slider>().value = PlayerPrefs.GetFloat("Move Speed", 1.0f);

        _options.transform.GetChild(1).GetComponentInChildren<Slider>().onValueChanged.AddListener(SetMusicVolume);
        _options.transform.GetChild(2).GetComponentInChildren<Toggle>().onValueChanged.AddListener(SetInvertCameraY);
        _options.transform.GetChild(3).GetComponentInChildren<Toggle>().onValueChanged.AddListener(SetInvertScrollwheel);
        _options.transform.GetChild(4).GetComponentInChildren<Slider>().onValueChanged.AddListener(SetCameraSensitivity);
        _options.transform.GetChild(5).GetComponentInChildren<Slider>().onValueChanged.AddListener(SetMoveSpeed);

        Load();
    }

    public void SetMusicVolume(float f)
    {
        PlayerPrefs.SetFloat("Music Volume", f);
        PlayerPrefs.Save();
    }
    public void SetInvertCameraY(bool b)
    {
        PlayerPrefs.SetInt("Invert Camera Y", b ? 1 : 0);
        PlayerPrefs.Save();
    }
    public void SetInvertScrollwheel(bool b)
    {
        PlayerPrefs.SetInt("Invert Scrollwheel", b ? 1 : 0);
        PlayerPrefs.Save();
    }
    public void SetCameraSensitivity(float f)
    {
        PlayerPrefs.SetFloat("Camera Sensitivity", f);
        PlayerPrefs.Save();
    }
    public void SetMoveSpeed(float f)
    {
        PlayerPrefs.SetFloat("Move Speed", f);
        PlayerPrefs.Save();
    }

    public void ActivateSave()
    {
        _autosave = StartCoroutine(Autosave());
        canvas.transform.GetChild(7).GetChild(1).GetChild(2).GetChild(2).GetComponent<Button>().interactable = true;
        canvas.transform.GetChild(7).GetChild(1).GetChild(2).GetChild(3).GetComponent<Button>().interactable = true;
        canvas.transform.GetChild(7).GetChild(1).GetChild(2).GetChild(2).GetComponent<ButtonControl>().DeActivate();
        canvas.transform.GetChild(7).GetChild(1).GetChild(2).GetChild(3).GetComponent<ButtonControl>().DeActivate();
    }

    public void DeactivateSave()
    {
        if (_autosave != null) StopCoroutine(_autosave);
        //Menu Save
        canvas.transform.GetChild(7).GetChild(1).GetChild(2).GetChild(2).GetComponent<Button>().interactable = false;
        canvas.transform.GetChild(7).GetChild(1).GetChild(2).GetChild(3).GetComponent<Button>().interactable = false;
        canvas.transform.GetChild(7).GetChild(1).GetChild(2).GetChild(2).GetComponent<ButtonControl>().DeActivate();
        canvas.transform.GetChild(7).GetChild(1).GetChild(2).GetChild(3).GetComponent<ButtonControl>().DeActivate();
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Restart()
    {
        DeactivateSave();
        StartCoroutine(NewGame());
    }

    public void LoadGame()
    {
        DeactivateSave();
        StartCoroutine(NewGame(false));
    }

    private IEnumerator NewGame(bool delete = true)
    {
        _mainOverlay.Activate();
        yield return new WaitForSeconds(_mainOverlay.FadeDuration);

        if (delete)
        File.Delete(Application.persistentDataPath + "/game.dat");

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(0);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator Autosave()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1);
            Save();
        }
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/game.dat");

        GameData dat = new GameData();
        dat.player = new myTransform(player.transform);

        dat.points = points;

        dat.glyphs = LiteralPicker.SaveGlyphs();

        dat.timerTime = _timer.GetTime();
        dat.difficulty = (int)player.GetComponent<PlayerMechanics>().CurrentDifficulty;

        dat.dict = objectData;

        bf.Serialize(file, dat);
        file.Close();
    }

    private void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/game.dat"))
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/game.dat", FileMode.Open);

                GameData dat = (GameData)bf.Deserialize(file);
                file.Close();
                dat.player.setTransform(player.transform);

                points = dat.points;
                _points.text = "Points: " + points;
                _points.GetComponent<TextControl>().Activate();

                LiteralPicker.LoadGlyphs(dat.glyphs);

                _timer.SetTime(dat.timerTime);

                player.GetComponent<PlayerMechanics>().SetDifficulty(dat.difficulty);
                if (dat.difficulty == 2) _timer.Activate();


                objectData = dat.dict;

                DataLoaded(this, null);
                ActivateSave();
            }
            catch (SerializationException e)
            {
                Debug.Log(e);
                Spawn();
            }
        }
        else
        {
            PlayerMechanics.Instance.SetControlMode(ControlMode.DimScreen);
            tutorial.Open();
        }

        Riddle[] list = GameObject.FindObjectsOfType<Riddle>();
        foreach (Riddle l in list)
        {
            l.onSolved += AddPoint;
            l.onFailed += SubtractPoint;
        }

        AddPoints(0);
    }

    public void OpenLexicon()
    {
        if (player.GetComponent<PlayerMechanics>().CurrentDifficulty == DifficultyLevel.Medium) AddPoints(-5);
        if (player.GetComponent<PlayerMechanics>().CurrentDifficulty == DifficultyLevel.Hard) AddPoints(-10);
    }

    public void AddPoints(int value)
    {
        points += value;
        points = points < 0 ? 0 : points;
        _points.text = "Points: " + points;



        if (points > 4)
        {
            if (player.GetComponent<PlayerMechanics>().CurrentDifficulty == DifficultyLevel.Medium)
            {
                canvas.transform.GetChild(7).GetChild(1).GetChild(2).GetChild(1).GetComponent<Button>().interactable = true;
                canvas.transform.GetChild(7).GetChild(1).GetChild(2).GetChild(1).GetComponent<ButtonControl>().DeActivate();
            }
            else if (player.GetComponent<PlayerMechanics>().CurrentDifficulty == DifficultyLevel.Hard && points > 9)
            {
                canvas.transform.GetChild(7).GetChild(1).GetChild(2).GetChild(1).GetComponent<Button>().interactable = true;
                canvas.transform.GetChild(7).GetChild(1).GetChild(2).GetChild(1).GetComponent<ButtonControl>().DeActivate();
            }
        }
        if (points < 10)
        {
            if (player.GetComponent<PlayerMechanics>().CurrentDifficulty == DifficultyLevel.Hard)
            {
                canvas.transform.GetChild(7).GetChild(1).GetChild(2).GetChild(1).GetComponent<Button>().interactable = false;
                canvas.transform.GetChild(7).GetChild(1).GetChild(2).GetChild(1).GetComponent<ButtonControl>().DeActivate();
            }
            else if (player.GetComponent<PlayerMechanics>().CurrentDifficulty == DifficultyLevel.Medium && points < 5)
            {
                canvas.transform.GetChild(7).GetChild(1).GetChild(2).GetChild(1).GetComponent<Button>().interactable = false;
                canvas.transform.GetChild(7).GetChild(1).GetChild(2).GetChild(1).GetComponent<ButtonControl>().DeActivate();
            }
        }

    }

    public void AddPoint(object sender, EventArgs arg)
    {
        AddPoints(1);
    }

    public void SubtractPoint(object sender, EventArgs arg)
    {
        AddPoints(-1);
    }

    public void Spawn()
    {
        objectData.Clear();
        player.transform.position = spawn.position;
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(GameControl), true)]
    public class AddSaves : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Add all ItemSaves"))
            {

                foreach (RigidBodyPickUp o in GameObject.FindObjectsOfType<RigidBodyPickUp>())
                {

                    if (o.gameObject.GetComponent<SaveState>() == null) o.gameObject.AddComponent<SaveState>();
                }
                foreach (PutDown o in GameObject.FindObjectsOfType<PutDown>())
                {

                    if (o.gameObject.GetComponent<SaveState>() == null) o.gameObject.AddComponent<SaveState>();
                }
                foreach (Riddle o in GameObject.FindObjectsOfType<Riddle>())
                {

                    if (o.gameObject.GetComponent<SaveState>() == null) o.gameObject.AddComponent<SaveState>();
                }
            }
            if (GUILayout.Button("Assign unique IDs"))
            {
                foreach (SaveState o in GameObject.FindObjectsOfType<SaveState>())
                {
                    if (o.uniqueID == "")
                    {
                        o.GetComponent<SaveState>().uniqueID = Guid.NewGuid().ToString();
                        EditorUtility.SetDirty(o);
                    }
                }
            }
        }
    }
#endif
}

[Serializable]
public struct myVector3
{
    float x, y, z;

    public myVector3(Vector3 vec)
    {
        x = vec.x; y = vec.y; z = vec.z;
    }

    public Vector3 getVector3()
    {
        return new Vector3(x, y, z);
    }
}

[Serializable]
public struct myQuaternion
{
    float x, y, z, w;

    public myQuaternion(Quaternion quat)
    {
        x = quat.x; y = quat.y; z = quat.z; w = quat.w;
    }

    public Quaternion getQuaternion()
    {
        return new Quaternion(x, y, z, w);
    }
}

[Serializable]
public struct myTransform
{
    myVector3 position;
    myQuaternion rotation;

    public myTransform(Transform t)
    {
        position = new myVector3(t.position);
        rotation = new myQuaternion(t.rotation);
    }

    public void setTransform(Transform toBeSet)
    {
        toBeSet.position = position.getVector3();
        toBeSet.rotation = rotation.getQuaternion();
    }

    public Vector3 getPosition()
    {
        return position.getVector3();
    }

}

[Serializable]
struct GameData
{
    public myTransform player;
    public int difficulty;
    public float timerTime;
    public int points;

    public string[] glyphs;

    public Dictionary<string, Stream> dict;
}

