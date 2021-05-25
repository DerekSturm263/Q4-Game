using UnityEngine;

public class AbilityUnlock : Interactable
{
    private Animator anim;

    public static Transform player;
    [SerializeField] private byte ability;

    private ParticleSystem particles;
    private ParticleSystem.EmissionModule particlesEmission;
    private ParticleSystem.VelocityOverLifetimeModule particlesVelocity;

    private Vector2 targetPos;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        anim.enabled = false;

        player = FindObjectOfType<PlayerMovement>().transform;

        particles = GetComponentInChildren<ParticleSystem>();
        particlesEmission = particles.emission;
        particlesVelocity = particles.velocityOverLifetime;

        targetPos = particles.transform.position;
    }

    private void Start()
    {
        if (!canUse)
        {
            particlesEmission.rateOverTime = 0f;
        }
    }

    private void Update()
    {
        particles.transform.position = Vector2.Lerp(particles.transform.position, targetPos, Time.deltaTime * 2.5f);
    }

    public override void Effect()
    {
        if (!canUse)
            return;

        player.GetComponent<PlayerMovement>().StartCutscene(gameObject);
        anim.enabled = true;

        PlayerMovement.Freeze();
        PlayerMovement.UnlockAbility(ability);

        canUse = false;
    }

    public void TargetPlayer()
    {
        targetPos = player.position;
    }

    public void Reset()
    {
        PlayerMovement.UnFreeze();
        player.GetComponent<PlayerMovement>().EndCutscene();
    }

    public void Unlock()
    {
        SoundPlayer.Play("new_ability");
        LoadTutorial.Display(AbilityTutorial.abilities[ability]);
    }
}
