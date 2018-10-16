using UnityEngine.Experimental.Rendering;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
#endif

namespace Kata03 {
   
    public class CustomRenderPipelineAsset : RenderPipelineAsset
    {
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Render Pipeline/Kata03/Pipeline Asset")]
        static void CreateKata03Pipeline() {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,CreateInstance<CreateKata03PipelineAsset>(),"Kata03 Pipeline.asset",null,null);
        }
        class CreateKata03PipelineAsset : EndNameEditAction
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
