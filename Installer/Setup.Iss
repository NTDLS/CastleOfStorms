#define Version "0.0.37"

[Setup]
;-- Main Setup Information
 AppName                         = Strikeforce Infinite
 AppVerName                      = Strikeforce Infinite {#Version}
 AppCopyright                    = Copyright © 1995-2024 NetworkDLS.
 DefaultDirName                  = {commonpf}\NetworkDLS\Strikeforce Infinite
 DefaultGroupName                = NetworkDLS\Strikeforce Infinite
 UninstallDisplayIcon            = {app}\Cos.Client.exe
 SetupIconFile                   = ../@Resources\StrikeforceInfinite.ico
 PrivilegesRequired              = admin
 Uninstallable                   = Yes
 Compression                     = ZIP/9
 ChangesAssociations             = Yes
 OutputBaseFilename              = Cos.Installer-{#Version}
 ArchitecturesInstallIn64BitMode = x64
 ArchitecturesAllowed            = x64

;-- Windows 2000 & XP (Support Dialog)
 AppPublisher    = NetworkDLS
 AppPublisherURL = http://www.NetworkDLS.com/
 AppUpdatesURL   = http://www.NetworkDLS.com/
 AppVersion      = {#Version}
 
[Components]
 Name: "Client";       Description: "Strikeforce Infinite"; Types: Full Compact Custom; Flags: Fixed;
 Name: "Assets";       Description: "Assets";               Types: Full Compact Custom; Flags: Fixed;
 Name: "FontAsset";    Description: "Fonts";                Types: Full Compact Custom; Flags: Fixed;
 
[Files]
 Source: "../Cos.Client\bin\Release\net8.0-windows7.0\*.exe";   DestDir: "{app}"; Components: Client;   Flags: IgnoreVersion; 
 Source: "../Cos.Client\bin\Release\net8.0-windows7.0\*.dll";   DestDir: "{app}"; Components: Client;   Flags: IgnoreVersion;
 Source: "../Cos.Client\bin\Release\net8.0-windows7.0\*.json";  DestDir: "{app}"; Components: Client;   Flags: IgnoreVersion;
 Source: "Cos.Assets.rez";                                      DestDir: "{app}"; Components: Assets; Flags: IgnoreVersion recursesubdirs;

 Source: "Fonts\Digital-7 (Mono).ttf"; DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Digital-7 Mono";  Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\Digital-7.ttf";        DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "digital-7";       Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\Orbitronio.ttf";       DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Orbitronio";      Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\Roboto-Regular.ttf";   DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Roboto";          Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\Roboto-Thin.ttf";      DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Roboto Th";       Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\visitor1.ttf";         DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Visitor TT1 BRK"; Flags: onlyifdoesntexist uninsneveruninstall

[Registry]
 Root: HKLM; Subkey: "Software\NetworkDLS\Strikeforce Infinite"; ValueType: STRING;ValueName: "CommonAssetPath"; ValueData:"{app}\Assets";                      Flags: CreateValueIfDoesntExist;
 Root: HKLM; Subkey: "Software\NetworkDLS\Strikeforce Infinite"; ValueType: STRING;ValueName: "UserAssetPath";   ValueData:"{userappdata}\StrikeforceInfinite"; Flags: CreateValueIfDoesntExist;

[Icons]
 Name: "{group}\Strikeforce Infinite"; Filename: "{app}\Cos.Client.exe"; Components: Client;
 
[Run]
 Filename: "{app}\Cos.Client.exe"; Description: "Run Strikeforce Infinite now?"; Flags: postinstall nowait skipifsilent shellexec; Components: Client;
