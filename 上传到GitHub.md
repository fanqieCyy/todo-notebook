# 上传到 GitHub，让其他 Windows 电脑下载

建议采用“仓库存源码，Releases 放可运行压缩包”的方式。

## 1. 建立仓库

登录 GitHub，点击右上角加号 → New repository。名称可用 `todo-notebook`。希望任何人都能下载就选 Public；只供指定账号访问则选 Private。创建仓库。

## 2. 上传源代码

将发布材料中 `github-source` 文件夹里面的内容上传到仓库根目录：README.md、重新生成.ps1、source 文件夹、assets 文件夹和 .gitignore。使用仓库页面的 Add file → Upload files，上传后提交。

不要直接上传你正在用的整个安装目录：里面有个人待办、历史头像和旧程序。已准备的 github-source 使用明确的文件清单，不包含这些文件。

源代码公开和允许他人自由修改、再分发是不同的事情；如希望开源协作，可以后续选择并添加合适的 LICENSE。

## 3. 发布下载包

1. 打开仓库的 Releases，选择创建新 Release。
2. 新建标签，例如 `v1.0.0`，标题填“待办小本 v1.0.0”。
3. 写明功能和使用方式：Windows 桌面待办；下载 ZIP、解压、运行 TodoNotebook.exe；数据只存本机，不自动跨设备同步。
4. 将 `TodoNotebook-v1.0.0-Windows.zip` 作为附件上传。
5. 检查附件已上传，再点击 Publish release。

其他人应下载你上传的 Windows ZIP，不是 GitHub 自动生成的 Source code 压缩包。后者是源码，里面不一定有可直接运行的程序。

官方说明：https://docs.github.com/en/repositories/releasing-projects-on-github/managing-releases-in-a-repository

## 4. 其他电脑怎么用

打开 Release 页面 → 下载 Windows ZIP → 解压到有写入权限的文件夹 → 双击 TodoNotebook.exe。

当前是 Windows WinForms 程序，依赖 Windows .NET Framework；不是 macOS 或 Linux 应用。已在当前电脑生成并自检，尚未在其他电脑逐台验证。不需要安装 Node.js、Python 或开发工具来运行现成 EXE。

第一次添加事项后，程序会在旁边创建 data 文件夹。每台电脑保存各自的待办；上传到 GitHub 不会自动开启数据同步。可通过导出 XML 和恢复数据手动迁移。

程序目前没有数字签名。若目标电脑的安全机制拦截，应先确认下载来源和文件内容；不要关闭安全软件来解决。

## 5. 后续更新

修改代码后生成新程序，建立新的版本标签，例如 v1.0.1，并上传新的干净 ZIP。用户退出旧程序后替换程序文件，保留自己的 data 文件夹。不要把个人 tasks.xml 放进发布包。

发布前建议在另一台 Windows 电脑检查：启动、添加、重开后保留、到时提醒、随手记不提醒，以及小窗口显示。
