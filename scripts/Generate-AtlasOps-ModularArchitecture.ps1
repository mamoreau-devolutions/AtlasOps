[CmdletBinding()]
param(
    [string] $RepositoryRoot = (Split-Path -Parent $PSScriptRoot),
    [switch] $SkipSolutionUpdate
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$arguments = @((Join-Path $PSScriptRoot 'generate_modular.py'), '--repository-root', $RepositoryRoot)
if ($SkipSolutionUpdate) {
    $arguments += '--skip-solution-update'
}

& python @arguments
if ($LASTEXITCODE -eq 0) {
    & python (Join-Path $PSScriptRoot 'generate_modular_operations.py') --repository-root $RepositoryRoot
}

if ($LASTEXITCODE -ne 0) {
    throw "AtlasOps modular architecture generation failed with exit code $LASTEXITCODE."
}
