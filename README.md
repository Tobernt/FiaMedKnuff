# Fia med knuff

A C# and XAML implementation of the board game Fia med knuff for Windows, built with UWP.

## Features

- Player turns, dice rolls, and computer-controlled players.
- Game settings and sound controls.
- High scores saved locally as JSON.

## Build

Open `FiaMedKnuff.sln` in Visual Studio with UWP development tools and the Windows SDK installed. Restore NuGet packages, select an architecture such as x64, and build the application.

The project targets Windows SDK 10.0.18362.0 with a minimum Windows version of 10.0.17763.0. It uses `Microsoft.NETCore.UniversalWindowsPlatform` and `System.Text.Json`.

## Project structure

- `MainPage.xaml` and `MainPage.xaml.cs`: board interface and interaction.
- `GameLogic.cs`, `Player.cs`, and `AIPlayer.cs`: game rules and player state.
- `PlayerScoreManager.cs`: JSON persistence and score ordering.
- `UserControls/`: settings, dice, sound, and score controls.

Scores are stored in the application's local data folder. No database or external service is required. This is a desktop C# project, not an ASP.NET application.
