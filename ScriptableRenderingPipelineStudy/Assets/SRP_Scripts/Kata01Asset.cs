using UnityEngine.Experimental.Rendering;

//在编辑器模式下，加载编辑器所需的资源操作
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
#endif

namespace Kata01 {
    public class CustomRenderPipelineAsset : RenderPipelineAsset
    {

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Render Pipeline/Kata01/Pipeline Asset")]
        static void CreateKata01Pipeline() {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,CreateInstance<CreateKata01PipelineAsset>(),
                "Kata01 Pipeline.asset",null,null);
        }

        class CreateKata01PipelineAsset : EndNameEditAction
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
