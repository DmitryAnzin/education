using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Timers : MonoBehaviour
{

    public class Timer
    {
        /// <summary>
        /// Выполняемая функция
        /// </summary>
        public Action<Timer> Func;
        /// <summary>
        /// Текущая задержка между выполнениями
        /// </summary>
        public float CurrentDelay;
        /// <summary>
        /// задержка между выполнениями
        /// </summary>
        public float Delay;
        /// <summary>
        /// Флаг показывающий что таймер выполняется не через фиксированный промежуток времени, а по фреймам
        /// </summary>
        public bool IsFrameTimer;
        /// <summary>
        /// Время прошедшее со старта
        /// </summary>
        public float TimeFromStart = 0;
        /// <summary>
        /// задержка перед началом выполнения
        /// </summary>
        public float StartDelayTime;
        /// <summary>
        /// По достижении которого таймер остановиться
        /// </summary>
        public float PlayTime;
        /// <summary>
        /// Нормализованное время проигрывания таймера
        /// </summary>
        public float NormalizedPlayTime
        {
            get { return Mathf.Min(1, (TimeFromStart - StartDelayTime) / PlayTime); }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="func">выполняемая функция в которую передаётся созданный таймер</param>
        /// <param name="delay">задержка со старта</param>
        /// <param name="isFrameTimer"> Флаг показывающий что таймер выполняется не через фиксированный промежуток времени, а по фреймам</param>
        public Timer(Action<Timer> func, float delay, bool isFrameTimer = false)
        {
            Func = func;
            CurrentDelay = delay;
            Delay = delay;
            IsFrameTimer = isFrameTimer;
            Self._timers.Add(this);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="func">выполняемая функция в которую передаётся созданный таймер</param>
        /// <param name="startDelayTime">задержка перед стартом таймера</param>
        /// <param name="playTime">По достижении которого таймер остановиться</param>
        /// <param name="delay">задержка между выполнениями</param>
        /// <param name="endAnimAction">функция выполняющаяся по завершении таймера</param>
        /// <param name="isFrameTimer">Флаг показывающий что таймер выполняется не через фиксированный промежуток времени, а по фреймам</param>
        public Timer(Action<Timer> func, float startDelayTime, float playTime, float delay = 0, Action endAnimAction = null, bool isFrameTimer = false)
        {
            StartDelayTime = startDelayTime;
            PlayTime = playTime;
            Func = (timer) =>
            {
                func(timer);
                if (NormalizedPlayTime == 1)
                {
                    timer.Stop();
                    if (endAnimAction != null)
                    {
                        endAnimAction();
                    }
                }
            };
            CurrentDelay = startDelayTime;
            Delay = delay;
            IsFrameTimer = isFrameTimer;
            Self._timers.Add(this);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="func">выполняемая функция</param>
        /// <param name="delay">задержка между выполнениями</param>
        /// <param name="isFrameTimer">Флаг показывающий что таймер выполняется не через фиксированный промежуток времени, а по фреймам</param>
        public Timer(Action func, float delay, bool isFrameTimer = false)
        {
            Func = (timer) => { func(); timer.Stop(); };
            CurrentDelay = delay;
            Delay = delay;
            IsFrameTimer = isFrameTimer;
            Self._timers.Add(this);
        }
        /// <summary>
        /// Остановка таймера
        /// </summary>
        public void Stop()
        {
            Self._timers.Remove(this);
        }

    }

    private static Timers _self;

    public static Timers Self
    {
        get
        {
            if (_self == null)
            {
                (new GameObject("Timer")).AddComponent<Timers>();
            }
            return _self;
        }
    }

    readonly List<Timer> _timers = new List<Timer>();

    void Awake()
    {
        _self = this;
    }

    void Update()
    {
        for (int i = 0; i < _timers.Count; i++)
        {
            var timer = _timers[i];
            if (timer.IsFrameTimer)
            {
                timer.CurrentDelay--;
                timer.TimeFromStart++;
            }
            else
            {
                timer.CurrentDelay -=  Time.deltaTime;
                timer.TimeFromStart += Time.deltaTime;
            }
            if (timer.CurrentDelay < 0)
            {
                timer.Func(timer);
                timer.CurrentDelay += timer.Delay;
            }
        }
    }
}
