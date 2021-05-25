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
    public static AirBubble[] bubbles;
    public static GameObject[] cutscenes;
    public static GameObject[] tutorials;

    public static GameObject savingIndicator;

    public static float musicVolume;
    public static float musicVolume2;

    private float timeBetweenAmbientNoises = 2f;
    private float timeSinceLastAmbientNoise = 0f;

    [SerializeField] private AudioClip[] ambientSounds = new AudioClip[3];
    private AudioSource camAudio;

    [SerializeField] private bool ignoreSave = false;

    private void Awake()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        uiCont = FindObjectOfType<UIController>();
        entities = FindObjectsOfType<EntityAI>();
        pickups = FindObjectsOfType<Pickup>();
        interactables = FindObjectsOfType<Interactable>();
        bubbles = FindObjectsOfType<AirBubble>();
        cutscenes = GameObject.FindGameObjectsWithTag("Cutscene");
        tutorials = GameObject.FindGameObjectsWithTag("Tutorial");

        camAudio = cam.GetComponent<AudioSource>();

        savingIndicator = GameObject.FindGameObjectWithTag("Saving");
        savingIndicator.SetActive(false);

        if (!ignoreSave)
        {
            if (newGame)
            {
                UIController.numDays = 1;
                UIController.timePassedSinceGameBegun = 0f;
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
    }

    private void Update()
    {
        if (UIController.timeTitle == "night")
        {
            musicVolume = 0f;
            musicVolume2 = 0f;

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
            musicVolume = 0.5f;
            musicVolume2 = 0.3f;
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

        for (int i = 0; i < bubbles.Length; ++i)
        {
            SaveDataController.SaveBubble(i, Application.persistentDataPath + path);
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
        UIController.timePassedSinceGameBegun = uiData.timePassed;

        Settings.useFullscreen = uiData.useFullscreen;
        Settings.useParticles = uiData.useParticles;
        Settings.usePostProcessing = uiData.usePostProcessing;
        MusicPlayer.SetVolume(0, uiData.musicVolume);
        MusicPlayer.SetVolume(1, uiData.musicVolume * 0.6f);
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

        // Bubbles.
        for (int i = 0; i < bubbles.Length; ++i)
        {
            BubbleSaveData bubbleSaveData = SaveDataController.LoadBubble(i, Application.persistentDataPath + path);
            bubbles[i].timeSincePopped = bubbleSaveData.timeSincePopped;
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
}
