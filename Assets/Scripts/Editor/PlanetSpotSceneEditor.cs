using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Planet))]
public class PlanetSpotSceneEditor : Editor
{
    private void OnSceneGUI()
    {
        Planet planet = (Planet)target;
        if (planet == null)
        {
            return;
        }

        if (planet.EnemySpots == null)
        {
            planet.EnemySpots = new List<Spot>();
        }

        if (planet.AllySpots == null)
        {
            planet.AllySpots = new List<Spot>();
        }

        if (planet.SatelliteSpots == null)
        {
            planet.SatelliteSpots = new List<Spot>();
        }

        if (Tools.current == Tool.Move || Tools.current == Tool.Transform)
        {
            foreach (Spot spot in planet.EnemySpots)
            {
                if (spot.SpotTransform == null)
                {
                    continue;
                }
                Undo.RecordObject(planet, "Moving spot");
                Vector3 newPosition = Handles.DoPositionHandle(spot.SpotTransform.position, Quaternion.identity);
                newPosition.y = planet.transform.position.y;
                spot.SpotTransform.position = (newPosition - planet.transform.position).normalized * planet.GetOrbitDistanceFromPlanet() + planet.transform.position;
                Handles.Label(spot.SpotTransform.position, spot.SpotTransform.name);
            }

            foreach (Spot spot in planet.AllySpots)
            {
                if (spot.SpotTransform == null)
                {
                    continue;
                }
                Undo.RecordObject(planet, "Moving spot");
                Vector3 newPosition = Handles.DoPositionHandle(spot.SpotTransform.position, Quaternion.identity);
                newPosition.y = planet.transform.position.y;
                spot.SpotTransform.position = (newPosition - planet.transform.position).normalized * planet.GetOrbitDistanceFromPlanet() + planet.transform.position;
                Handles.Label(spot.SpotTransform.position, spot.SpotTransform.name);
            }
        }

        foreach (Spot spot in planet.SatelliteSpots)
        {
            if (spot.SpotTransform == null)
            {
                continue;
            }
            if (Tools.current == Tool.Move || Tools.current == Tool.Transform)
            {
                Undo.RecordObject(planet, "Moving spot");
                Vector3 newPosition = Handles.DoPositionHandle(spot.SpotTransform.position, spot.SpotTransform.rotation);
                newPosition.y = planet.transform.position.y;
                spot.SpotTransform.position = (newPosition - planet.transform.position).normalized * planet.GetOrbitDistanceFromPlanet() + planet.transform.position;
            }
            if (Tools.current == Tool.Rotate || Tools.current == Tool.Transform)
            {
                Undo.RecordObject(planet, "Rotating spot");
                spot.SpotTransform.rotation = Handles.DoRotationHandle(spot.SpotTransform.rotation, spot.SpotTransform.position);
            }
            Handles.Label(spot.SpotTransform.position, spot.SpotTransform.name);
        }
    }
}