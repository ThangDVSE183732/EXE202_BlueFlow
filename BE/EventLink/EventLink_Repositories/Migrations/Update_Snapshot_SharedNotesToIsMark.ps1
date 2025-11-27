# Update EventLinkDBContextModelSnapshot.cs to replace SharedNotes with IsMark

$snapshotPath = "EventLink_Repositories\Migrations\EventLinkDBContextModelSnapshot.cs"

Write-Host "Updating EventLinkDBContextModelSnapshot.cs..." -ForegroundColor Yellow

try {
    # Read the file content
    $content = Get-Content $snapshotPath -Raw
    
    # Replace SharedNotes property with IsMark
    $updatedContent = $content -replace `
        'b\.Property<string>\("SharedNotes"\)\s+\.HasColumnType\("nvarchar\(max\)"\);', `
        'b.Property<bool?>("IsMark")
                        .HasColumnType("bit");'
    
    # Write back to file
    Set-Content -Path $snapshotPath -Value $updatedContent -NoNewline
    
    Write-Host "? Successfully updated EventLinkDBContextModelSnapshot.cs" -ForegroundColor Green
    Write-Host "  - Removed: SharedNotes (string)" -ForegroundColor White
    Write-Host "  - Added: IsMark (bool?)" -ForegroundColor White
    
} catch {
    Write-Host "? Error updating snapshot file:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}
