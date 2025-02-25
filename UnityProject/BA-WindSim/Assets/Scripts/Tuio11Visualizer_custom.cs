using System;
using System.Collections.Generic;
using TuioNet.Tuio11;
using TuioUnity.Common;
using UnityEngine;
using UnityEngine.Serialization;

namespace TuioUnity.Tuio11
{
    /// <summary>
    /// Basic example how to implement a simple visualisation of Tuio 1.1 objects by registering on the Add and Remove
    /// events to spawn and destroy UI elements for each type.
    /// </summary>
    public class Tuio11Visualizer_custom: MonoBehaviour
    {
        [SerializeField] private TuioSessionBehaviour _tuioSessionBehaviour;
        [SerializeField] private Tuio11ObjectTransform _objectPrefab;
        [SerializeField] private Tuio11ObjectTransform _objectPrefabWindMill;

        public WindReadback _windreadback;

        public int[] shapeBuilding = {0, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20};
        public int[] shapeWindMill = {1, 2, 3};

        private readonly Dictionary<uint, Tuio11Behaviour> _tuioBehaviours = new();

        private Tuio11Dispatcher _dispatcher;
        private Tuio11Dispatcher Dispatcher => (Tuio11Dispatcher)_tuioSessionBehaviour.TuioDispatcher;

        private void OnEnable()
        {
            try
            {
                Dispatcher.OnObjectAdd += AddTuioObject;
                Dispatcher.OnObjectRemove += RemoveTuioObject;
            }
            catch (InvalidCastException exception)
            {
                Debug.LogError($"[Tuio Client] Check the TUIO-Version on the TuioSession object. {exception.Message}");
            }
        }

        private void OnDisable()
        {
            try
            {
                Dispatcher.OnObjectAdd -= AddTuioObject;
                Dispatcher.OnObjectRemove -= RemoveTuioObject;
            }
            catch (InvalidCastException exception)
            {
                Debug.LogError($"[Tuio Client] Check the TUIO-Version on the TuioSession object. {exception.Message}");
            }
        }
        
        private void AddTuioObject(object sender, Tuio11Object tuioObject)
        {
            switch (tuioObject.SymbolId)
            {
            case var _ when Array.Exists(shapeBuilding, element => element == tuioObject.SymbolId):
                var objectBehaviour = Instantiate(_objectPrefab, transform);
                objectBehaviour.Initialize(tuioObject);
                _tuioBehaviours.Add(tuioObject.SymbolId, objectBehaviour);
                Debug.Log("Added TUIO Object ID: " + tuioObject.SymbolId + " as ShapeBuilding");
                break;
            case var _ when Array.Exists(shapeWindMill, element => element == tuioObject.SymbolId):
                var objectBehaviourWindMill = Instantiate(_objectPrefabWindMill, transform);
                objectBehaviourWindMill.Initialize(tuioObject);
                _tuioBehaviours.Add(tuioObject.SymbolId, objectBehaviourWindMill);
                Debug.Log("Added TUIO Object ID: " + tuioObject.SymbolId + " as WindMill");
                _windreadback.FillWindMillArray(objectBehaviourWindMill.gameObject, (int)tuioObject.SymbolId - 1);
                break;
            default:
                Debug.LogError("Missing TUIO Object ID: " + tuioObject.SessionId);
                break;
            }

        }
        
        private void RemoveTuioObject(object sender, Tuio11Object tuioObject)
        {
            if (_tuioBehaviours.Remove(tuioObject.SymbolId, out var objectBehaviour))
            {
                objectBehaviour.Destroy();
            }
        }
    }
}