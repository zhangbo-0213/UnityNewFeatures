using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;


namespace Kata03 {
    public class CustomRenderPipeline : RenderPipeline
    {
        CommandBuffer _cb;

        //该函数在管线销毁时调用
        public override void Dispose()
        {
            base.Dispose();
            if (_cb != null) {
                _cb.Clear();
                _cb = null;
            }
        }

        //该函数在管线渲染时调用
        public override void Render(ScriptableRenderContext renderContext, Camera[] cameras)
        {
            base.Render(renderContext, cameras);

            if (_cb == null) {
                _cb = new CommandBuffer();
            }

            //设置Shader中要使用的光源变量名
            var _LightDir = Shader.PropertyToID("_LightDir");
            var _LightColor = Shader.PropertyToID("_LightColor");
            var _CameraPos = Shader.PropertyToID("_CameraPos");

            //对于每个相机执行的操作
            foreach (var camera in cameras)
            {
                //设置渲染上下文相机属性
                renderContext.SetupCameraProperties(camera);

                _cb.name = "Setup";
                //显式设置渲染目标为相机BackBuffer(如果相机没有指定渲染纹理，则直接绘制到屏幕)
                _cb.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
                //设置渲染目标颜色为相机背景色
                _cb.ClearRenderTarget(true, true, camera.backgroundColor);

                //设置相机的着色器全局变量
                Vector4 CameraPosition = new Vector4(camera.transform.localPosition.x, camera.transform.localPosition.y, camera.transform.localPosition.z, 1.0f);
                _cb.SetGlobalVector(_CameraPos, camera.transform.localToWorldMatrix * CameraPosition);
                renderContext.ExecuteCommandBuffer(_cb);
                _cb.Clear();

                //天空盒绘制
                renderContext.DrawSkybox(camera);

                //执行裁剪
                var culled = new CullResults();
                CullResults.Cull(camera, renderContext, out culled);

                /*
                 裁剪结果包括：
                    可见的物体列表:visibleRenderers
                    可见灯光列表:visibleLights
                    可见反射探针(CubeMap):visibleReflectionProbes
                 裁剪结果并未排序
                 */

                //获取所有灯光
                var lights = culled.visibleLights;
                _cb.name = "RenderLights";
                foreach (var light in lights)
                {
                    //挑选出平行光处理
                    if (light.lightType != LightType.Directional) continue;
                    //获取光源方向
                    Vector4 pos = light.localToWorld.GetColumn(0);
                    Vector4 lightDir = new Vector4(pos.x,pos.y,pos.z,0);
                    //获取光源颜色
                    Color lightColor = light.finalColor;
                    //构建shader常量缓存
                    _cb.SetGlobalVector(_LightDir,lightDir);
                    _cb.SetGlobalColor(_LightColor,lightColor);
                    renderContext.ExecuteCommandBuffer(_cb);
                    _cb.Clear();

                    var rs = new FilterRenderersSettings(true);
                    //只渲染固体范围
                    rs.renderQueueRange = RenderQueueRange.opaque;
                    //包括所有层
                    rs.layerMask = ~0;

                    //渲染设置，使用Shader中LightMode为"BaseLit"的Pass
                    var ds = new DrawRendererSettings(camera,new ShaderPassName("BaseLit"));
                    //物体绘制
                    renderContext.DrawRenderers(culled.visibleRenderers,ref ds,rs);

                    break;
                }

                //开始执行管线
                renderContext.Submit();
            }
        }
    }
}
