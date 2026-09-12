# Design: 上游同步、自动拉取 CI 与深色模式架构设计

## Architecture & Boundaries

```
                 +--------------------------+
                 |  EAappEmulater (App)     |
                 |  - Globals.Theme         |
                 |  - SettingView           |
                 +------------+-------------+
                              |
                              v
                 +--------------------------+
                 |  ModernWpf.Themes        |
                 |  - ThemeManager          |
                 |  - ThemeType Enum        |
                 |  - SystemEvents listener |
                 +------------+-------------+
                              |
              +---------------+---------------+
              |                               |
              v                               v
+---------------------------+   +---------------------------+
| Themes/Light.xaml         |   | Themes/Dark.xaml          |
| - ThemeBackgroundBrush    |   | - ThemeBackgroundBrush    |
| - ThemeForegroundBrush    |   | - ThemeForegroundBrush    |
| - ThemeCardBackgroundBrush|   | - ThemeCardBackgroundBrush|
| - ThemeBorderBrush        |   | - ThemeBorderBrush        |
| - ...                     |   | - ...                     |
+---------------------------+   +---------------------------+
              |                               |
              +---------------+---------------+
                              | DynamicResource
                              v
                 +--------------------------+
                 | ModernWpf Controls/Styles|
                 | - Window, WinButton      |
                 | - NavMenu, CardMenu      |
                 | - TextBox, ComboBox      |
                 | - ListBox, ListView      |
                 +--------------------------+
```

## Data Flow & Contracts

1. **配置流**:
   - `Config.ini`: `[Globals] Theme=System` (或 `Light`, `Dark`).
   - `Globals.Read()`: 读取 `Theme`，默认为 `System`。
   - `Globals.Write()`: 保存 `Theme` 变更。
2. **主题生效流**:
   - 启动时：`App.OnStartup` -> `ThemeManager.ApplyTheme(themeType)`。
   - 如果为 `ThemeType.System`，`ThemeManager` 查询 Windows 注册表 `HKCU\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize\AppsUseLightTheme`。
     - 若值为 `0` -> 实际加载 `Dark.xaml`。
     - 若值为 `1` (或不存在) -> 实际加载 `Light.xaml`。
   - 动态切换：`ThemeManager.ApplyTheme` 从 `Application.Current.Resources.MergedDictionaries` 移除旧主题字典，合并新主题字典。
   - 系统主题变化响应：注册 `SystemEvents.UserPreferenceChanged`。若当前模式为 `System`，系统主题变更时自动重新求值并刷新画刷。
3. **CI 同步流**:
   - 定时事件 (00:00 UTC) / 手动事件 (`workflow_dispatch`) -> GitHub Actions Runner -> 拉取 `CrazyZhang666/EAappEmulater:main` -> 合并到 `main` -> 推送 `origin main`。
