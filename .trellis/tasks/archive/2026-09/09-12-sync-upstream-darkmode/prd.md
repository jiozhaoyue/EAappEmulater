# PRD: 同步上游、配置自动拉取 CI 与深色模式支持 (默认系统主题)

## Goal
为 EAappEmulater 项目拉取上游最新提交、配置 GitHub Actions 自动拉取上游的 CI 工作流，并以最小侵入方式为软件添加深色模式支持（默认跟随系统主题，支持界面切换与持久化）。

## Confirmed Facts & Background
- 上游仓库：`https://github.com/CrazyZhang666/EAappEmulater.git`，分支 `main`。
- 本地仓库与上游无分叉提交，本地 `main` 纯快进落后于 `upstream/main` 2 个 commit（包含 Juno 登录重构与相关改动）。
- 项目基于 .NET 6.0-windows WPF 开发，界面核心控件位于 `ModernWpf` 本地项目类库。
- 当前界面画刷多直接硬编码在 `ModernWpf/Styles/` XAML 中，尚未支持深色调色板。

## Requirements
1. **上游同步**：
   - 快速合并 `upstream/main` 最新提交，解决或避免任何冲突。
   - 保证拉取最新代码后解决方案编译通过。
2. **CI 自动化拉取上游**：
   - 在 `.github/workflows/sync-upstream.yml` 中配置定时任务与手动触发。
   - 设定每日 UTC 00:00 执行，并支持 `workflow_dispatch`。
   - 自动获取上游最新分支并合并推送至 `main`。
3. **深色模式支持 (最小侵入)**：
   - 提取基础浅色画刷 (`Themes/Light.xaml`) 与深色画刷 (`Themes/Dark.xaml`)。
   - 采用 Win11/Fluent 现代深灰风格（背景 `#202020`，控件 `#2D2D2D`，文本 `#F0F0F0`，边框 `#3E3E42`）。
   - 在 `ModernWpf` 中实现 `ThemeManager`，检测 Windows 注册表 `AppsUseLightTheme`，支持跟随系统。
   - 默认设置为跟随系统主题（`System`）。
   - 在设置界面 (`SettingView`) 语言选择旁边增加主题选择下拉框（跟随系统、浅色、深色）。
   - 在 `Globals.cs` 与 `Config.ini` 中读取并保存主题设置。
   - 在多语言文件 (`zh-CN.xaml`, `en-US.xaml` 等) 中增加对应的多语言文本。

## Acceptance Criteria
- [ ] `git merge --ff-only upstream/main` 成功，本地 `main` 拥有上游最新提交。
- [ ] 存在 `.github/workflows/sync-upstream.yml`，具备 schedule 与 workflow_dispatch 触发能力。
- [ ] 解决方案 `dotnet build EAappEmulater.sln` 顺利编译通过（0 错误）。
- [ ] 应用首次启动时默认跟随系统主题；当系统为深色模式时自动显示深色主题，系统为浅色模式时自动显示浅色主题。
- [ ] 用户可在设置页面随时切换跟随系统/浅色/深色，界面平滑刷新无需重启。
- [ ] 用户选择的主题能够保存在 `Config.ini`，下次启动依然生效。

## Out of Scope
- 重写现有控件体系或更换第三方重型 UI 库（如完整的外部 MahApps 或外部 ModernWpfUI NuGet）。
- 修改游戏核心启动、反作弊、TCP 代理服务等底层业务逻辑。
