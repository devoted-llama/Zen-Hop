public interface ISettable {
    public string SettingsKey { get; set; }
    public void RegisterSettings(dynamic state);
}
