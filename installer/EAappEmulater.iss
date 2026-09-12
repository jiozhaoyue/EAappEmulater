; Inno Setup 6+ 脚本: EA App 模拟器 (EAappEmulater)
; 支持双模式安装：当前用户（免管理员权限）或系统全局（所有用户，需管理员权限）

#define MyAppName "EA App 模拟器"
#define MyAppExeName "EADesktop.exe"
#define MyAppPublisher "CrazyZhang666"
#define MyAppURL "https://github.com/CrazyZhang666/EAappEmulater"
#define MyAppId "{{A1155EFC-E1C6-4ABC-B91E-8A6A391490E3}"

#ifndef MyAppVersion
#define MyAppVersion "1.9.1.4"
#endif

[Setup]
; 应用基础信息
AppId={#MyAppId}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} v{#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}

; 双模式安装关键配置：
; 1. 默认权限设为最低 (lowest)，不强制要求管理员提权
; 2. 允许通过命令行及启动对话框让用户自主选择安装范围（为所有用户安装 / 仅为当前用户安装）
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=commandline dialog

; 默认安装目录：{autopf} 在系统级安装时解析为 Program Files，在用户级安装时解析为 %LOCALAPPDATA%\Programs
DefaultDirName={autopf}\EAappEmulater
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes

; 安装包产物输出配置
OutputDir=..\installer_output
OutputBaseFilename=EAappEmulater-Setup
SetupIconFile=..\EAappEmulater\Assets\Icons\Favicon.ico
UninstallDisplayIcon={app}\{#MyAppExeName}

; 压缩算法
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern
ArchitecturesInstallIn64BitMode=x64compatible

; 卸载与静默安装支持
UninstallFilesDir={app}

[Languages]
Name: "chinesesimplified"; MessagesFile: "compiler:Languages\ChineseSimplified.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"

[Files]
; 复制发布目录下的所有文件
Source: "..\publish\EAappEmulater-win-x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
; {autoprograms} 和 {autodesktop} 会根据安装模式自动路由到所有用户或当前用户的开始菜单/桌面
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
; 安装完成后提示运行应用
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
