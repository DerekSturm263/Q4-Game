using UnityEngine;

public class Settings : MonoBehaviour
{
    public static bool useFullscreen = true;
    public static bool useParticles = true;
    public static bool usePostProcessing = true;

    public UnityEngine.Audio.AudioMixer mixer;

    public UnityEngine.UI.Toggle fullscreenToggle;
    public UnityEngine.UI.Toggle particlesToggle;
    public UnityEngine.UI.Toggle postProcessingToggle;

    public UnityEngine.UI.Slider musicSlider;
    public UnityEngine.UI.Slider sfxSlider;

    private ParticleSystem[] particles;
    private System.Collections.Generic.List<float> particleEmissionRates = new System.Collections.Generic.List<float>();

    private void Start()
    {
        particles = FindObjectsOfType<ParticleSystem>();

        for (int i = 0; i < particles.Length; ++i)
        {
            particleEmissionRates.Add(particles[i].emission.rateOverTime.constant);
        }

        Screen.fullScreen = useFullscreen;

        for (int i = 0; i < particles.Length; ++i)
        {
            ParticleSystem.EmissionModule emission = particles[i].emission;

            emission.rateOverTime = useParticles ? particleEmissionRates[i] : 0f;
        }

        if (Camera.main.GetComponent<UnityEngine.Rendering.Volume>() != null)
        {
            Camera.main.GetComponent<UnityEngine.Rendering.Volume>().enabled = usePostProcessing;
        }

        AdjustMusicVolume(MusicPlayer.volume[0]);
        AdjustSFXVolume(SoundPlayer.volume);

        fullscreenToggle.isOn = useFullscreen;
        particlesToggle.isOn = useParticles;
        postProcessingToggle.isOn = usePostProcessing;

        musicSlider.value = MusicPlayer.volume[0];
        sfxSlider.value = SoundPlayer.volume;
    }

    public void ToggleFullscreen(bool isOn)
    {
        SoundPlayer.Play("ui_select");

        useFullscreen = isOn;
        Screen.fullScreen = useFullscreen;
    }

    public void ToggleParticles(bool isOn)
    {
        SoundPlayer.Play("ui_select");

        useParticles = isOn;

        for (int i = 0; i < particles.Length; ++i)
        {
            ParticleSystem.EmissionModule emission = particles[i].emission;

            emission.rateOverTime = isOn ? particleEmissionRates[i] : 0f;
        }
    }   
    
    public void TogglePostProcessing(bool isOn)
    {
        SoundPlayer.Play("ui_select");

        usePostProcessing = isOn;

        if (Camera.main.GetComponent<UnityEngine.Rendering.Volume>() != null)
        {
            Camera.main.GetComponent<UnityEngine.Rendering.Volume>().enabled = usePostProcessing;
        }
    }

    public void AdjustMusicVolume(float newVolume)
    {
        GameController.musicScalar = Mathf.Lerp(0.5f, 1f, newVolume);

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Title")
        {
            MusicPlayer.SetVolume(0, newVolume);
        }
    }

    public void AdjustSFXVolume(float newVolume)
    {
        SoundPlayer.SetVolume(newVolume);
        mixer.SetFloat("Volume", Mathf.Lerp(-40f, 20f, newVolume));
    }
}
