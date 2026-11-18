param([string]$IconPath = '', [switch]$BuildOnly)
$ErrorActionPreference = 'Stop'
try {
    $root = $PSScriptRoot
    $compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
    if (!(Test-Path -LiteralPath $compiler)) { $compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe' }
    if (!(Test-Path -LiteralPath $compiler)) { throw '没有找到 Windows .NET Framework 编译器。' }
    $build = Join-Path $root 'build'
    New-Item -ItemType Directory -Path $build -Force | Out-Null
    $icon = Join-Path $root 'assets\app.ico'
    if (!$IconPath -and (Test-Path -LiteralPath (Join-Path $root 'assets\custom.png'))) { $IconPath = Join-Path $root 'assets\custom.png' }
    if ($IconPath) {
        if (![IO.Path]::IsPathRooted($IconPath)) { $IconPath = Join-Path $root $IconPath }
        if (!(Test-Path -LiteralPath $IconPath)) { throw "找不到图标文件：$IconPath" }
        if ([IO.Path]::GetExtension($IconPath) -ieq '.ico') { $icon = $IconPath }
        else {
            $packer = Join-Path $build 'IconPack.exe'
            & $compiler /nologo /target:exe "/out:$packer" /reference:System.Drawing.dll (Join-Path $root 'source\IconPack.cs')
            if ($LASTEXITCODE -ne 0) { throw '图标转换工具生成失败。' }
            $icon = Join-Path $build 'candidate.ico'
            & $packer $IconPath $icon
            if ($LASTEXITCODE -ne 0) { throw '图标转换失败，请使用有效的 PNG 图片或 ICO 图标。' }
        }
    }
    $candidate = Join-Path $build 'TodoNotebook.exe'
    & $compiler /nologo /target:winexe "/out:$candidate" "/win32icon:$icon" "/resource:$icon,app.ico" /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /reference:System.Xml.dll /reference:System.Core.dll (Join-Path $root 'source\Todo.cs')
    if ($LASTEXITCODE -ne 0) { throw '程序生成失败，原程序没有被替换。' }
    $check = Start-Process -FilePath $candidate -ArgumentList '--self-test' -Wait -PassThru -WindowStyle Hidden
    if ($check.ExitCode -ne 0) { throw "自检失败（$($check.ExitCode)），原程序没有被替换。" }
    if ($BuildOnly) { Write-Host "已生成并通过自检：$candidate。原程序和待办数据未修改。"; exit 0 }
    $installed = Join-Path $root 'TodoNotebook.exe'
    $running = Get-Process TodoNotebook -ErrorAction SilentlyContinue | Where-Object { $_.Path -eq $installed }
    if ($running) { throw '请先右键右下角托盘图标，选择“退出程序”，再重新运行脚本。关闭窗口不会退出程序。' }
    $backup = Join-Path $root ('backups\' + (Get-Date -Format 'yyyyMMdd-HHmmss-fff'))
    New-Item -ItemType Directory -Path $backup -Force | Out-Null
    if (Test-Path -LiteralPath $installed) { Copy-Item -LiteralPath $installed -Destination $backup }
    $activeIcon = Join-Path $root 'assets\app.ico'
    if (Test-Path -LiteralPath $activeIcon) { Copy-Item -LiteralPath $activeIcon -Destination $backup }
    Copy-Item -LiteralPath $candidate -Destination $installed -Force
    if ([IO.Path]::GetFullPath($icon) -ne [IO.Path]::GetFullPath($activeIcon)) { Copy-Item -LiteralPath $icon -Destination $activeIcon -Force }
    Write-Host "更新完成。旧程序备份：$backup"
    Write-Host '双击 TodoNotebook.exe 打开。待办数据没有改动。'
} catch { Write-Host "未完成：$($_.Exception.Message)" -ForegroundColor Red; exit 1 }
