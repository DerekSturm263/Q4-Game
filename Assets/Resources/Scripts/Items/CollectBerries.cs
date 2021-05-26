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
        CheckBerries();
    }

    public void CheckBerries()
    {
        if (!canUse)
        {
            foreach (GameObject berry in berries)
            {
                berry.gameObject.SetActive(false);
            }

            particlesEmission.rateOverTime = 0f;
        }
        else
        {
            foreach (GameObject berry in berries)
            {
                berry.gameObject.SetActive(true);
            }

            particlesEmission.rateOverTime = 10f;
        }
    }

    public override void Effect()
    {
        if (!canUse)
            return;

        Debug.Log("Berries Collected");
        foreach (GameObject berry in berries)
        {
            berry.SetActive(false);
        }

        SoundPlayer.Play(berriesCollected);
        particlesEmission.rateOverTime = 0f;
        uiCont.GiveFood(numberOfBerries);
        berriesCollectedNum += 3;

        canUse = false;
    }
}
