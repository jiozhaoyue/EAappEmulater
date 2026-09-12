# Implement: 上游同步、自动拉取 CI 与深色模式实施清单

## 实施步骤清单

1. **Step 1: 上游代码同步** [DONE]
   - 暂存或提交当前未跟踪文件状态
   - `git merge --ff-only upstream/main`
   - `dotnet build EAappEmulater.sln` 验证编译通过

2. **Step 2: 创建 GitHub Actions CI 同步工作流** [DONE]
   - 新建 `.github/workflows/sync-upstream.yml`
   - 配置每日 schedule 与 workflow_dispatch

3. **Step 3: 实现 ModernWpf 深色主题基础设施** [DONE]
   - 新建 `ModernWpf/Themes/ThemeType.cs`
   - 新建 `ModernWpf/Themes/Light.xaml` 与 `ModernWpf/Themes/Dark.xaml`
   - 新建 `ModernWpf/Themes/ThemeManager.cs`（支持系统注册表探测与事件监听、DwmSetWindowAttribute 标题栏支持）
   - 修改 `ModernWpf/Themes/Generic.xaml` 默认引入 `Light.xaml`
   - 修改 `ModernWpf/Styles/*.xaml` 将硬编码色改为 `{DynamicResource ...}`

4. **Step 4: EAappEmulater 客户端业务对接** [DONE]
   - 修改 `EAappEmulater/Globals.cs` 增加 `Theme` 读写持久化（默认 `System`）
   - 修改 `EAappEmulater/App.xaml.cs` 启动时应用主题
   - 修改 `EAappEmulater/Windows/*.xaml` 边框、标题栏与侧栏动态画刷
   - 修改 `EAappEmulater/Views/SettingView.xaml` & `SettingView.xaml.cs` 添加主题下拉框
   - 修改 `EAappEmulater/Assets/Files/Lang/*.xaml` 补充 8 种语言的多语言翻译条目

5. **Step 5: 验证与编译** [DONE]
   - `dotnet build EAappEmulater.sln` 0 错误通过编译
   - 检查启动与切换逻辑及 Windows DWM 沉浸式标题栏联动
