# Design: Inno Setup 双模式安装包与 CI 集成方案

## 架构设计

### 1. Inno Setup 双模式核心机制
Inno Setup 6 引入了对用户级（Non-Administrative）安装的原生双模式支持：
```ini
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=commandline dialog
```
- 当用户启动安装包时，默认弹出模式选择框：
  1. 为所有用户安装（Install for all users） -> 触发 UAC 提权，目标目录解析为 `{autopf}` = `C:\Program Files\EAappEmulater`，快捷方式写入公共位置。
  2. 仅为当前用户安装（Install for me only） -> 无需提权，目标目录解析为 `{autopf}` = `C:\Users\<User>\AppData\Local\Programs\EAappEmulater`，快捷方式写入当前用户专属目录。

### 2. 构建产物映射
- 应用源码发布为单文件 EXE：`publish/EAappEmulater-win-x64/EADesktop.exe`。
- 安装包脚本位置：`installer/EAappEmulater.iss`。
- 安装包输出位置：`installer_output/EAappEmulater-Setup.exe`。
- 图标路径：`EAappEmulater/Assets/Icons/Favicon.ico`。

### 3. CI 流程集成
在 GitHub Actions `windows-latest` 虚拟机中：
- 原生包含 Chocolatey，可通过 `choco install innosetup --no-progress` 安装 Inno Setup 6。
- 编译命令：`& "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" /DMyAppVersion="$version" installer/EAappEmulater.iss`。
- 在 `dotnet-desktop.yml` 中将生成的安装包作为 Artifact 上传。
- 在 `release.yml` 中将便携版与安装版两份资产共同附至 GitHub Release。
