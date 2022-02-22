public interface ISettable<T> {
    public string SettingsKey { get; set; }
    public void RegisterSettings(T value);
}
