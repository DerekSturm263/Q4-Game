using System.Threading.Tasks;
using Types.Camera;
using Unity.Cinemachine;

public class CameraShakeController : Types.SingletonBehaviour<CameraShakeController>
{
    public async void Shake(ShakeSettingsAsset settings)
    {
        var noise = FindFirstObjectByType<CinemachineBasicMultiChannelPerlin>();

        if (noise)
        {
            noise.FrequencyGain = settings.Value.Frequency;
            noise.AmplitudeGain = settings.Value.Amplitude;

            await Task.Run(async () =>
            {
                await Task.Delay((int)(settings.Value.Time * 1000));

                noise.FrequencyGain = 0;
                noise.AmplitudeGain = 0;
            });
        }
    }

    public void StartShake(ShakeSettingsAsset settings)
    {
        var noise = FindFirstObjectByType<CinemachineBasicMultiChannelPerlin>();

        noise.FrequencyGain = settings.Value.Frequency;
        noise.AmplitudeGain = settings.Value.Amplitude;
    }

    public void EndShake()
    {
        var noise = FindFirstObjectByType<CinemachineBasicMultiChannelPerlin>();

        noise.FrequencyGain = 0;
        noise.AmplitudeGain = 0;
    }
}
