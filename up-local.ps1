param(
	[string]$ProjectPath = "src/DNDTracker.Main/DNDTracker.Main.csproj",
	[switch]$Build,
	[switch]$Detached
)

$ErrorActionPreference = "Stop"

$composeArgs = @("compose", "up")

if ($Build) {
	$composeArgs += "--build"
}

if ($Detached) {
	$composeArgs += "-d"
}

docker @composeArgs

if ($LASTEXITCODE -ne 0) {
	throw "docker compose up fallito con codice $LASTEXITCODE"
}
