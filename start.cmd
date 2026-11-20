@echo off
REM ==========================================================================
REM  RepairIT - Lancement de tous les projets en une seule fois (Windows)
REM  Chaque service s'ouvre dans sa propre fenetre de console.
REM ==========================================================================
cd /d "%~dp0"

REM Une seule compilation de toute la solution avant le lancement :
REM evite que les services compilent RepairIT.Shared en meme temps.
echo Compilation de la solution RepairIT...
dotnet build RepairIT.slnx
if errorlevel 1 (
    echo La compilation a echoue, lancement annule.
    pause
    exit /b 1
)

start "RepairIT.CatalogService (5001)"         dotnet run --no-build --project RepairIT.CatalogService --launch-profile http
start "RepairIT.UserService (5002)"            dotnet run --no-build --project RepairIT.UserService --launch-profile http
start "RepairIT.RepairService (5003)"          dotnet run --no-build --project RepairIT.RepairService --launch-profile http
start "RepairIT.SalesService (5004)"           dotnet run --no-build --project RepairIT.SalesService --launch-profile http
start "RepairIT.AuthentificationService (5005)" dotnet run --no-build --project RepairIT.AuthentificationService --launch-profile http
start "RepairIT.Client (5000)"                 dotnet run --no-build --project RepairIT.Client --launch-profile http

echo.
echo Front Blazor        : http://localhost:5000
echo Catalogue (Swagger) : http://localhost:5001/swagger
echo Utilisateurs        : http://localhost:5002/swagger
echo Reparations         : http://localhost:5003/swagger
echo Ventes              : http://localhost:5004/swagger
echo Authentification    : http://localhost:5005/swagger
