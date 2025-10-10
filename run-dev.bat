@echo off
setlocal

REM Rutas basadas en la ubicación del .bat
set "ROOT=%~dp0"
set "API_DIR=%ROOT%Api"
set "WEB_DIR=%ROOT%Frontend"

REM Checks básicos
where dotnet >nul 2>nul || (echo [ERR] Falta dotnet en PATH & pause & exit /b 1)
where npm    >nul 2>nul || (echo [ERR] Falta npm en PATH    & pause & exit /b 1)
if not exist "%API_DIR%" echo [ERR] No se encontró "%API_DIR%" & pause & exit /b 1
if not exist "%WEB_DIR%" echo [ERR] No se encontró "%WEB_DIR%" & pause & exit /b 1

REM Backend
echo [API] Restaurando dependencias...
pushd "%API_DIR%"
dotnet restore
echo [API] Iniciando en http://localhost:5144 ...
start "Api" cmd /c "dotnet watch run --no-restore --urls http://localhost:5144"
popd

REM Frontend
if not exist "%WEB_DIR%\.env.local" (
  echo VITE_API_BASE_URL=http://localhost:5144>"%WEB_DIR%\.env.local"
  echo [WEB] Creado .env.local con VITE_API_BASE_URL=http://localhost:5144
)

echo [WEB] Instalando deps (si faltan)...
if not exist "%WEB_DIR%\node_modules" (
  pushd "%WEB_DIR%"
  npm install
  popd
)

echo [WEB] Iniciando Vite en http://localhost:5173 ...
start "gym-web" cmd /c "cd /d "%WEB_DIR%" && npm run dev -- --port 5173"

echo.
echo ✅ Todo lanzado.
echo API:     http://localhost:5144/swagger
echo Frontend http://localhost:5173
echo.
pause
