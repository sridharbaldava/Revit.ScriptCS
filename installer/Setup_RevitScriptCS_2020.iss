[Setup]
AppName=RevitScriptCS for Autodesk Revit 2020
AppVerName=RevitScriptCS for Autodesk Revit 2020
RestartIfNeededByRun=false
DefaultDirName={commonpf}\RevitScriptCS\2020
OutputBaseFilename=Setup_RevitScriptCS_2020
ShowLanguageDialog=auto
FlatComponentsList=false
UninstallFilesDir={app}\Uninstall
UninstallDisplayName=RevitScriptCS for Autodesk Revit 2020
AppVersion=2020.0
VersionInfoVersion=2020.0
VersionInfoDescription=RevitScriptCS for Autodesk Revit 2020
VersionInfoTextVersion=RevitScriptCS for Autodesk Revit 2020
SourceDir="..\src\Revit.ScriptCS.ScriptRunner\bin\x64\Release\"

[Files]
Source: "..\..\..\Revit.ScriptCS.ScriptRunner.addin"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2020"; Flags: replacesameversion
Source: "ICSharpCode.AvalonEdit.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "Microsoft.CodeAnalysis.CSharp.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "Microsoft.CodeAnalysis.CSharp.Features.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "Microsoft.CodeAnalysis.CSharp.Scripting.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "Microsoft.CodeAnalysis.CSharp.Workspaces.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "Microsoft.CodeAnalysis.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "Microsoft.CodeAnalysis.Features.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "Microsoft.CodeAnalysis.FlowAnalysis.Utilities.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "Microsoft.CodeAnalysis.Scripting.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "Microsoft.CodeAnalysis.Workspaces.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "Microsoft.DiaSymReader.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "Revit.ScriptCS.ScriptRunner.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "RoslynPad.Editor.Windows.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "RoslynPad.Roslyn.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "RoslynPad.Roslyn.Windows.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "System.Buffers.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "System.Collections.Immutable.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "System.Composition.AttributedModel.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "System.Composition.Convention.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "System.Composition.Hosting.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "System.Composition.Runtime.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "System.Composition.TypedParts.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "System.Memory.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "System.Numerics.Vectors.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "System.Reactive.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "System.Reflection.Metadata.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "System.Runtime.CompilerServices.Unsafe.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "System.Text.Encoding.CodePages.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "System.Threading.Tasks.Extensions.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "AvalonDock.dll"; DestDir: "{app}"; Flags: replacesameversion

[code]
{ HANDLE INSTALL PROCESS STEPS }
procedure CurStepChanged(CurStep: TSetupStep);
var
  AddInFilePath: String;
  LoadedFile : TStrings;
  AddInFileContents: String;
  ReplaceString: String;
  SearchString: String;
begin

  if CurStep = ssPostInstall then
  begin

  AddinFilePath := ExpandConstant('{userappdata}\Autodesk\Revit\Addins\2020\Revit.ScriptCS.ScriptRunner.addin');
  LoadedFile := TStringList.Create;
  SearchString := 'Assembly>Revit.ScriptCS.ScriptRunner.dll<';
  ReplaceString := 'Assembly>' + ExpandConstant('{app}') + '\Revit.ScriptCS.ScriptRunner.dll<';

  try
      LoadedFile.LoadFromFile(AddInFilePath);
      AddInFileContents := LoadedFile.Text;

      { Only save if text has been changed. }
      if StringChangeEx(AddInFileContents, SearchString, ReplaceString, True) > 0 then
      begin;
        LoadedFile.Text := AddInFileContents;
        LoadedFile.SaveToFile(AddInFilePath);
      end;
    finally
      LoadedFile.Free;
    end;

  end;
end;

