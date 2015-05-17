using System;

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
                Progress = 0;
            }
        }

        public float Progress { get; private set; }

        public PropertyInterpolator(Func<T> propertyGetter, Action<T> propertySetter,
            InterpolationFunction interpolationFunction)
        {
            getProperty = propertyGetter;
            setProperty = propertySetter;
            interpolate = interpolationFunction;
            TargetValue = getProperty();
        }

        public void UpdateForward(float dProgress)
        {
            Progress += dProgress;
            if (Progress > 1) Progress = 1;
            Update(Progress);
        }

        public void Update(float Progress)
        {
            setProperty(interpolate(sourceValue, targetValue, Progress));
        }

        public delegate T InterpolationFunction(T from, T to, float progress);
    }
}
