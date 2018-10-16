using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Kata01 {
    public class CustomRenderPipeline : RenderPipeline
    {
        CommandBuffer _cb;
        //管线销毁时调用
        public override void Dispose()
        {
            base.Dispose();
            if (_cb != null) {
                _cb.Clear();
                _cb = null;
            }
        }
        //该函数在需要绘制管线时调用
        public override void Render(ScriptableRenderContext renderContext, Camera[] cameras)
        {
            base.Render(renderContext,cameras);

            if (_cb == null)
                _cb = new CommandBuffer();

            foreach (var camera in cameras)
            {
                //设置上下文相机属性(指定渲染目标为当前相机，如果该相机没有指定渲染纹理，则渲染结果将会进入backBuffer，即绘制到屏幕)
                renderContext.SetupCameraProperties(camera);
                //清除渲染目标的深度，颜色通道，并设置渲染目标为蓝色
                _cb.ClearRenderTarget(true,true,Color.gray);
                //提交指令队列至当前content处理
                renderContext.ExecuteCommandBuffer(_cb);
                //清除CommandBuffer指令队列（不会自动清理，需要手动清理）
                _cb.Clear();
                //开始执行上下文，指令提交执行渲染过程 
                renderContext.Submit();
            }
        }

    }
}

