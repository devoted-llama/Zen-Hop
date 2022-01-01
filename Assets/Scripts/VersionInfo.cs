using UnityEngine;

[CreateAssetMenu(fileName = "VersionInfo", menuName = "ScriptableObjects/VersionInfo", order = 1)]
public class VersionInfo : ScriptableObject {
    public string version;
    public int buildNumber;
    public string dateTime;
}