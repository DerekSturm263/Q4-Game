using System.Threading.Tasks;
using Types.Camera;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShakeController : Types.SingletonBehaviour<CameraShakeController>
{
    private float _frequency;
    private float _amplitude;

    public async void Shake(ShakeSettingsAsset settings)
    {
        _frequency = settings.Value.Frequency;
        _amplitude = settings.Value.Amplitude;

        var noise = FindFirstObjectByType<CinemachineBasicMultiChannelPerlin>();

        if (noise)
        {
            noise.FrequencyGain = _frequency;
            noise.AmplitudeGain = _amplitude;

            await Task.Run(async () =>
            {
                await Task.Delay((int)(settings.Value.Time * 1000));

                noise.FrequencyGain = 0;
                noise.AmplitudeGain = 0;
            });
        }
    }
}
