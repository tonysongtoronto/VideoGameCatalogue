# RunTests.ps1 - PowerShell 脚本

# 设置测试项目目录名称
$TestProjectName = "VideoGameCatalogue.Tests"

Write-Host "--- 开始自动运行 .NET 测试 ---"

# 检查测试项目目录是否存在
if (Test-Path $TestProjectName) {
    # 导航到测试项目目录
    Set-Location $TestProjectName
    
    Write-Host "正在进入目录: $($TestProjectName)"
    
    # 运行 dotnet test 命令
    # -c Release: 以 Release 配置运行（通常更快）
    # --logger "console;verbosity=normal": 设置控制台输出级别
    Write-Host "正在执行 dotnet test..."
    
    try {
        dotnet test -c Debug --logger "console;verbosity=normal"
        
        # 检查上一个命令的退出代码
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ 所有测试成功通过！" -ForegroundColor Green
        } else {
            Write-Host "❌ 部分测试失败！请查看详细输出。" -ForegroundColor Red
        }
    } catch {
        Write-Host "发生错误：无法运行 dotnet test。" -ForegroundColor Red
        Write-Host $_
    }
    
    # 返回到脚本开始时的目录
    Set-Location ..
    
} else {
    Write-Host "❌ 错误：未找到测试项目目录 $($TestProjectName)" -ForegroundColor Red
}

Write-Host "--- 自动测试运行结束 ---"