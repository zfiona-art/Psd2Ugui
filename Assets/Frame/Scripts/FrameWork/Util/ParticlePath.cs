using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
[DisallowMultipleComponent]
[RequireComponent(typeof(ParticleSystem))]
public class ParticlePath : MonoBehaviour
{
    public float BeizerControlRatio = 1.1f;             //二阶贝塞尔曲线控制比例
    public float DestroyDistance = 0.05f;               //粒子销毁距离
    public float CheckPosDistance = 0.1f;
    public float DelayMoveTime = 0.2f;                  //延时移动时间
    public AnimationCurve speedCurve;                   //速度曲线

    private Vector3 _targetPos;                          //粒子移动轨迹目标坐标点
    private ParticleSystem.Particle[] _particles;
    private ParticleSystem _ps;
    private ParticleSystem.MainModule _psmm;
    private int _particleCount = 0;                      //粒子数目

    private Action _onParticlePathFinished;
    public Action<int> OnParticleArriveTarget;             //当粒子到达目标位置

    private Dictionary<int, Vector3> _beizerControlPosDic;

    private bool _particleSystemStoped = false;
    private bool isInited = false;
    public void Init()
    {
        if (isInited) return;
        isInited = true;
        if (null == _ps)
        {
            _ps = GetComponent<ParticleSystem>();
        }
        if (null == _ps)
        {
            return;
        }
        _psmm = _ps.main;
        _psmm.simulationSpace = ParticleSystemSimulationSpace.World;
        _psmm.loop = false;
        _beizerControlPosDic = new Dictionary<int, Vector3>();
    }

    private void Awake()
    {
        Init();

    }

    private void LateUpdate()
    {
        if (null == _ps)
            return;
        if (_ps.isStopped && _particleSystemStoped == false)
        {
            _particleSystemStoped = true;
            if (null != _onParticlePathFinished)
                _onParticlePathFinished();
            return;
        }
        InitializeIfNeeded();
        _particleCount = _ps.GetParticles(_particles);
        if (_particleCount <= 0)
            return;
        float proportion = 0;
        int destroyParticles = 0;
        for (int i = 0; i < _particleCount; ++i)
        {
            if (_particles[i].startLifetime != 0 && CanMove(_particles[i]))
            {
                if (!IsDestroy(_particles[i]))
                {
                    //计算粒子已消耗生命时间
                    proportion = (_particles[i].startLifetime - _particles[i].remainingLifetime - DelayMoveTime) / _particles[i].startLifetime;
                    _particles[i].position = Vector3.Lerp(_particles[i].position, _targetPos, proportion);
                }
                else
                {
                    _particles[i].remainingLifetime = -1;
                    destroyParticles++;
                }
            }
        }
        _ps.SetParticles(_particles, _particleCount);
        if (destroyParticles > 0 && null != OnParticleArriveTarget)
        {
            OnParticleArriveTarget(destroyParticles);
        }
    }

    /// <summary>
    /// 是否销毁粒子
    /// </summary>
    /// <param name="particle"></param>
    /// <returns>销毁，返回true，否则返回false</returns>
    private bool IsDestroy(ParticleSystem.Particle particle)
    {
        bool checkDis = Vector3.Distance(particle.position, _targetPos) >= DestroyDistance;  //true:不销毁
        bool needCheckPos = Vector3.Distance(particle.position, _targetPos) < CheckPosDistance;
        bool checkPos = true;
        if (needCheckPos)
        {
            Vector3 v1 = particle.position - _targetPos;
            Vector3 v2 = _targetPos - transform.position;
            checkPos = v1.x <= 0 ? v2.x > 0 : v2.x <= 0;
        }
        return !(checkDis && checkPos);
    }

    private bool CanMove(ParticleSystem.Particle particle)
    {
        return particle.startLifetime - particle.remainingLifetime >= DelayMoveTime;
    }

    private void InitializeIfNeeded()
    {
        if (null == _ps)
            return;
        if (_particles == null || _particles.Length < _ps.main.maxParticles)
        {
            _particles = new ParticleSystem.Particle[_ps.main.maxParticles];
        }
    }

    //开始粒子轨迹运动
    //taretPos:轨迹运动目标坐标
    //callback:粒子轨迹运动完回调
    public void BeginParticlePath(Vector3 taretPos, Action callback = null)
    {
        if (!isInited) Init();
        _targetPos = taretPos;
        if (null != callback)
            _onParticlePathFinished = callback;
    }

    public void SetParticleMaxNum(int num)
    {
        if (!isInited) Init();
        _psmm.maxParticles = Math.Max(num, 0);
    }
}
