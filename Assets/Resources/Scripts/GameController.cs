using UnityEngine;

public class GameController : MonoBehaviour
{
    public static bool newGame; // Controlled by title screen.

    public static Camera cam;
    public static PlayerMovement player;
    public static UIController uiCont;
    public static EntityAI[] entities;
    public static Pickup[] pickups;
    public static Interactable[] interactables;
    public static GameObject[] cutscenes;
    public static GameObject[] tutorials;

    public static GameObject savingIndicator;

    public static float musicScalar = 1f;

    public static float musicVolume = 1f;
    public static float musicVolume2 = 0.5f;

    private float timeBetweenAmbientNoises = 2f;
    private float timeSinceLastAmbientNoise = 0f;

    [SerializeField] private AudioClip[] ambientSounds = new AudioClip[3];
    private AudioSource camAudio;

    [SerializeField] private bool ignoreSave = false;

    public static GameObject nighttimeEnemiesObj;
    public static System.Collections.Generic.List<GameObject> nighttimeEnemies = new System.Collections.Generic.List<GameObject>();

    private void Awake()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        uiCont = FindObjectOfType<UIController>();
        entities = FindObjectsOfType<EntityAI>();
        pickups = FindObjectsOfType<Pickup>();
        interactables = FindObjectsOfType<Interactable>();
        cutscenes = GameObject.FindGameObjectsWithTag("Cutscene");
        tutorials = GameObject.FindGameObjectsWithTag("Tutorial");

        camAudio = cam.GetComponent<AudioSource>();

        nighttimeEnemiesObj = GameObject.FindGameObjectWithTag("Enemies2");
        foreach (Transform enemy in nighttimeEnemiesObj.GetComponentsInChildren<Transform>())
        {
            if (enemy.gameObject != nighttimeEnemiesObj)
            {
                nighttimeEnemies.Add(enemy.gameObject);
                Debug.Log(enemy.name);
            }
        }

        savingIndicator = GameObject.FindGameObjectWithTag("Saving");
        savingIndicator.SetActive(false);

        if (!ignoreSave)
        {
            if (newGame)
            {
                UIController.numFood = 0;
                UIController.numDays = 1;
                UIController.time = 0f;
                UIController.startSunLate = true;
                UIController.timePassedSinceGameBegun = 0f;
                uiCont.timeDisplay.transform.Rotate(new Vector3(0f, 0f, 0f));
                CollectBerries.berriesCollectedNum = 0;
                PlayerMovement.deathCount = 0;

                SaveGame(SaveDataController.AutoSavePath);
                SaveGame(SaveDataController.ManualSavePath);
            }
            else
            {
                TryLoadGame();
            }
        }

