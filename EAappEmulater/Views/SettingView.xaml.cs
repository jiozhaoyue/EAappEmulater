using CommunityToolkit.Mvvm.Input;
using EAappEmulater.Helper;
using EAappEmulater.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace EAappEmulater.Views;

/// <summary>
/// SettingView.xaml 的交互逻辑
/// </summary>
public partial class SettingView : UserControl, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private ObservableCollection<LanguageEntry> _languageList = new();
    public ObservableCollection<LanguageEntry> LanguageList
    {
        get => _languageList;
        set { _languageList = value; OnPropertyChanged(nameof(LanguageList)); }
    }

    private string _currentLanguage = string.Empty;
    public string CurrentLanguage
    {
        get => _currentLanguage;
        set
        {
            if (_currentLanguage == value) return;
            _currentLanguage = value;
            OnPropertyChanged(nameof(CurrentLanguage));

            // Apply immediately when changed by the ComboBox
            if (!string.IsNullOrWhiteSpace(_currentLanguage))
            {
                try
                {
                    App.SetLanguage(_currentLanguage);
                    Globals.Language = _currentLanguage;
                    Globals.DefaultLanguage = _currentLanguage;
                    Globals.Write();
                    LoadThemes();
                }
                catch { }
            }
        }
    }

    private ObservableCollection<ThemeEntry> _themeList = new();
    public ObservableCollection<ThemeEntry> ThemeList
    {
        get => _themeList;
        set { _themeList = value; OnPropertyChanged(nameof(ThemeList)); }
    }

    private string _currentTheme = "System";
    public string CurrentTheme
    {
        get => _currentTheme;
        set
        {
            if (_currentTheme == value) return;
            _currentTheme = value;
            OnPropertyChanged(nameof(CurrentTheme));

            if (Enum.TryParse(_currentTheme, out ModernWpf.Themes.ThemeType themeType))
            {
                ModernWpf.Themes.ThemeManager.ApplyTheme(themeType);
                Globals.Theme = _currentTheme;
                Globals.Write();
            }
        }
    }

    public SettingView()
    {
        InitializeComponent();

        ToDoList();

        // load languages
        var langs = LanguageConfigHelper.GetLanguages();
        LanguageList = new ObservableCollection<LanguageEntry>(langs);

        // show current language
        CurrentLanguage = string.IsNullOrWhiteSpace(Globals.Language) ? (Globals.DefaultLanguage ?? "") : Globals.Language;
        if (string.IsNullOrWhiteSpace(CurrentLanguage) && LanguageList.Count > 0)
            CurrentLanguage = LanguageList[0].Code;

        // load themes
        LoadThemes();
        CurrentTheme = string.IsNullOrWhiteSpace(Globals.Theme) ? "System" : Globals.Theme;

        DataContext = this;
    }

    private void LoadThemes()
    {
        string systemName = Application.Current.TryFindResource("Views.SettingView.ThemeSystem") as string ?? "跟随系统 (System)";
        string lightName = Application.Current.TryFindResource("Views.SettingView.ThemeLight") as string ?? "浅色模式 (Light)";
        string darkName = Application.Current.TryFindResource("Views.SettingView.ThemeDark") as string ?? "深色模式 (Dark)";

        ThemeList = new ObservableCollection<ThemeEntry>
        {
            new ThemeEntry { Code = "System", Name = systemName },
            new ThemeEntry { Code = "Light", Name = lightName },
            new ThemeEntry { Code = "Dark", Name = darkName }
        };
    }

    private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private void ToDoList()
    {
        FormLabel_VersionInfo.Content = CoreUtil.VersionInfo.ToString();

        FormLabel_UserName.Content = CoreUtil.UserName;
        FormLabel_MachineName.Content = CoreUtil.MachineName;
        FormLabel_OSVersion.Content = CoreUtil.OSVersion;
        FormLabel_SystemDirectory.Content = CoreUtil.SystemDirectory;

        FormLabel_RuntimeVersion.Content = CoreUtil.RuntimeVersion;
        FormLabel_OSArchitecture.Content = CoreUtil.OSArchitecture;
        FormLabel_RuntimeIdentifier.Content = CoreUtil.RuntimeIdentifier;
    }

    /// <summary>
    /// 打开配置文件
    /// </summary>
    [RelayCommand]
    private void OpenConfigFolder()
    {
        ProcessHelper.OpenDirectory(CoreUtil.Dir_Default);
    }


    /// <summary>
    /// 切换语言 (保留为命令入口, 也会使用 CurrentLanguage 的 setter)
    /// </summary>
    [RelayCommand]
    private void ChangeLanguage()
    {
        if (!string.IsNullOrWhiteSpace(CurrentLanguage))
        {
            App.SetLanguage(CurrentLanguage);
            Globals.Language = CurrentLanguage;
            Globals.DefaultLanguage = CurrentLanguage;
            Globals.Write();
            LoadThemes();
        }
    }
}

public class ThemeEntry
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
