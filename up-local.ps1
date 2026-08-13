param(
	[string]$ProjectPath = "src/DNDTracker.Main/DNDTracker.Main.csproj",
	[string]$SecretsId = "DndTracker",
	[switch]$Build,
	[switch]$Detached
)

$ErrorActionPreference = "Stop"

$secretsOutput = $null

if (-not [string]::IsNullOrWhiteSpace($SecretsId)) {
	Write-Host "Lettura degli User Secrets con id: $SecretsId"
	$secretsOutput = dotnet user-secrets list --id $SecretsId
}

if ($LASTEXITCODE -ne 0 -or -not $secretsOutput) {
	Write-Host "Fallback: lettura User Secrets dal progetto: $ProjectPath"
	$secretsOutput = dotnet user-secrets list --project $ProjectPath
}

if ($LASTEXITCODE -ne 0) {
	throw "Impossibile leggere gli User Secrets né con id '$SecretsId' né da '$ProjectPath'."
}

$requiredSecrets = @(
	"NEW_RELIC_LICENSE_KEY"
)

foreach ($secretName in $requiredSecrets) {
	$secretLine = $secretsOutput |
		Where-Object { $_ -match "^$([regex]::Escape($secretName))\s*=\s*" } |
		Select-Object -First 1

	if (-not $secretLine) {
		throw "$secretName non trovata negli User Secrets. Esegui: dotnet user-secrets set `"$secretName`" `"<valore>`" --id $SecretsId"
	}

	$secretValue = ($secretLine -split '\s*=\s*', 2)[1].Trim()

	if ([string]::IsNullOrWhiteSpace($secretValue)) {
		throw "$secretName è vuota negli User Secrets."
	}

	Set-Item -Path "Env:$secretName" -Value $secretValue
	Write-Host "$secretName caricata in variabile ambiente per questa sessione."
}

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
