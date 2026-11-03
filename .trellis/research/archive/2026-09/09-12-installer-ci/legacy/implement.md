# Implement: Inno Setup 双模式安装包制作与 CI 流水线更新

## 实施清单

1. **Step 1: 创建 Inno Setup 安装包脚本** [DONE]
   - 编写 `installer/EAappEmulater.iss`
   - 配置 `PrivilegesRequired=lowest` 及 `PrivilegesRequiredOverridesAllowed=commandline dialog`
   - 配置多语言、快捷方式、启动与卸载支持

2. **Step 2: 更新 `dotnet-desktop.yml` 打包工作流** [DONE]
   - 保留原有单文件发布流程
   - 增加 Inno Setup 环境安装与安装包编译步骤
   - 上传安装版 Artifact（`EAappEmulater-Setup`）与便携版 Artifact（`EAappEmulater-win-x64`）

3. **Step 3: 更新 `release.yml` 发布工作流** [DONE]
   - 动态提取版本号或 Tag（自动去除 `v` 前缀注入 `/DMyAppVersion`）
   - 编译生成安装包
   - 同时将单文件版（`EADesktop.exe`）与安装版（`EAappEmulater-Setup.exe`）发布至 GitHub Release

4. **Step 4: 本地验证与质量自查** [DONE]
   - 校验路径与打包产物（本地 `dotnet publish` 输出结构已验证）
   - 运行 dotnet 检查并确认 0 错误编译通过
