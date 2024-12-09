using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace Core.Particles
{
    public class ColorGradient
    {
        private List<(float time, Vector4 color)> _colors = new List<(float, Vector4)>();

        public void AddColor(float time, Vector4 color)
        {
            _colors.Add((time, color));
            _colors.Sort((a, b) => a.time.CompareTo(b.time));
        }

        public Vector4 GetColor(float t)
        {
            if (_colors.Count == 0)
                return Vector4.One;

            if (t <= _colors[0].time)
                return _colors[0].color;

            if (t >= _colors[_colors.Count - 1].time)
                return _colors[_colors.Count - 1].color;

            for (int i = 0; i < _colors.Count - 1; i++)
            {
                if (t >= _colors[i].time && t <= _colors[i + 1].time)
                {
                    float lerpFactor = (t - _colors[i].time) / (_colors[i + 1].time - _colors[i].time);
                    return Vector4.Lerp(_colors[i].color, _colors[i + 1].color, lerpFactor);
                }
            }

            return Vector4.One;
        }

        public List<(float time, Vector4 color)> GetColors()
        {
            return _colors;
        }
    }
}