@echo off
setlocal ENABLEDELAYEDEXPANSION
color 0A

echo ========================================
echo 🚀 INICIANDO PROYECTO GYM FULLSTACK
echo ========================================

REM === 📁 Rutas basadas en la ubicación del .bat ===
set "ROOT=%~dp0"
set "API_DIR=%ROOT%Api"
set "WEB_DIR=%ROOT%frontend"

REM === 🧩 Checks básicos ===
where dotnet >nul 2>nul || (echo [ERR] Falta dotnet en PATH & pause & exit /b 1)
where npm    >nul 2>nul || (echo [ERR] Falta npm en PATH    & pause & exit /b 1)
if not exist "%API_DIR%" echo [ERR] No se encontró "%API_DIR%" & pause & exit /b 1
if not exist "%WEB_DIR%" echo [ERR] No se encontró "%WEB_DIR%" & pause & exit /b 1

echo.
echo ✅ Verificaciones completadas correctamente.
echo.

REM === 🔹 Iniciar Backend (.NET API) ===
echo [API] Restaurando dependencias...
pushd "%API_DIR%"
dotnet restore >nul
echo [API] Iniciando en http://localhost:5144 ...
start "API_GYM" /MIN cmd /c "cd /d "%API_DIR%" && dotnet watch run --no-restore --urls http://localhost:5144"
popd

REM === 🔹 Configurar Frontend (Vite / React) ===
if not exist "%WEB_DIR%\.env.local" (
  echo VITE_API_BASE_URL=http://localhost:5144>"%WEB_DIR%\.env.local"
  echo [WEB] ✅ Creado archivo .env.local con VITE_API_BASE_URL=http://localhost:5144
)

echo [WEB] Verificando dependencias...
if not exist "%WEB_DIR%\node_modules" (
  pushd "%WEB_DIR%"
  echo [WEB] Instalando dependencias con npm install...
  npm install >nul
  popd
)

echo [WEB] Iniciando Vite (frontend) en http://localhost:5173/login ...
start "GYM_WEB" /MIN cmd /c "cd /d "%WEB_DIR%" && npm run dev -- --port 5173"

echo.
echo ========================================
echo ✅ Todo lanzado correctamente.
echo ----------------------------------------
echo 🌐 API:       http://localhost:5144/swagger
echo 💻 Frontend:  http://localhost:5173/login
echo ----------------------------------------
echo Presioná Ctrl + C para detener ambos servidores.
echo ========================================
echo.
timeout /t -1
endlocal
