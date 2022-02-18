using UnityEngine.Events;


public interface IChangeableSettingsElement {
    public UnityEvent OnChange { get; }
    public dynamic Value { get; }
    public string SettingsKey { get; }
    public void SetValue(dynamic value);
}
