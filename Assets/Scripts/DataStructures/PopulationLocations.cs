
using System.Collections.Generic;
using UnityEngine;

namespace DataStructures
{

    public class PopulationLocations :  Dictionary<int, Vector2>
    {
        public Vector2 Max => new Vector2(_maxX, _maxY);
        public Vector2 Min => new Vector2(_minX, _minY);

        private float _maxX;
        private float _maxY;
                           
        private float _minX;
        private float _minY;
        public PopulationLocations()
        {
            _maxX = float.MinValue;
            _maxY = float.MinValue;

            _minX = float.MaxValue;
            _minY = float.MaxValue;
        }

        public void Add(int populationId, Vector2 location)
        {
            _maxX = location.x > _maxX ? location.x : _maxX;
            _maxY = location.y > _maxY ? location.y : _maxY;

            _minX = location.x < _minX ? location.x : _minX;
            _minY = location.y < _minY ? location.y : _minY;

            base.Add(populationId, location);
        }

    }
}
