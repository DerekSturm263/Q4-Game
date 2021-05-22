using System.Collections.Generic;
using UnityEngine;

public class CollectBerries : Interactable
{
    private UIController uiCont;

    [SerializeField] private int numberOfBerries;
    private List<GameObject> berries = new List<GameObject>();

    private ParticleSystem particles;
    private ParticleSystem.EmissionModule particlesEmission;

    [SerializeField] private AudioClip berriesCollected;
    public static uint berriesCollectedNum = 0;

    private void Awake()
    {
        particles = GetComponentInChildren<ParticleSystem>();
        particlesEmission = particles.emission;
        uiCont = FindObjectOfType<UIController>();

        foreach (Transform berry in GetComponentInChildren<Transform>())
        {
            if (berry.GetComponent<ParticleSystem>() == null)
            {
                berries.Add(berry.gameObject);
            }
        }
    }

    private void Start()
    {
        if (!canUse)
        {
            foreach (GameObject berry in berries)
            {
                Destroy(berry.gameObject);
            }

            particlesEmission.rateOverTime = 0f;
        }
    }

    public override void Effect()
    {
        if (!canUse)
            return;

        Debug.Log("Berries Collected");
        foreach (GameObject berry in berries)
        {
            Destroy(berry.gameObject);
        }

        SoundPlayer.Play(berriesCollected);
        particlesEmission.rateOverTime = 0f;
        uiCont.GiveFood(numberOfBerries);
        berriesCollectedNum += 3;

        canUse = false;
    }
}
