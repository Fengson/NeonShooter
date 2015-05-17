using System;
using UnityEngine;

namespace NeonShooter.Utils
{
    public class PropertyInterpolator<T>
        where T : struct
    {
        Func<T> getProperty;
        Action<T> setProperty;
        InterpolationFunction interpolate;

        T sourceValue;
        T targetValue;

        public T TargetValue
        {
            get { return targetValue; }
            set
            {
                sourceValue = getProperty();
                targetValue = value;
                progress = 0;
            }
        }

        float progress;
        public float Progress
        {
            get { return progress; }
            set
            {
                if (value == progress) return;

                if (value < 0) value = 0;
                if (value > 1) value = 1;

                progress = value;

                if (progress == 0) setProperty(sourceValue);
                else if (progress == 1) setProperty(targetValue);
                else setProperty(interpolate(sourceValue, targetValue, progress));
            }
        }

        public PropertyInterpolator(Func<T> propertyGetter, Action<T> propertySetter,
            InterpolationFunction interpolationFunction)
        {
            getProperty = propertyGetter;
            setProperty = propertySetter;
            interpolate = interpolationFunction;
            TargetValue = getProperty();
        }

        public void Update(float dProgress)
        {
            if (Progress == 1) return;
            Progress = Mathf.Max(1, Progress + dProgress);
        }

        public delegate T InterpolationFunction(T from, T to, float progress);
    }
}
