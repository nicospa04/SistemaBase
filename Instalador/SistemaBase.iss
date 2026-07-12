#define MyAppName "SistemaBase"
#define MyAppVersion "1.5"
#define MyAppPublisher "My Company, Inc."
#define MyAppExeName "SistemaBase.exe"
#define BuildOutput "C:\Users\nicol\Desktop\SistemaBase\SistemaBase\bin\Release"

[Setup]
AppId={{53BD5D5F-9099-4105-8CFE-1B3361A0A4FC}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
UninstallDisplayIcon={app}\{#MyAppExeName}
DisableProgramGroupPage=yes
OutputDir=C:\Users\nicol\Desktop\Instalador
OutputBaseFilename=SistemaBaseSetup
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "{#BuildOutput}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Code]
const
  REG_SQL_INSTANCES = 'HKLM\SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL';
  REG_SQL_INSTANCES_WOW = 'HKLM\SOFTWARE\Wow6432Node\Microsoft\Microsoft SQL Server\Instance Names\SQL';

var
  SQLPage: TWizardPage;
  InstanceCombo: TNewComboBox;
  InstanceLabel: TLabel;
  SelectedInstance: String;

function ExecCaptureOutput(const Cmd, Params, OutFile: string): Boolean;
var
  RC: Integer;
begin
  Result := Exec('cmd.exe', '/c ' + Cmd + ' ' + Params + ' > "' + OutFile + '" 2>&1',
    '', SW_HIDE, ewWaitUntilTerminated, RC) and (RC = 0);
end;

procedure AddIfNotExists(Combo: TNewComboBox; const Value: String);
var
  i: Integer;
begin
  for i := 0 to Combo.Items.Count - 1 do
    if SameText(Combo.Items[i], Value) then
      Exit;

  Combo.Items.Add(Value);
end;

function FirstToken(const Line: String): String;
var
  i: Integer;
  ch: Char;
  Started: Boolean;
begin
  Result := '';
  Started := False;

  for i := 1 to Length(Line) do
  begin
    ch := Line[i];
    if (ch = ' ') or (ch = #9) then
    begin
      if Started then
        Exit;
    end
    else
    begin
      Started := True;
      Result := Result + ch;
    end;
  end;
end;

procedure ParseRegQueryOutput(const FileName: String; Combo: TNewComboBox);
var
  S: AnsiString;
  L: TStringList;
  i: Integer;
  Line, InstanceName, MachineName: String;
begin
  if not LoadStringFromFile(FileName, S) then
    Exit;

  L := TStringList.Create;
  try
    L.Text := String(S);
    MachineName := GetEnv('COMPUTERNAME');

    for i := 0 to L.Count - 1 do
    begin
      Line := Trim(L[i]);
      if Line = '' then
        Continue;

      if Copy(Line, 1, 5) = 'HKEY_' then
        Continue;

      if Pos('REG_', Line) = 0 then
        Continue;

      InstanceName := FirstToken(Line);
      if InstanceName = '' then
        Continue;

      if SameText(InstanceName, 'MSSQLSERVER') then
      begin
        AddIfNotExists(Combo, MachineName);
      end
      else
      begin
        AddIfNotExists(Combo, MachineName + '\' + InstanceName);
      end;
    end;
  finally
    L.Free;
  end;
end;

procedure ParseLocalDBOutput(const FileName: String; Combo: TNewComboBox);
var
  S: AnsiString;
  L: TStringList;
  i: Integer;
  Line: String;
begin
  if not LoadStringFromFile(FileName, S) then
    Exit;

  L := TStringList.Create;
  try
    L.Text := String(S);

    for i := 0 to L.Count - 1 do
    begin
      Line := Trim(L[i]);
      if Line = '' then
        Continue;

      if SameText(Copy(Line, 1, 9), 'Instances') then
        Continue;

      if Pos('No LocalDB instances', Line) > 0 then
        Continue;

      AddIfNotExists(Combo, '(localdb)\' + Line);
    end;
  finally
    L.Free;
  end;
end;

procedure ParseSqlServicesOutput(const FileName: String; Combo: TNewComboBox);
var
  S: AnsiString;
  L: TStringList;
  i: Integer;
  Line, ServiceName, MachineName: String;
begin
  if not LoadStringFromFile(FileName, S) then
    Exit;

  L := TStringList.Create;
  try
    L.Text := String(S);
    MachineName := GetEnv('COMPUTERNAME');

    for i := 0 to L.Count - 1 do
    begin
      Line := Trim(L[i]);
      if Pos('SERVICE_NAME:', UpperCase(Line)) <> 1 then
        Continue;

      ServiceName := Trim(Copy(Line, Pos(':', Line) + 1, Length(Line)));
      if SameText(ServiceName, 'MSSQLSERVER') then
      begin
        AddIfNotExists(Combo, MachineName);
      end
      else if SameText(Copy(ServiceName, 1, 6), 'MSSQL$') then
      begin
        AddIfNotExists(Combo, MachineName + '\' + Copy(ServiceName, 7, Length(ServiceName)));
      end;
    end;
  finally
    L.Free;
  end;
end;

function TestSqlConnection(const ServerName: String; var ErrorMessage: String): Boolean;
var
  Conn: Variant;
begin
  Result := False;
  ErrorMessage := '';

  try
    Conn := CreateOleObject('ADODB.Connection');
    Conn.ConnectionTimeout := 5;
    Conn.Open('Provider=SQLOLEDB;Data Source=' + ServerName + ';Initial Catalog=master;Integrated Security=SSPI;');
    Conn.Close;
    Result := True;
  except
    ErrorMessage := GetExceptionMessage;
  end;
end;

procedure FillInstanceCombo(Combo: TNewComboBox);
var
  TmpFile, LocalDBOut, ServicesOut: String;
begin
  Combo.Items.Clear;

  TmpFile := ExpandConstant('{tmp}\reg_sql_instances.txt');
  if ExecCaptureOutput('reg query', '"' + REG_SQL_INSTANCES + '" /s /reg:64', TmpFile) then
    ParseRegQueryOutput(TmpFile, Combo);

  if ExecCaptureOutput('reg query', '"' + REG_SQL_INSTANCES + '" /s /reg:32', TmpFile) then
    ParseRegQueryOutput(TmpFile, Combo);

  if ExecCaptureOutput('reg query', '"' + REG_SQL_INSTANCES_WOW + '" /s /reg:64', TmpFile) then
    ParseRegQueryOutput(TmpFile, Combo);

  if ExecCaptureOutput('reg query', '"' + REG_SQL_INSTANCES_WOW + '" /s /reg:32', TmpFile) then
    ParseRegQueryOutput(TmpFile, Combo);

  ServicesOut := ExpandConstant('{tmp}\sql_services.txt');
  if ExecCaptureOutput('sc', 'query state= all', ServicesOut) then
    ParseSqlServicesOutput(ServicesOut, Combo);

  LocalDBOut := ExpandConstant('{tmp}\localdb_instances.txt');
  if ExecCaptureOutput('sqllocaldb', 'info', LocalDBOut) then
    ParseLocalDBOutput(LocalDBOut, Combo);
end;

procedure InitializeWizard;
begin
  SQLPage := CreateCustomPage(wpSelectDir,
    'Seleccionar instancia de SQL Server',
    'Elija la instancia donde se instalara la base SistemaBase');

  InstanceLabel := TLabel.Create(SQLPage);
  InstanceLabel.Parent := SQLPage.Surface;
  InstanceLabel.Left := 0;
  InstanceLabel.Top := 0;
  InstanceLabel.Width := SQLPage.SurfaceWidth;
  InstanceLabel.Caption :=
    'Seleccione o escriba una instancia:'#13#10#13#10 +
    'El instalador muestra las instancias detectadas en esta computadora.'#13#10 +
    'Si no aparece, escriba el nombre exacto del servidor SQL Server.';

  InstanceCombo := TNewComboBox.Create(SQLPage);
  InstanceCombo.Parent := SQLPage.Surface;
  InstanceCombo.Left := 0;
  InstanceCombo.Top := 100;
  InstanceCombo.Width := SQLPage.SurfaceWidth;
  InstanceCombo.Style := csDropDown;

  FillInstanceCombo(InstanceCombo);
  if InstanceCombo.Items.Count > 0 then
    InstanceCombo.ItemIndex := 0
  else
    InstanceLabel.Caption :=
      'No se detectaron instancias de SQL Server.'#13#10#13#10 +
      'Escriba el nombre exacto de la instancia instalada en esta computadora.';
end;

function NextButtonClick(CurPageID: Integer): Boolean;
var
  ErrorMessage: String;
begin
  Result := True;

  if CurPageID = SQLPage.ID then
  begin
    SelectedInstance := Trim(InstanceCombo.Text);

    if SelectedInstance = '' then
    begin
      MsgBox('Debe ingresar una instancia de SQL Server.', mbError, MB_OK);
      Result := False;
    end;

    if Result and not TestSqlConnection(SelectedInstance, ErrorMessage) then
    begin
      MsgBox('No se pudo conectar a la instancia "' + SelectedInstance + '".'#13#10#13#10 +
        'Verifique que el servicio SQL Server este iniciado y que el nombre sea correcto.'#13#10#13#10 +
        ErrorMessage, mbError, MB_OK);
      Result := False;
    end;
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
  begin
    if SelectedInstance = '' then
      SelectedInstance := Trim(InstanceCombo.Text);

    SaveStringToFile(ExpandConstant('{app}\instancia.txt'), SelectedInstance, False);
    SaveStringToFile(ExpandConstant('{app}\entroInstalador.txt'), '', False);
  end;
end;

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
