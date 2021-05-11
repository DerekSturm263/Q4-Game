using UnityEngine;

public class GameController : MonoBehaviour
{
    private AudioSource audioSrc;

    [SerializeField] private int saveTime = 120;

    public static Camera cam;
    public static PlayerMovement player;
    public static UIController uiCont;
    public static EntityAI[] entities;
    public static Pickup[] pickups;
    public static Interactable[] interactables;
    public static AirBubble[] bubbles;

    public static GameObject savingIndicator;

    public static float musicVolume;
    public static float musicVolume2;

    private float timeBetweenAmbientNoises = 2f;
    private float timeSinceLastAmbientNoise = 0f;

    [SerializeField] private AudioClip[] ambientSounds = new AudioClip[3];

    private void Awake()
    {
        audioSrc = GetComponent<AudioSource>();

        DontDestroyOnLoad(gameObject);

        if (!MusicPlayer.Exists())
        {
            MusicPlayer.Initialize();
            SoundPlayer.Initialize();

            MusicPlayer.Play(0, "Lost in the Woods (Main Theme)");
            MusicPlayer.Play(1, "Lost in the Ocean (Underwater Theme)");
            MusicPlayer.SetVolume(0, 0.5f);
            MusicPlayer.SetVolume(1, 0f);

            SoundPlayer.SetVolume(0.5f);

            musicVolume = MusicPlayer.volume[0];
            musicVolume2 = MusicPlayer.volume[0] * 0.66f;
        }

        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        uiCont = FindObjectOfType<UIController>();
        entities = FindObjectsOfType<EntityAI>();
        pickups = FindObjectsOfType<Pickup>();
        interactables = FindObjectsOfType<Interactable>();
        bubbles = FindObjectsOfType<AirBubble>();

        savingIndicator = GameObject.FindGameObjectWithTag("Saving");
        savingIndicator.SetActive(false);

        if (SaveDataController.HasSave())
        {
            LoadGame();
        }
        else
        {
            SaveGame();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U) || ((int) Time.realtimeSinceStartup % saveTime == 0 && !savingIndicator.activeSelf))
        {
            SaveGame();
        }

        if (UIController.timeTitle == "night")
        {
            musicVolume = Mathf.Lerp(musicVolume, 0f, Time.deltaTime);
            musicVolume2 = Mathf.Lerp(musicVolume2, 0f, Time.deltaTime);

            timeSinceLastAmbientNoise += Time.deltaTime;

            if (timeSinceLastAmbientNoise >= timeBetweenAmbientNoises)
            {
                timeSinceLastAmbientNoise = 0f;
                timeBetweenAmbientNoises = Random.Range(5f, 10f);

                PlaySound(ambientSounds, true);
            }
        }
        else
        {
            musicVolume = Mathf.Lerp(musicVolume, 0.5f, Time.deltaTime);
            musicVolume2 = Mathf.Lerp(musicVolume2, 0.33f, Time.deltaTime);
        }
    }

    public void PlaySound(AudioClip[] sound, bool interuptSound = false, float pitch = 1f, float volume = 1f)
    {
        if (audioSrc.isPlaying && !interuptSound)
            return;

        audioSrc.clip = sound[Random.Range(0, sound.Length)];
        audioSrc.pitch = pitch;
        audioSrc.volume = volume;
        audioSrc.Play();
    }

    private static void SaveGame()
    {
        savingIndicator.SetActive(true);

        SaveDataController.SaveCamera();
        SaveDataController.SavePlayer();
        SaveDataController.SaveUI();

        for (int i = 0; i < entities.Length; ++i)
        {
            SaveDataController.SaveEntity(i);
        }

        for (int i = 0; i < pickups.Length; ++i)
        {
            SaveDataController.SavePickup(i);
        }

        for (int i = 0; i < interactables.Length; ++i)
        {
            SaveDataController.SaveInteractable(i);
        }

        for (int i = 0; i < bubbles.Length; ++i)
        {
            SaveDataController.SaveBubble(i);
        }
    }

    private static void LoadGame()
    {
        // Camera.
        CameraSaveData camData = SaveDataController.LoadCamera();
        cam.transform.position = new Vector3(camData.position[0], camData.position[1], camData.position[2]);

        // Player.
        PlayerSaveData playerData = SaveDataController.LoadPlayer();
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

        // UI.
        UISaveData uiData = SaveDataController.LoadUI();
        UIController.time = uiData.time;
        UIController.numFood = uiData.berryCount;

        // Entities.
        for (int i = 0; i < entities.Length; ++i)
        {
            EntitySaveData entitySaveData = SaveDataController.LoadEntity(i);
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
            PickupSaveData pickupSaveData = SaveDataController.LoadPickup(i);
            pickups[i].transform.position = new Vector2(pickupSaveData.position[0], pickupSaveData.position[1]);
        }

        // Interactables.
        for (int i = 0; i < interactables.Length; ++i)
        {
            InteractableSaveData interactableSaveData = SaveDataController.LoadInteractable(i);
            interactables[i].canUse = interactableSaveData.canUse;
        }

        // Bubbles.
        for (int i = 0; i < bubbles.Length; ++i)
        {
            BubbleSaveData bubbleSaveData = SaveDataController.LoadBubble(i);
            bubbles[i].timeSincePopped = bubbleSaveData.timeSincePopped;
        }
    }
}
