using UnityEngine;
using UnityEngine.Rendering;

namespace MonoBehaviours
{
    [ExecuteInEditMode]
    public class CameraCuller : MonoBehaviour
    {
        private Camera _camera;

        [SerializeField] private Renderer[] _rndrs;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += RenderPipelineManager_beginCameraRendering;
            RenderPipelineManager.endCameraRendering += RenderPipelineManager_endCameraRendering;
        }

        private void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= RenderPipelineManager_beginCameraRendering;
            RenderPipelineManager.endCameraRendering -= RenderPipelineManager_endCameraRendering;
        }

        private void RenderPipelineManager_beginCameraRendering(ScriptableRenderContext arg1, Camera arg2)
        {
            if (arg2 != _camera)
                return;

            for (int i = 0; i < _rndrs.Length; ++i)
            {
                _rndrs[i].enabled = false;
            }
        }

        private void RenderPipelineManager_endCameraRendering(ScriptableRenderContext arg1, Camera arg2)
        {
            if (arg2 != _camera)
                return;

            for (int i = 0; i < _rndrs.Length; ++i)
            {
                _rndrs[i].enabled = true;
            }
        }
    }
}
