using UnityEngine.Events;

public interface IChangeableSettingsElement<T> {
    public T Value { get; }
    public string SettingsKey { get; }
    public void SetValue(T value);
    public void AddListener(UnityAction<T> call);
}
