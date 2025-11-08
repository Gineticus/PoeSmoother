# PoE Smoother - WPF GUI Application

## Overview
PoE Smoother has been converted from a console application to a modern Windows Presentation Foundation (WPF) GUI application.

## Features

### User Interface
- **Modern GUI**: Clean, user-friendly interface with styled buttons and controls
- **File Browser**: Browse for GGPK or .bin files using a native Windows file dialog
- **Patch Selection**: Visual list of all available patches with checkboxes
- **Bulk Operations**: Select all or deselect all patches with one click
- **Progress Indication**: Visual progress bar during patch application
- **Status Updates**: Real-time status messages showing current operation

### Available Patches
1. Camera
2. Fog
3. Environment Particles
4. Minimap
5. Shadow
6. Light
7. Corpse
8. Delirium
9. Particles

## How to Use

1. **Launch the Application**
   - Run the executable or use `dotnet run --project PoeSmoother.csproj`

2. **Select GGPK File**
   - Click the "Browse..." button
   - Navigate to your Path of Exile installation
   - Select either:
     - `Content.ggpk` file, or
     - `Bundles2\_.index.bin` file
   - Example path: `C:\Program Files (x86)\Steam\steamapps\common\Path of Exile\Bundles2\_.index.bin`

3. **Choose Patches**
   - Review the list of available patches with descriptions
   - Check individual patches you want to apply, or
   - Use "Select All" to choose all patches, or
   - Use "Select None" to deselect all patches

4. **Apply Patches**
   - Click "Apply Selected Patches" to apply only checked patches
   - Click "Apply All Patches" to apply all patches regardless of selection
   - Monitor the progress bar and status messages
   - Wait for the success confirmation dialog

## Technical Details

### Project Structure
```
PoeSmoother/
├── App.xaml                 - WPF Application definition with styles
├── App.xaml.cs             - Application code-behind
├── MainWindow.xaml         - Main window UI definition
├── MainWindow.xaml.cs      - Main window logic and patch application
├── PoeSmoother.csproj      - Project configuration
└── Patches/                - Patch implementations
    ├── IPatch.cs
    ├── Camera.cs
    ├── Corpse.cs
    ├── Delirium.cs
    ├── EnvironmentParticles.cs
    ├── Fog.cs
    ├── Light.cs
    ├── Minimap.cs
    ├── Particles.cs
    └── Shadow.cs
```

### Technology Stack
- **.NET 9.0**: Latest .NET framework
- **WPF (Windows Presentation Foundation)**: Modern Windows UI framework
- **MVVM Pattern**: PatchViewModel for data binding
- **Async/Await**: Non-blocking UI during patch operations
- **LibGGPK3**: Library for GGPK file manipulation

### Key Changes from Console Version
1. **Project Type**: Changed from `Exe` to `WinExe`
2. **Target Framework**: Changed from `net9.0` to `net9.0-windows`
3. **UI Framework**: Added `<UseWPF>true</UseWPF>`
4. **Entry Point**: Removed Program.cs, using App.xaml as entry point
5. **User Experience**: Visual interface instead of text-based menu

## Building the Application

### Debug Build
```powershell
dotnet build
```

### Release Build
```powershell
dotnet build -c Release
```

### Run Application
```powershell
dotnet run --project PoeSmoother.csproj
```

## Error Handling
- File validation before operations
- Try-catch blocks around patch application
- User-friendly error messages in dialog boxes
- Status updates for operation progress

## Requirements
- Windows OS (WPF is Windows-only)
- .NET 9.0 Runtime
- Path of Exile installation with GGPK or index.bin file

## Future Enhancements
- Patch preset saving/loading
- Undo/restore functionality
- Patch information tooltips
- Backup creation before applying patches
- Multi-language support
