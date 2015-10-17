using System;
using UnityEngine;

namespace NeonShooter.Utils
{
    public abstract class PropertyInterpolator
    {
        public static PropertyInterpolator<Vector3>.InterpolationFunction Vector3Lerp =
            (v1, v2, p) => Vector3.Lerp(v1, v2, p);
        public static PropertyInterpolator<Vector2>.InterpolationFunction Vector2LerpAngle =
            (v1, v2, p) => new Vector2(Mathf.LerpAngle(v1.x, v2.x, p), Mathf.LerpAngle(v1.y, v2.y, p));
        public static PropertyInterpolator<Quaternion>.InterpolationFunction QuaternionLerp =
            (q1, q2, p) => Quaternion.Lerp(q1, q2, p);

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

                UpdatePropertyValue();
            }
        }

        protected PropertyInterpolator()
        {
        }

        protected abstract void UpdatePropertyValue();

        public void Update(float dProgress)
        {
            if (Progress == 1) return;
            Progress = Progress + dProgress;
        }

        protected void SetProgressNoUpdate(float progress)
        {
            this.progress = progress;
        }
    }

    public class PropertyInterpolator<T> : PropertyInterpolator
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
                SetProgressNoUpdate(0);
            }
        }

        protected override void UpdatePropertyValue()
        {
            if (Progress == 0) setProperty(sourceValue);
            else if (Progress == 1) setProperty(targetValue);
            else setProperty(interpolate(sourceValue, targetValue, Progress));
        }

        public PropertyInterpolator(Func<T> propertyGetter, Action<T> propertySetter,
            InterpolationFunction interpolationFunction)
        {
            getProperty = propertyGetter;
            setProperty = propertySetter;
            interpolate = interpolationFunction;
            TargetValue = getProperty();
        }

        public delegate T InterpolationFunction(T from, T to, float progress);
    }
}
