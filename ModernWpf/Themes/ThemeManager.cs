using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Microsoft.Win32;

namespace ModernWpf.Themes;

public static class ThemeManager
{
    private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    public static ThemeType CurrentTheme { get; private set; } = ThemeType.System;
    public static ThemeType ActualTheme { get; private set; } = ThemeType.Light;

    public static event EventHandler<ThemeType> ThemeChanged;

    static ThemeManager()
    {
        try
        {
            SystemEvents.UserPreferenceChanged += (sender, args) =>
            {
                if (CurrentTheme == ThemeType.System)
                {
                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        ApplyTheme(ThemeType.System);
                    });
                }
            };
        }
        catch { }
    }

    /// <summary>
    /// 检测系统当前是否为深色模式
    /// </summary>
    public static bool IsSystemDarkTheme()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            if (key?.GetValue("AppsUseLightTheme") is int useLightTheme)
            {
                return useLightTheme == 0;
            }
        }
        catch { }

        return false;
    }

    /// <summary>
    /// 应用指定的主题
    /// </summary>
    public static void ApplyTheme(ThemeType theme)
    {
        CurrentTheme = theme;
        ActualTheme = (theme == ThemeType.System)
            ? (IsSystemDarkTheme() ? ThemeType.Dark : ThemeType.Light)
            : theme;

        var app = Application.Current;
        if (app == null) return;

        string themeDictUri = ActualTheme == ThemeType.Dark
            ? "/ModernWpf;component/Themes/Dark.xaml"
            : "/ModernWpf;component/Themes/Light.xaml";

        var newDict = new ResourceDictionary
        {
            Source = new Uri(themeDictUri, UriKind.RelativeOrAbsolute)
        };

        // 移除 Generic.xaml 中内嵌的默认主题字典（若有），避免覆盖新主题
        var genericDict = app.Resources.MergedDictionaries
            .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("Generic.xaml"));
        if (genericDict != null)
        {
            var childTheme = genericDict.MergedDictionaries
                .FirstOrDefault(d => d.Source != null &&
                    (d.Source.OriginalString.EndsWith("Themes/Light.xaml") ||
                     d.Source.OriginalString.EndsWith("Themes/Dark.xaml")));
            if (childTheme != null)
            {
                genericDict.MergedDictionaries.Remove(childTheme);
            }
        }

        // 查找在顶级 MergedDictionaries 中的现存主题字典
        var existingDict = app.Resources.MergedDictionaries
            .FirstOrDefault(d => d.Source != null &&
                (d.Source.OriginalString.EndsWith("Themes/Light.xaml") ||
                 d.Source.OriginalString.EndsWith("Themes/Dark.xaml")));

        if (existingDict != null)
        {
            int index = app.Resources.MergedDictionaries.IndexOf(existingDict);
            app.Resources.MergedDictionaries[index] = newDict;
        }
        else
        {
            // 插入在 Generic.xaml 之后，使得主题画刷具有最高优先级
            int genericIndex = -1;
            for (int i = 0; i < app.Resources.MergedDictionaries.Count; i++)
            {
                if (app.Resources.MergedDictionaries[i].Source?.OriginalString?.Contains("Generic.xaml") == true)
                {
                    genericIndex = i;
                    break;
                }
            }
            if (genericIndex >= 0)
            {
                app.Resources.MergedDictionaries.Insert(genericIndex + 1, newDict);
            }
            else
            {
                app.Resources.MergedDictionaries.Add(newDict);
            }
        }

        // 更新当前所有打开窗口的沉浸式深色模式属性
        foreach (System.Windows.Window window in app.Windows)
        {
            SetWindowImmersiveDarkMode(window, ActualTheme == ThemeType.Dark);
        }

        ThemeChanged?.Invoke(null, ActualTheme);
    }

    /// <summary>
    /// 为指定窗口设置 Windows 原生沉浸式深色标题栏
    /// </summary>
    public static void SetWindowImmersiveDarkMode(System.Windows.Window window, bool isDark)
    {
        if (window == null) return;

        try
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero) return;

            int useImmersiveDarkMode = isDark ? 1 : 0;
            if (DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useImmersiveDarkMode, sizeof(int)) != 0)
            {
                DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1, ref useImmersiveDarkMode, sizeof(int));
            }
        }
        catch { }
    }
}
