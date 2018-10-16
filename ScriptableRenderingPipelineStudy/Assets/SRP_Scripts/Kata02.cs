using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

namespace Kata02 {
    public class CustomRenderPipeline : RenderPipeline {
        //指令缓存对象
        CommandBuffer _cb;

        //在管线销毁时调用
        public override void Dispose()
        {
            base.Dispose();
            if (_cb != null) {
                _cb.Clear();
                _cb = null;
            }
        }

        //在管线渲染时调用
        public override void Render(ScriptableRenderContext renderContext, Camera[] cameras)
        {
            base.Render(renderContext, cameras);

            if (_cb == null) {
                _cb = new CommandBuffer();
            }

            //设置每一个相机
            foreach (var camera in cameras)
            {
                renderContext.SetupCameraProperties(camera);

                _cb.name = "Setup";
                //显式设置渲染目标为相机BackBuffer(如果相机没有指定渲染纹理，则直接绘制到屏幕)
                _cb.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
                //设置渲染目标初始颜色为相机背景颜色
                _cb.ClearRenderTarget(true,true,camera.backgroundColor);
                renderContext.ExecuteCommandBuffer(_cb);
                _cb.Clear();

                //绘制天空盒子,需要在初始清除后绘制
                renderContext.DrawSkybox(camera);

                //执行裁剪
                var culled = new CullResults();
                CullResults.Cull(camera,renderContext, out culled);

                //设置 Filtering Setting,初始化所有参数
                var fs = new FilterRenderersSettings(true);
                //设置只绘制不透明物体
                fs.renderQueueRange = RenderQueueRange.opaque;
                //设置绘制所有层
                fs.layerMask = ~0;

                //设置Rendering Setting
                //在构造的时候需要传入LightMode参数
                var rs = new DrawRendererSettings(camera,new ShaderPassName("Unlit"));
                //在绘制不透明物体可以借助Z-Buffer，因此不需要额外的排序
                rs.sorting.flags = SortFlags.None;

                //绘制物体
                renderContext.DrawRenderers(culled.visibleRenderers,ref rs,fs);

                //执行管线
                renderContext.Submit();
            }
        }
    }
}
