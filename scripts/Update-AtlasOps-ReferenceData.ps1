[CmdletBinding()]
param(
    [ValidateSet('All', 'Iana', 'Lifecycle', 'Localization', 'Cloud', 'Geospatial')]
    [string[]] $Pack = @('All')
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$dataRoot = Join-Path $repositoryRoot 'src\AtlasOps.ReferenceData\Data'
$stagingRoot = Join-Path ([System.IO.Path]::GetTempPath()) ('atlasops-reference-data-' + [Guid]::NewGuid().ToString('N'))
$retrievedAt = [DateTimeOffset]::UtcNow.ToString('O')

function Test-Pack {
    param([string] $Name)

    return $Pack -contains 'All' -or $Pack -contains $Name
}

function Invoke-Download {
    param(
        [string] $Uri,
        [string] $Destination
    )

    if (-not $Uri.StartsWith('https://', [StringComparison]::OrdinalIgnoreCase)) {
        throw "Only HTTPS reference-data sources are allowed: $Uri"
    }

    New-Item -ItemType Directory -Force (Split-Path -Parent $Destination) | Out-Null
    Invoke-WebRequest -Uri $Uri -OutFile $Destination
}

function Copy-PackAtomically {
    param(
        [string] $Name,
        [string] $Source
    )

    $destination = Join-Path $dataRoot $Name
    $backup = "$destination.previous"
    if (Test-Path $backup) {
        Remove-Item $backup -Recurse -Force
    }

    if (Test-Path $destination) {
        Move-Item $destination $backup
    }

    try {
        Move-Item $Source $destination
        if (Test-Path $backup) {
            Remove-Item $backup -Recurse -Force
        }
    }
    catch {
        if (Test-Path $destination) {
            Remove-Item $destination -Recurse -Force
        }

        if (Test-Path $backup) {
            Move-Item $backup $destination
        }

        throw
    }
}

function Write-Manifest {
    param(
        [string] $Directory,
        [string] $Name,
        [string] $Version,
        [string] $License,
        [string[]] $Sources
    )

    $files = Get-ChildItem $Directory -Recurse -File |
        Where-Object { $_.Name -ne 'manifest.json' } |
        Sort-Object FullName |
        ForEach-Object {
            [ordered]@{
                path = [IO.Path]::GetRelativePath($Directory, $_.FullName).Replace('\', '/')
                bytes = $_.Length
                sha256 = (Get-FileHash $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
            }
        }

    $manifest = [ordered]@{
        name = $Name
        version = $Version
        license = $License
        retrievedAt = $retrievedAt
        sources = $Sources
        fileCount = @($files).Count
        files = @($files)
    }
    $manifest | ConvertTo-Json -Depth 6 | Set-Content (Join-Path $Directory 'manifest.json') -Encoding utf8
}

try {
    New-Item -ItemType Directory -Force $stagingRoot | Out-Null
    New-Item -ItemType Directory -Force $dataRoot | Out-Null

    if (Test-Pack 'Iana') {
        $directory = Join-Path $stagingRoot 'iana'
        New-Item -ItemType Directory -Force $directory | Out-Null
        $sources = @(
            'https://www.iana.org/assignments/service-names-port-numbers/service-names-port-numbers.csv',
            'https://www.iana.org/assignments/protocol-numbers/protocol-numbers-1.csv',
            'https://www.iana.org/assignments/tls-parameters/tls-parameters-4.csv'
        )
        foreach ($source in $sources) {
            Invoke-Download $source (Join-Path $directory ([IO.Path]::GetFileName($source)))
        }
        Invoke-Download 'https://www.iana.org/help/licensing-terms' (Join-Path $directory 'LICENSE.html')
        Write-Manifest $directory 'IANA protocol parameter registries' '2026-03 snapshot' 'CC0-1.0' $sources
        Copy-PackAtomically 'iana' $directory
    }

    if (Test-Pack 'Lifecycle') {
        $download = Join-Path $stagingRoot 'release-data.zip'
        $expanded = Join-Path $stagingRoot 'release-data-expanded'
        $directory = Join-Path $stagingRoot 'lifecycle'
        Invoke-Download 'https://github.com/endoflife-date/release-data/archive/refs/heads/main.zip' $download
        Expand-Archive $download $expanded
        $sourceRoot = Get-ChildItem $expanded -Directory | Select-Object -First 1
        New-Item -ItemType Directory -Force $directory | Out-Null
        Copy-Item (Join-Path $sourceRoot.FullName 'releases') $directory -Recurse
        Copy-Item (Join-Path $sourceRoot.FullName 'LICENSE') $directory
        $commit = (Invoke-RestMethod 'https://api.github.com/repos/endoflife-date/release-data/commits/main').sha
        Write-Manifest $directory 'endoflife.date release data' $commit 'MIT' @(
            'https://github.com/endoflife-date/release-data'
        )
        Copy-PackAtomically 'lifecycle' $directory
    }

    if (Test-Pack 'Localization') {
        $directory = Join-Path $stagingRoot 'localization'
        $cldr = Join-Path $directory 'cldr'
        $tzdb = Join-Path $directory 'tzdb'
        New-Item -ItemType Directory -Force $cldr | Out-Null
        New-Item -ItemType Directory -Force $tzdb | Out-Null
        foreach ($package in @('cldr-core', 'cldr-localenames-full', 'cldr-numbers-full', 'cldr-dates-full')) {
            $archive = Join-Path $stagingRoot "$package.tgz"
            $packageRoot = Join-Path $cldr $package
            Invoke-Download "https://registry.npmjs.org/$package/-/$package-48.2.0.tgz" $archive
            New-Item -ItemType Directory -Force $packageRoot | Out-Null
            tar -xzf $archive -C $packageRoot
            if ($LASTEXITCODE -ne 0) {
                throw "Failed to extract $package."
            }
        }
        Invoke-Download 'https://raw.githubusercontent.com/unicode-org/cldr-json/48.2.0/LICENSE' (Join-Path $cldr 'LICENSE')
        $tzArchive = Join-Path $stagingRoot 'tzdata-latest.tar.gz'
        Invoke-Download 'https://data.iana.org/time-zones/tzdata-latest.tar.gz' $tzArchive
        tar -xzf $tzArchive -C $tzdb
        if ($LASTEXITCODE -ne 0) {
            throw 'Failed to extract tzdata.'
        }
        $tzVersion = (Get-Content (Join-Path $tzdb 'version') -Raw).Trim()
        Write-Manifest $directory 'Unicode CLDR and IANA tzdb' "CLDR 48.2.0; tzdb $tzVersion" 'Unicode-3.0 and public domain' @(
            'https://github.com/unicode-org/cldr-json',
            'https://data.iana.org/time-zones/tzdata-latest.tar.gz'
        )
        Copy-PackAtomically 'localization' $directory
    }

    if (Test-Pack 'Cloud') {
        $directory = Join-Path $stagingRoot 'cloud'
        $raw = Join-Path $stagingRoot 'cloud-raw'
        New-Item -ItemType Directory -Force $directory | Out-Null
        New-Item -ItemType Directory -Force $raw | Out-Null
        $aws = Join-Path $raw 'aws.json'
        $azure = Join-Path $raw 'azure.json'
        $gcp = Join-Path $raw 'gcp.json'
        Invoke-Download 'https://instances.vantage.sh/instances.json' $aws
        Invoke-Download 'https://instances.vantage.sh/azure/instances.json' $azure
        Invoke-Download 'https://instances.vantage.sh/gcp/instances.json' $gcp
        python (Join-Path $PSScriptRoot 'Normalize-CloudInstances.py') `
            --aws $aws --azure $azure --gcp $gcp --output (Join-Path $directory 'offers.ndjson')
        if ($LASTEXITCODE -ne 0) {
            throw 'Cloud catalog normalization failed.'
        }
        Invoke-Download 'https://raw.githubusercontent.com/vantage-sh/ec2instances.info/master/LICENSE' (Join-Path $directory 'LICENSE')
        Write-Manifest $directory 'Vantage cloud instance catalogs' '2026-03 snapshot' 'MIT' @(
            'https://instances.vantage.sh/instances.json',
            'https://instances.vantage.sh/azure/instances.json',
            'https://instances.vantage.sh/gcp/instances.json'
        )
        Copy-PackAtomically 'cloud' $directory
    }

    if (Test-Pack 'Geospatial') {
        $directory = Join-Path $stagingRoot 'geospatial'
        New-Item -ItemType Directory -Force $directory | Out-Null
        $base = 'https://raw.githubusercontent.com/nvkelso/natural-earth-vector/master/geojson'
        $files = @(
            'ne_110m_admin_0_countries.geojson',
            'ne_50m_admin_1_states_provinces.geojson',
            'ne_10m_populated_places.geojson',
            'ne_10m_airports.geojson',
            'ne_10m_ports.geojson',
            'ne_10m_time_zones.geojson',
            'ne_10m_roads.geojson',
            'ne_10m_railroads.geojson',
            'ne_10m_rivers_lake_centerlines.geojson',
            'ne_10m_lakes.geojson'
        )
        foreach ($file in $files) {
            Invoke-Download "$base/$file" (Join-Path $directory $file)
        }
        Invoke-Download 'https://www.naturalearthdata.com/about/terms-of-use/' (Join-Path $directory 'LICENSE.html')
        Write-Manifest $directory 'Natural Earth vector layers' 'master snapshot 2026-03' 'Public domain' ($files | ForEach-Object { "$base/$_" })
        Copy-PackAtomically 'geospatial' $directory
    }
}
finally {
    if (Test-Path $stagingRoot) {
        Remove-Item $stagingRoot -Recurse -Force
    }
}
