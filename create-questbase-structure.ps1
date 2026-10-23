param(
    [string]$SolutionPath = "D:\Games\dev_game\QuestBase_platform",
    [string]$ApiPath = "D:\Games\dev_game\QuestBase_platform\QuestBase.Api",
    [string]$TestsPath = "D:\Games\dev_game\QuestBase_platform\QuestBase.Tests"
)

# =====================
# API STRUCTURE
# =====================

$apiFolders = @(
    "Features\Auth",
    "Features\Projects",
    "Features\Players",
    "Features\Saves",
    "Features\Inventory",
    "Features\Economy",
    "Features\Quests",

    "Domain\Users",
    "Domain\Projects",
    "Domain\Players",
    "Domain\Inventory",
    "Domain\Economy",
    "Domain\Quests",

    "Data\EntityConfigurations",
    "Data\Migrations",

    "Shared\Auth",
    "Shared\Exceptions",
    "Shared\Middleware",
    "Shared\Extensions",

    "Properties"
)

foreach ($folder in $apiFolders) {
    $path = Join-Path $ApiPath $folder
    New-Item -ItemType Directory -Force -Path $path | Out-Null
}

$apiFiles = @(
    "Features\Auth\AuthEndpoints.cs",
    "Features\Auth\AuthService.cs",
    "Features\Auth\JwtTokenService.cs",
    "Features\Auth\AuthDtos.cs",

    "Features\Projects\ProjectEndpoints.cs",
    "Features\Projects\ProjectService.cs",
    "Features\Projects\ProjectDtos.cs",
    "Features\Projects\ApiKeyGenerator.cs",

    "Features\Players\PlayerEndpoints.cs",
    "Features\Players\PlayerService.cs",
    "Features\Players\PlayerDtos.cs",

    "Features\Saves\SaveEndpoints.cs",
    "Features\Saves\SaveService.cs",
    "Features\Saves\SaveDtos.cs",

    "Features\Inventory\InventoryEndpoints.cs",
    "Features\Inventory\InventoryService.cs",
    "Features\Inventory\InventoryDtos.cs",

    "Features\Economy\EconomyEndpoints.cs",
    "Features\Economy\EconomyService.cs",
    "Features\Economy\EconomyDtos.cs",

    "Features\Quests\QuestEndpoints.cs",
    "Features\Quests\QuestService.cs",
    "Features\Quests\QuestEngine.cs",
    "Features\Quests\QuestDtos.cs",

    "Domain\Users\User.cs",
    "Domain\Projects\Project.cs",
    "Domain\Projects\ApiKey.cs",
    "Domain\Players\Player.cs",
    "Domain\Players\PlayerSave.cs",
    "Domain\Inventory\Item.cs",
    "Domain\Inventory\InventoryItem.cs",
    "Domain\Economy\Currency.cs",
    "Domain\Economy\PlayerCurrency.cs",
    "Domain\Quests\Quest.cs",
    "Domain\Quests\PlayerQuest.cs",

    "Data\QuestBaseDbContext.cs",
    "Data\EntityConfigurations\UserConfiguration.cs",
    "Data\EntityConfigurations\ProjectConfiguration.cs",
    "Data\EntityConfigurations\PlayerConfiguration.cs",
    "Data\EntityConfigurations\QuestConfiguration.cs",

    "Shared\Auth\ApiKeyAuthenticationHandler.cs",
    "Shared\Auth\CurrentProject.cs",
    "Shared\Auth\CurrentPlayer.cs",

    "Shared\Exceptions\NotFoundException.cs",
    "Shared\Exceptions\ConflictException.cs",
    "Shared\Exceptions\UnauthorizedException.cs",

    "Shared\Middleware\ExceptionHandlingMiddleware.cs",

    "Shared\Extensions\ServiceCollectionExtensions.cs",
    "Shared\Extensions\ClaimsPrincipalExtensions.cs"
)

foreach ($file in $apiFiles) {
    $path = Join-Path $ApiPath $file
    $directory = Split-Path $path -Parent

    if (-not (Test-Path $directory)) {
        New-Item -ItemType Directory -Force -Path $directory | Out-Null
    }

    if (-not (Test-Path $path)) {
        New-Item -ItemType File -Path $path | Out-Null
    }
}

# =====================
# TEST STRUCTURE
# =====================

$testFolders = @(
    "Auth",
    "Projects",
    "Players",
    "Saves",
    "Inventory",
    "Economy",
    "Quests"
)

foreach ($folder in $testFolders) {
    $path = Join-Path $TestsPath $folder
    New-Item -ItemType Directory -Force -Path $path | Out-Null
}

$testFiles = @(
    "Auth\AuthServiceTests.cs",
    "Projects\ProjectServiceTests.cs",
    "Players\PlayerServiceTests.cs",
    "Saves\SaveServiceTests.cs",
    "Inventory\InventoryServiceTests.cs",
    "Economy\EconomyServiceTests.cs",
    "Quests\QuestServiceTests.cs"
)

foreach ($file in $testFiles) {
    $path = Join-Path $TestsPath $file
    $directory = Split-Path $path -Parent

    if (-not (Test-Path $directory)) {
        New-Item -ItemType Directory -Force -Path $directory | Out-Null
    }

    if (-not (Test-Path $path)) {
        New-Item -ItemType File -Path $path | Out-Null
    }
}

Write-Host "API structure: $ApiPath" -ForegroundColor Green
Write-Host "Tests structure: $TestsPath" -ForegroundColor Green