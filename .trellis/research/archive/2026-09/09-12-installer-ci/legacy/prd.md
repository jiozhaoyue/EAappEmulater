# Setup Inno Setup dual installer and integrate build/release CI

## Goal

Inherit upstream build and release CI, create Inno Setup dual-mode installer supporting both current user and system wide installation in a single EXE

## Requirements

1. 继承上游构建与发布 CI 工作流（`.github/workflows/dotnet-desktop.yml` 与 `release.yml`）。
2. 提供 Inno Setup 6+ 制作脚本 `installer/EAappEmulater.iss`。
3. 单一安装程序 EXE，原生支持双安装模式：
   - 为所有用户安装（系统级，安装至 `Program Files`，需要管理员权限）。
   - 仅为当前用户安装（用户级，安装至 `LocalAppData\Programs`，无需管理员权限）。
4. 安装向导提供桌面快捷方式（可选）、开始菜单目录、卸载项以及安装完成立即运行选项。
5. CI 产物更新：
   - `dotnet-desktop.yml`：上传便携版 `EADesktop.exe` 与安装版 `EAappEmulater-Setup.exe` 至 Artifact。
   - `release.yml`：Release 资产同时包含 `EADesktop.exe` 与 `EAappEmulater-Setup.exe`。

## Acceptance Criteria

- [x] 编写 `installer/EAappEmulater.iss` 并支持 `PrivilegesRequiredOverridesAllowed=commandline dialog`。
- [x] 更新 `.github/workflows/dotnet-desktop.yml`，在 CI 中安装 Inno Setup 并编译安装包，上传 Artifact。
- [x] 更新 `.github/workflows/release.yml`，在 Tag 发布与手动触发时编译安装包并附到 Release。
- [x] 本地验证 `dotnet publish` 输出路径与 Inno Setup 引用一致。

## Notes

- Keep `prd.md` focused on requirements, constraints, and acceptance criteria.
- Lightweight tasks can remain PRD-only.
- For complex tasks, add `design.md` for technical design and `implement.md` for execution planning before `task.py start`.
