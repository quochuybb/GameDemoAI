using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AudioPing
{
    public Vector3 position;
    public float radius;
    
    public AudioPing(Vector3 pos, float rad)
    {
        position = pos;
        radius = rad;
    }
}

[CreateAssetMenu(menuName = "AI/Audio Ping Set", fileName = "NewAudioPingSet")]
public class AudioPingSet : ScriptableObject
{
    private readonly List<AudioPing> _pings = new List<AudioPing>();
    public List<AudioPing> Pings => _pings;

    public void AddPing(Vector3 position, float radius)
    {
        _pings.Add(new AudioPing(position, radius));
    }

    public void Clear()
    {
        _pings.Clear();
    }
}