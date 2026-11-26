; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Squrl"
#ifndef MyAppVersion
  #define MyAppVersion "0.0.0-dev"
#endif
#define MyAppExeName "squrl.exe"
#define OutputFileName "squrl"

#ifndef OutputDir
  #define OutputDir "..\artifacts"
#endif
#ifndef SourceDir
  #define SourceDir "..\publish\windows"
#endif

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{683FD8B1-94BF-49AD-98B8-1B3FDDEF9834}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
UninstallDisplayIcon={app}\{#MyAppExeName}
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
DisableProgramGroupPage=yes
PrivilegesRequired=admin
OutputDir={#OutputDir}
OutputBaseFilename={#OutputFileName}-{#MyAppVersion}-win-x64
SolidCompression=yes
WizardStyle=modern dynamic

[Dirs]
Name: "{commonappdata}\{#MyAppName}"
Name: "{commonappdata}\{#MyAppName}\backups"
Name: "{commonappdata}\{#MyAppName}\logs"
Name: "{commonappdata}\{#MyAppName}\images"

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "{#SourceDir}\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion

Source: "{#SourceDir}\*"; \
  DestDir: "{app}"; \
  Excludes: "*.pdb,*.xml,appsettings.*.json,*.db"; \
  Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Registry]
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; \
  ValueType: string; ValueName: "Squrl"; Flags: uninsdeletevalue

[Run]
Filename: "netsh"; \
  Parameters: "advfirewall firewall add rule name=""squrl"" dir=in action=allow protocol=TCP localport=33600"; \
  Flags: runhidden waituntilterminated

Filename: "{app}\{#MyAppExeName}"; \
  Description: "Launch Squrl"; \
  Flags: nowait postinstall skipifsilent

[UninstallRun]
Filename: "netsh"; \
  Parameters: "advfirewall firewall delete rule name=""squrl"""; \
  RunOnceId: "DeleteSqurlFirewallRule"; \
  Flags: runhidden waituntilterminated