using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class PlayableTime : BasicPlayableBehaviour {
    //动画曲线，通过曲线控制取值
    public AnimationCurve mCurve;
    //已播放时间
    private float havePlayedTime;
    //曲线总间隔时间
    private float allTime;

    //当playable停止播放时，将Game的时间播放速度重置为1
    public override void OnGraphStop(Playable playable)
    {
        Time.timeScale = 1;
    }

    //当playable播放时，获得该片段的完整时长
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        allTime = (float)PlayableExtensions.GetDuration(playable);
    }

    //在playable播放时的每一帧，获取累计已播放时间，除以总间隔，
    //对应到曲线的x轴，从曲线上取得对应播放速度值
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        havePlayedTime += info.deltaTime;
        Time.timeScale = mCurve.Evaluate(havePlayedTime/allTime);
    }
}
