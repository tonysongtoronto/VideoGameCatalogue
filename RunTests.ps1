# RunTests.ps1 - PowerShell script

# Set the test project directory name
$TestProjectName = "VideoGameCatalogue.Tests"

Write-Host "--- Starting automatic .NET test run ---"

# Check if the test project directory exists
if (Test-Path $TestProjectName) {
    # Navigate to the test project directory
    Set-Location $TestProjectName
    
    Write-Host "Entering directory: $($TestProjectName)"
    
    # Run dotnet test command
    # -c Debug: Run in Debug configuration (includes full debug symbols)
    # --logger "console;verbosity=normal": Set console output verbosity
    Write-Host "Executing dotnet test..."
    
    try {
        dotnet test -c Debug --logger "console;verbosity=normal"
        
        # Check the exit code of the last command
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ All tests passed successfully!" -ForegroundColor Green
        } else {
            Write-Host "❌ Some tests failed! Please review the detailed output." -ForegroundColor Red
        }
    } catch {
        Write-Host "An error occurred: Unable to run dotnet test." -ForegroundColor Red
        Write-Host $_
    }
    
    # Return to the original directory
    Set-Location ..
    
} else {
    Write-Host "❌ Error: Test project directory $($TestProjectName) not found." -ForegroundColor Red
}

Write-Host "--- Automatic test run completed ---"