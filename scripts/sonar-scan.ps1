[CmdletBinding()]
param(
    [string]$ProjectKey = "familymanagementsystem",
    [string]$ProjectName = "FamilyManagementSystem",
    [string]$ServerUrl = "",
    [string]$Token = "",
    [string]$Solution = "FamilyManagementSystem.slnf",
    [switch]$SkipSonarTrigger
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($ServerUrl)) {
    $ServerUrl = if ($env:SONAR_HOST_URL) { $env:SONAR_HOST_URL } else { "http://localhost:9000" }
}

if ([string]::IsNullOrWhiteSpace($Token)) {
    $Token = $env:SONAR_TOKEN
    if ([string]::IsNullOrWhiteSpace($Token)) {
        $mcpConfigFile = "$env:USERPROFILE\.gemini\config\mcp_config.json"
        if (Test-Path $mcpConfigFile) {
            $mcpJson = Get-Content $mcpConfigFile -Raw | ConvertFrom-Json
            $tokenArg = $mcpJson.mcpServers.sonarqube.args | Where-Object { $_ -like "SONARQUBE_TOKEN=*" }
            if ($tokenArg) { $Token = $tokenArg -replace "^SONARQUBE_TOKEN=", "" }
        }
    }
}

if ([string]::IsNullOrWhiteSpace($Token)) {
    Write-Error "Sonar token not specified. Please provide -Token or set the SONAR_TOKEN environment variable."
    exit 1
}

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host " SonarQube Scanner for $ProjectName" -ForegroundColor Cyan
Write-Host " Server: $ServerUrl" -ForegroundColor Cyan
Write-Host " Project Key: $ProjectKey" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan

# 1. Verify dotnet-sonarscanner is available
if (-not (Get-Command dotnet-sonarscanner -ErrorAction SilentlyContinue)) {
    Write-Host "dotnet-sonarscanner tool not found in PATH. Checking global dotnet tools..." -ForegroundColor Yellow
    $dotnetToolsPath = "$env:USERPROFILE\.dotnet\tools"
    if (Test-Path "$dotnetToolsPath\dotnet-sonarscanner.exe") {
        $env:PATH = "$dotnetToolsPath;$env:PATH"
    } else {
        Write-Host "Installing dotnet-sonarscanner globally..." -ForegroundColor Green
        dotnet tool install --global dotnet-sonarscanner
    }
}

# 2. Begin SonarScanner
Write-Host "`n[1/4] Starting SonarScanner Begin step..." -ForegroundColor Green
dotnet-sonarscanner begin `
    /k:"$ProjectKey" `
    /n:"$ProjectName" `
    /d:sonar.host.url="$ServerUrl" `
    /d:sonar.token="$Token" `
    /d:sonar.cs.opencover.reportsPaths="**/TestResults/**/coverage.opencover.xml" `
    /d:sonar.coverage.exclusions="**/tests/**,**/*Tests/**,**/*Tests.cs,**/Migrations/**,**/Program.cs,**/AppHost.cs,**/DependencyInjection.cs,**/src/FamilyManagement.UI.Shared/Services/**,**/*.razor" `
    /d:sonar.exclusions="**/bin/**,**/obj/**,**/wwwroot/**,**/node_modules/**,**/.tools/**,**/Migrations/**,**/*.g.cs,**/*.Designer.cs" `
    /d:sonar.cpd.exclusions="**/Migrations/**,**/Program.cs,**/SqliteSchemaInitializer.cs"

if ($LASTEXITCODE -ne 0) {
    Write-Error "SonarScanner begin step failed."
    exit $LASTEXITCODE
}

# 3. Build Solution
Write-Host "`n[2/4] Building solution ($Solution)..." -ForegroundColor Green
dotnet build $Solution --configuration Release --no-incremental

if ($LASTEXITCODE -ne 0) {
    Write-Error "Build step failed."
    exit $LASTEXITCODE
}

# 4. Run Tests & Collect Coverage
Write-Host "`n[3/4] Running tests and collecting OpenCover coverage..." -ForegroundColor Green
dotnet test $Solution `
    --configuration Release `
    --no-build `
    /p:SkipAutoSonar=true `
    --collect:"XPlat Code Coverage;Format=opencover"

if ($LASTEXITCODE -ne 0) {
    Write-Error "Test execution failed."
    exit $LASTEXITCODE
}

# 5. End SonarScanner (Upload results to SonarQube)
Write-Host "`n[4/4] Finalizing and uploading analysis to SonarQube..." -ForegroundColor Green
dotnet-sonarscanner end /d:sonar.token="$Token"

if ($LASTEXITCODE -ne 0) {
    Write-Error "SonarScanner end step failed."
    exit $LASTEXITCODE
}

Write-Host "`n==========================================" -ForegroundColor Green
Write-Host " SonarQube Analysis Complete!" -ForegroundColor Green
Write-Host " Dashboard: $ServerUrl/dashboard?id=$ProjectKey" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Green
