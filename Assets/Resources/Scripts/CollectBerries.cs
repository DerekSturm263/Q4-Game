using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectBerries : Interactable
{
    private UIController uiCont;

    [SerializeField] private int numberOfBerries;
    private List<GameObject> berries = new List<GameObject>();

    private ParticleSystem particles;
    private ParticleSystem.EmissionModule particlesEmission;

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

    public override void Effect()
    {
        if (!canUse)
            return;

        Debug.Log("Berries Collected");
        foreach (GameObject berry in berries)
        {
            Destroy(berry.gameObject);
        }

        particlesEmission.rateOverTime = 0f;
        uiCont.GiveFood(numberOfBerries);

        canUse = false;
    }
}