        if (UIController.timeTitle == "night")
        {
            SpawnEnemies();
        }
        else
        {
            DespawnEnemies();
        }
    }

    private void Update()
    {
        if (UIController.timeTitle == "night")
        {
            musicVolume = Mathf.Lerp(musicVolume, 0f, Time.deltaTime);
            musicVolume2 = Mathf.Lerp(musicVolume2, 0f, Time.deltaTime);

            timeSinceLastAmbientNoise += Time.deltaTime;

            if (timeSinceLastAmbientNoise >= timeBetweenAmbientNoises)
            {
                timeSinceLastAmbientNoise = 0f;
                timeBetweenAmbientNoises = Random.Range(5f, 10f);

                PlaySound(camAudio, ambientSounds, true);
            }
        }
        else
        {
            musicVolume = Mathf.Lerp(musicVolume, 1f, Time.deltaTime);
            musicVolume2 = Mathf.Lerp(musicVolume, 0.5f, Time.deltaTime);
        }
    }

    public void PlaySound(AudioSource source, AudioClip[] sound, bool interuptSound = false, float pitch = 1f, float volume = 1f)
    {
        if (source.isPlaying && !interuptSound)
            return;

        source.clip = sound[Random.Range(0, sound.Length)];
        source.pitch = pitch;
        source.volume = volume;
        source.Play();
    }

    public static void TryLoadGame()
    {
        Debug.Log("Loading Game");
        LoadGame(SaveDataController.ManualSavePath);
    }

    public static void TryLoadAutoSaveGame()
    {
        Debug.Log("Loading Auto Save Game");
        LoadGame(SaveDataController.AutoSavePath);

        foreach (CollectBerries berry in FindObjectsOfType<CollectBerries>())
        {
            berry.CheckBerries();
        }
    }

    public static void TrySaveGame()
    {
        if (savingIndicator.activeSelf)
            return;

        Debug.Log("Saving Game");
        SaveGame(SaveDataController.ManualSavePath);
    }

    public static void TryAutoSaveGame()
    {
        if (savingIndicator.activeSelf)
            return;

        Debug.Log("Auto Saving Game");
        SaveGame(SaveDataController.AutoSavePath);
    }

    private static void SaveGame(string path)
    {
        savingIndicator.SetActive(true);

        SaveDataController.SaveCamera(Application.persistentDataPath + path);
        SaveDataController.SavePlayer(Application.persistentDataPath + path);
        SaveDataController.SaveUI(Application.persistentDataPath + path);

        for (int i = 0; i < entities.Length; ++i)
        {
            SaveDataController.SaveEntity(i, Application.persistentDataPath + path);
        }

        for (int i = 0; i < pickups.Length; ++i)
        {
            SaveDataController.SavePickup(i, Application.persistentDataPath + path);
        }

        for (int i = 0; i < interactables.Length; ++i)
        {
            SaveDataController.SaveInteractable(i, Application.persistentDataPath + path);
        }

        for (int i = 0; i < cutscenes.Length; ++i)
        {
            SaveDataController.SaveCutscene(i, Application.persistentDataPath + path);
        }

        for (int i = 0; i < tutorials.Length; ++i)
        {
            SaveDataController.SaveTutorial(i, Application.persistentDataPath + path);
        }
    }

    private static void LoadGame(string path)
    {
        // Camera.
        CameraSaveData camData = SaveDataController.LoadCamera(Application.persistentDataPath + path);
        cam.transform.position = new Vector3(camData.position[0], camData.position[1], camData.position[2]);

        // Player.
        PlayerSaveData playerData = SaveDataController.LoadPlayer(Application.persistentDataPath + path);
        player.transform.position = new Vector2(playerData.position[0], playerData.position[1]);
        player.moveState = (PlayerMovement.MoveState) playerData.moveState;
        if (player.moveState == PlayerMovement.MoveState.Water)
        {
            player.EnterWater();
        }
        else if (player.moveState == PlayerMovement.MoveState.Wall)
        {
            player.BeginClimb(true);
        }
        PlayerMovement.abilities = playerData.abilities;
        player.breathLeftUnderwater = playerData.underwaterBreathLeft;
        PlayerMovement.lastPosBeforeSwimOrPit = new Vector2(playerData.lastPosBeforeSwimOrPit[0], playerData.lastPosBeforeSwimOrPit[1]);
        PlayerMovement.lastPosBeforeCaughtByEnemy = new Vector2(playerData.lastPosBeforeCaughtByEnemy[0], playerData.lastPosBeforeCaughtByEnemy[1]);
        PlayerMovement.lastPosBeforeFoodRunsOut = new Vector2(playerData.lastPosBeforeFoodRunsOut[0], playerData.lastPosBeforeFoodRunsOut[1]);
        PlayerMovement.lastPosBeforeThorns = new Vector2(playerData.lastPosBeforeThorns[0], playerData.lastPosBeforeThorns[1]);
        PlayerMovement.deathCount = playerData.deathCount;
        CollectBerries.berriesCollectedNum = playerData.berriesCollected;

        // UI.
        UISaveData uiData = SaveDataController.LoadUI(Application.persistentDataPath + path);
        UIController.time = uiData.time;
        UIController.timeTitle = uiData.timeTitle;
        UIController.numFood = uiData.berryCount;
        UIController.numDays = uiData.daysPassed;
        uiCont.timeDisplay.transform.Rotate(new Vector3(0f, 0f, uiData.sunRotation));
        UIController.timePassedSinceGameBegun = uiData.timePassed;

        Settings.useFullscreen = uiData.useFullscreen;
        Settings.useParticles = uiData.useParticles;
        Settings.usePostProcessing = uiData.usePostProcessing;
        MusicPlayer.SetVolume(0, uiData.musicVolume);
        MusicPlayer.SetVolume(1, uiData.musicVolume * 0.5f);
        SoundPlayer.SetVolume(uiData.sfxVolume);

        uiCont.foodNumDisplay.text = UIController.numFood.ToString();
        uiCont.timeDisplay.transform.rotation = Quaternion.Euler(0f, 0f, uiData.sunRotation);

        // Entities.
        for (int i = 0; i < entities.Length; ++i)
        {
            EntitySaveData entitySaveData = SaveDataController.LoadEntity(i, Application.persistentDataPath + path);
            entities[i].transform.position = new Vector2(entitySaveData.position[0], entitySaveData.position[1]);
            entities[i].isSatisfied = entitySaveData.isSatisified;
            entities[i].pickupNum = entitySaveData.itemCarried;

            if (entities[i].pickupNum != 0)
            {
                pickups[entities[i].pickupNum - 1].Grab(entities[i].gameObject);
            }
        }

        // Pickups.
        for (int i = 0; i < pickups.Length; ++i)
        {
            PickupSaveData pickupSaveData = SaveDataController.LoadPickup(i, Application.persistentDataPath + path);
            pickups[i].transform.position = new Vector2(pickupSaveData.position[0], pickupSaveData.position[1]);
        }

        // Interactables.
        for (int i = 0; i < interactables.Length; ++i)
        {
            InteractableSaveData interactableSaveData = SaveDataController.LoadInteractable(i, Application.persistentDataPath + path);
            interactables[i].canUse = interactableSaveData.canUse;
        }

        // Cutscenes.
        for (int i = 0; i < cutscenes.Length; ++i)
        {
            CutsceneSaveData cutsceneSaveData = SaveDataController.LoadCutscene(i, Application.persistentDataPath + path);
            cutscenes[i].SetActive(cutsceneSaveData.enabled);
        }

        // Tutorials.
        for (int i = 0; i < tutorials.Length; ++i)
        {
            TutorialSaveData tutorialData = SaveDataController.LoadTutorial(i, Application.persistentDataPath + path);
            tutorials[i].SetActive(!tutorialData.hasUsed);
        }
    }

    public static void SpawnEnemies()
    {
        nighttimeEnemies.ForEach(x =>
        {
            if (Vector2.Distance(x.transform.position, player.transform.position) > 10f)
            {
                x.SetActive(true);
            }
        });
    }

    public static void DespawnEnemies()
    {
        nighttimeEnemies.ForEach(x =>
        {
            x.SetActive(false);
        });
    }
}
