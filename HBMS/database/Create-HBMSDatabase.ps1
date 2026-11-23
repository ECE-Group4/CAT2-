param([string]$DatabasePath = "$PSScriptRoot\HBMS.accdb", [switch]$Force)
$ErrorActionPreference = 'Stop'
if (Test-Path $DatabasePath) {
  if (-not $Force) { throw "Database already exists: $DatabasePath. Use -Force to replace it." }
  Remove-Item $DatabasePath -Force
}
$catalog = New-Object -ComObject ADOX.Catalog
$connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$DatabasePath;Jet OLEDB:Engine Type=6;"
$catalog.Create($connectionString)
$conn = New-Object -ComObject ADODB.Connection
$conn.Open($connectionString)
try {
  $sql = (Get-Content (Join-Path $PSScriptRoot 'schema.sql') | Where-Object { -not $_.Trim().StartsWith('--') }) -join [Environment]::NewLine
  foreach ($statement in ($sql -split ';' | Where-Object { $_.Trim() })) { $conn.Execute($statement.Trim()) | Out-Null }
  # Referential integrity is declared separately so Access can create all tables first.
  $relations = @(
    'ALTER TABLE Assets ADD CONSTRAINT FK_Assets_Wards FOREIGN KEY (WardID) REFERENCES Wards (WardID)',
    'ALTER TABLE Assets ADD CONSTRAINT FK_Assets_Vendors FOREIGN KEY (VendorID) REFERENCES Vendors (VendorID)',
    'ALTER TABLE VendorSLAs ADD CONSTRAINT FK_VendorSLAs_Vendors FOREIGN KEY (VendorID) REFERENCES Vendors (VendorID)',
    'ALTER TABLE Breakdowns ADD CONSTRAINT FK_Breakdowns_Assets FOREIGN KEY (AssetID) REFERENCES Assets (AssetID)',
    'ALTER TABLE Breakdowns ADD CONSTRAINT FK_Breakdowns_Users FOREIGN KEY (ReportedByUserID) REFERENCES Users (UserID)',
    'ALTER TABLE Breakdowns ADD CONSTRAINT FK_Breakdowns_ResponseUsers FOREIGN KEY (ResponseByUserID) REFERENCES Users (UserID)',
    'ALTER TABLE Breakdowns ADD CONSTRAINT FK_Breakdowns_SLA FOREIGN KEY (SLAID) REFERENCES VendorSLAs (SLAID)',
    'ALTER TABLE MaintenanceJobs ADD CONSTRAINT FK_Jobs_Breakdowns FOREIGN KEY (BreakdownID) REFERENCES Breakdowns (BreakdownID)',
    'ALTER TABLE JobSpareParts ADD CONSTRAINT FK_JobParts_Jobs FOREIGN KEY (JobID) REFERENCES MaintenanceJobs (JobID)',
    'ALTER TABLE JobSpareParts ADD CONSTRAINT FK_JobParts_Parts FOREIGN KEY (PartID) REFERENCES SpareParts (PartID)',
    'ALTER TABLE RepairCosts ADD CONSTRAINT FK_Costs_Jobs FOREIGN KEY (JobID) REFERENCES MaintenanceJobs (JobID)',
    'ALTER TABLE RepairCosts ADD CONSTRAINT FK_Costs_Users FOREIGN KEY (RecordedByUserID) REFERENCES Users (UserID)',
    'ALTER TABLE MaintenanceJobs ADD CONSTRAINT FK_Jobs_Users FOREIGN KEY (EngineerUserID) REFERENCES Users (UserID)',
    'ALTER TABLE AuditLogs ADD CONSTRAINT FK_Audit_Users FOREIGN KEY (UserID) REFERENCES Users (UserID)',
    'ALTER TABLE EscalationLogs ADD CONSTRAINT FK_Escalations_Breakdowns FOREIGN KEY (BreakdownID) REFERENCES Breakdowns (BreakdownID)',
    'ALTER TABLE ImportedAlerts ADD CONSTRAINT FK_ImportedAlerts_Breakdowns FOREIGN KEY (BreakdownID) REFERENCES Breakdowns (BreakdownID)'
  )
  foreach ($relation in $relations) { try { $conn.Execute($relation) | Out-Null } catch { Write-Warning "Could not create relation (provider/version dependent): $relation — $($_.Exception.Message)" } }
  # Synthetic reference data.
  $seed = @(
    "INSERT INTO Wards (WardCode,WardName,DepartmentName,LocationDescription,IsActive) VALUES ('ICU','Intensive Care Unit','Critical Care','Block A, Level 2',True)",
    "INSERT INTO Wards (WardCode,WardName,DepartmentName,LocationDescription,IsActive) VALUES ('ER','Emergency','Emergency Medicine','Block A, Ground Floor',True)",
    "INSERT INTO Wards (WardCode,WardName,DepartmentName,LocationDescription,IsActive) VALUES ('RAD','Radiology','Imaging','Block B, Ground Floor',True)",
    "INSERT INTO Wards (WardCode,WardName,DepartmentName,LocationDescription,IsActive) VALUES ('THEATRE','Theatre','Surgical Services','Block C, Level 1',True)",
    "INSERT INTO Wards (WardCode,WardName,DepartmentName,LocationDescription,IsActive) VALUES ('GEN','General Ward','Inpatient Services','Block D, Level 1',True)",
    "INSERT INTO Vendors (VendorCode,VendorName,ContactPerson,Phone,Email,IsActive) VALUES ('VND-001','MedTech Services Ltd','Service Desk','0700000001','service@example.invalid',True)",
    "INSERT INTO VendorSLAs (VendorID,SLAName,ResponseHours,ResolutionHours,EffectiveFrom,IsActive) VALUES (1,'Standard Biomedical SLA',4,24,Date(),True)",
    "INSERT INTO Assets (AssetCode,EquipmentName,EquipmentCategory,Manufacturer,Model,SerialNumber,WardID,Criticality,AssetStatus,DateCommissioned,VendorID) VALUES ('ASSET-VENT-001','ICU Ventilator 01','Ventilator','DemoMed','V-100','SYN-VENT-001',1,'CRITICAL_CARE','OPERATIONAL',Date(),1)",
    "INSERT INTO Assets (AssetCode,EquipmentName,EquipmentCategory,Manufacturer,Model,SerialNumber,WardID,Criticality,AssetStatus,DateCommissioned,VendorID) VALUES ('ASSET-MON-001','Patient Monitor 01','Patient Monitor','DemoMed','PM-20','SYN-MON-001',1,'CRITICAL_CARE','OPERATIONAL',Date(),1)",
    "INSERT INTO Assets (AssetCode,EquipmentName,EquipmentCategory,Manufacturer,Model,SerialNumber,WardID,Criticality,AssetStatus,DateCommissioned,VendorID) VALUES ('ASSET-XRAY-001','X-Ray Unit 01','X-Ray','DemoImaging','XR-5','SYN-XRAY-001',3,'HIGH','OPERATIONAL',Date(),1)",
    "INSERT INTO SpareParts (PartCode,PartName,UnitName,QuantityAvailable,ReorderLevel,UnitCost,IsActive) VALUES ('PART-FUSE-01','Medical-grade fuse','piece',25,5,150,True)",
    "INSERT INTO SpareParts (PartCode,PartName,UnitName,QuantityAvailable,ReorderLevel,UnitCost,IsActive) VALUES ('PART-CABLE-01','Power cable assembly','piece',8,2,1200,True)"
  )
  foreach ($statement in $seed) { $conn.Execute($statement) | Out-Null }
  # Password hash: PBKDF2-SHA256, 150,000 iterations, 16-byte salt; format PBKDF2$iterations$base64salt$base64hash.
  $salt = New-Object byte[] 16; [Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($salt)
  $derive = New-Object Security.Cryptography.Rfc2898DeriveBytes('ChangeMe123!',$salt,150000,[Security.Cryptography.HashAlgorithmName]::SHA256)
  $hash = $derive.GetBytes(32)
  $encoded = 'PBKDF2$150000$' + [Convert]::ToBase64String($salt) + '$' + [Convert]::ToBase64String($hash)
  $cmd = $conn.CreateCommand(); $cmd.CommandText = "INSERT INTO Users (FullName,Username,PasswordHash,RoleName,IsActive,CreatedAt) VALUES ('HBMS Administrator','admin','$encoded','ADMIN',True,Now())"; $cmd.Execute() | Out-Null
  Write-Host "HBMS database created: $DatabasePath" -ForegroundColor Green
  Write-Host 'Initial synthetic account: admin / ChangeMe123! — change immediately.' -ForegroundColor Yellow
} finally { $conn.Close(); [void][Runtime.InteropServices.Marshal]::ReleaseComObject($conn); [void][Runtime.InteropServices.Marshal]::ReleaseComObject($catalog) }
