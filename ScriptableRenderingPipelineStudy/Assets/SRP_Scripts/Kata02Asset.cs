using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Kata02 {
    public class CustomRenderPipelineAsset : RenderPipelineAsset
    {
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Render Pipeline/Kata02/Pipeline Asset")]
        static void CreateKata02Pipeline() {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,CreateInstance<CreateKata02PipelineAsset>(),
                "Kata02 Pipeline.asset",null,null);
        }

        class CreateKata02PipelineAsset : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                var instance = CreateInstance<CustomRenderPipelineAsset>();
                AssetDatabase.CreateAsset(instance,pathName);
            }
        }
#endif

        protected override IRenderPipeline InternalCreatePipeline()
        {
            return new CustomRenderPipeline();
        }
    }
}

