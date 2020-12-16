param($template,$filter)
$templateContent = Get-Content -Path $template
$ext = [io.path]::GetExtension($template)
Get-ChildItem "." -Filter $filter
| ForEach-Object {
    $path = Resolve-Path -Relative $_
    $name = [io.path]::GetFileNameWithoutExtension($_.Name);
    $fileName = $name + $ext
    
    $templateContent -replace '{NAME}', $name -replace '{PATH}', $path
    | Set-Content -Path $fileName
}

Write-Host