# 🏋️‍♂️ Sistema de Gestión para Gimnasios por Turnos Fijos

## 📘 Descripción General

El sistema **Gym** es una aplicación web completa desarrollada con **ASP.NET Core 9.0** (C#) y **React + Vite** para la gestión integral de un gimnasio con turnos fijos.  
Permite administrar **socios, profesores, planes, rutinas, comprobantes y turnos**, con autenticación por roles, subida de archivos, y comunicación API segura.

---
## ⚙️ Camino Feliz Paso a Paso

1️⃣ Registro del socio

Se crea un registro en socio.

Queda activo, pero aún no tiene suscripción.

2️⃣ Elección del plan

Se elige un plan (plan_id).

El sistema genera automáticamente una orden de pago (orden_pago):

estado_id = Pendiente

monto = plan.precio

vence_en = fecha_actual + 30 días

3️⃣ Generación y gestión de orden de pago

La orden queda pendiente hasta su pago.

Los estados válidos:

Pendiente → creada sin pago.

Aprobada → validada manualmente o con comprobante.

Rechazada → comprobante inválido o vencido.

4️⃣ Adjuntar comprobante (opcional)

El socio o el admin sube un archivo (comprobante) vinculado por orden_pago_id.

El backend guarda el archivo en wwwroot/uploads/comprobantes.

5️⃣ Validación y aprobación del pago

El administrador revisa el comprobante o registra un pago en efectivo.

Cambia el estado_id de la orden a Aprobado.

Puede opcionalmente registrar fecha_pago.

6️⃣ Activación automática de la suscripción

El sistema crea una nueva suscripcion:

inicio = fecha actual

fin = inicio + duración del plan

estado = Activa

El socio ya puede acceder a los servicios del gimnasio.

7️⃣ Reserva de turnos

El socio puede reservar según los días permitidos del plan.

Se crean registros en orden_turno con validación de cupos y horario.

8️⃣ Check-in en el gimnasio

Se realiza check-in (checkin).

El sistema valida:

Que la suscripcion esté activa.

Que tenga turno reservado.

Se actualiza el registro de asistencia.

## 🧱 Arquitectura del Proyecto
```bash
Gym/
├── Api/ → Backend (ASP.NET Core 9.0, Web API, EF Core, MariaDB)
│ ├── Context/ → Script de base de datos (MySQL/MariaDB)
│ ├── Controllers/ → Controladores REST
│ ├── Data/ → Contexto EF Core y modelos
│ ├── Services/ → Servicios auxiliares (archivos, storage, etc.)
│ ├── Program.cs → Configuración principal
│ ├── appsettings.json
│ └── Api.csproj
│
├── frontend (React + Vite + Tailwind)
│ ├── src/
│ ├── public/
│ ├── package.json
│ └── vite.config.ts
│
└── start-gym.bat
```
---
## ⚙️ Tecnologías utilizadas

| Capa | Tecnología |
|------|-------------|
| **Lenguaje** | C# (.NET 9.0) |
| **Framework Web** | ASP.NET Core Web API |
| **Frontend** | React + Vite + Tailwind CSS |
| **Base de datos** | MariaDB 10.4.32 |
| **ORM** | Entity Framework Core |
| **Autenticación** | JWT (JSON Web Token) |
| **Documentación API** | Swagger + Postman |
| **Estilo API** | RESTful |
| **Control de versiones** | Git + GitHub |

---

## 🧩 Estructura del Modelo de Datos

El sistema contiene más de 10 entidades relacionadas, cumpliendo con el requisito de “al menos 4 clases/tablas relacionadas con relación 1:N”.

**Principales entidades:**
- `usuario` → maneja autenticación, roles y estado.
- `socio` → datos del cliente del gimnasio.
- `personal` → personal de entrenamiento.
- `plan` → tipo de plan contratado.
- `suscripcion` → vínculo entre socio y plan.
- `rutina_plantilla` → ejercicios predefinidos por profesor.
- `ejercicio` → actividades con carga y repeticiones.
- `comprobante` → archivo de comprobantes de pago.

**Relaciones destacadas:**
- Un **plan** tiene muchos **socios**.  
- Un **socio** puede tener muchas **suscripciones**.  
- Un **personal** diseña muchas **rutinas**.  
- Una **rutina** contiene muchos **ejercicios**.

---

## 🔐 Seguridad y Roles

- El modelo `usuario` incluye campos:  
  ```csharp
  public string email { get; set; }
  public string password_hash { get; set; }
  public string rol { get; set; } // Admin, Profesor, Socio
  public string estado { get; set; }

Login basado en JWT (JSON Web Token).

Endpoints protegidos con [Authorize(Roles="Admin")].

Roles:

- Administrador → CRUD completo.

- Recepcionesta → manejo de cobro y horarios.

- Profesor → Rutinas y turnos (aun no implementado).

- Socio → Consultas personales (aun no implementado).

🖼️ Manejo de Archivos

Implementado mediante los servicios:

- IFileStorage.cs

- LocalFileStorage.cs

Permite almacenar comprobantes o archivos relacionados.

Campo avatar_url en usuario (para imagen de perfil).

⚛️ CRUD React + AJAX

El frontend está desarrollado con React + Vite + Tailwind.
Usa peticiones AJAX (axios/fetch) al backend, logrando una interfaz dinámica y moderna.
Uno de los ABM (por ejemplo, Planes o Socios) cumple completamente el requisito de CRUD vía API.

📄 Paginación y Búsqueda

Paginado real: cada endpoint devuelve solo la página solicitada.

var socios = _context.Socios
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToList();

Búsqueda AJAX:
/api/socios/buscar?q=juan devuelve coincidencias dinámicamente (ideal para selects en el frontend).


🧪 Pruebas y Colección Postman

Iniciar el backend con: dotnet run 

Acceder a Swagger:
👉 http://localhost:5144/swagger

Exportar la colección desde Postman: docs/GymAPI.postman_collection.json

| Rol           | Email                                 | Contraseña |
| ------------- | ------------------------------------- | ---------- |
| Administrador | [admin@gym.com](mailto:admin@gym.com) | admin123   |
| Recepcionista | [rece@gym.com](mailto:rece@gym.com)   | rece123    |
| Socio         | [socio@gym.com](mailto:socio@gym.com) | Socio123   |


## ✅ Cumplimiento de los Requerimientos

| # | Requisito | Implementado en / Descripción |
|---|------------|-------------------------------|
| 1 | 4+ clases/tablas con relación 1:N | `Socio`, `Plan`, `Suscripcion`, `Usuario`, `TurnoPlantilla` — relaciones gestionadas por EF Core |
| 2 | Seguridad con login y roles | JWT + `[Authorize(Roles="...")]` en controladores (`UsuariosController`, `PerfilController`) |
| 3 | Avatar en usuarios | Subida en `/perfil/{id}/avatar` + guardado en `/uploads/avatars` |
| 4 | Archivos adicionales | Subida de comprobantes (`OrdenPagoController`, `/uploads/comprobantes`) |
| 5 | ABM con React + AJAX | (en planes entre otras vistas) |
| 6 | Listados con paginado real | `SociosController`, `SuscripcionesController`, `UsuariosController` con `Skip()` / `Take()` |
| 7 | Selección con búsqueda AJAX | `Select2` / `react-select` en formularios (`Turnos`, `Suscripciones`) |
| 8 | API con JWT | Configurada en `Program.cs`, autenticación en todos los controladores |
| 9 | `.gitignore` | Incluye `/bin`, `/obj`, `/node_modules`, `/wwwroot/uploads` |
| 10 | Diagrama ER o de clases | Incluido en `Api/Context/` |
| 11 | README.md descriptivo | Este archivo 😉 |
| 12 | Usuarios por rol | Admin, Profesor y Socio definidos en tabla de ejemplo |
| 13 | Base de datos | Incluido en `Api/Context/` |
| 14 | Colección Postman | Incluido en `Api/Context/` |


🚀 Instrucciones de Ejecución
🔧 Backend
cd Gym/Api
  dotnet run

⚛️ Frontend
cd Gym/frontend
  npm run dev

o... cd Gym  
  .\start-gym.bat

Abrir en el navegador:
👉 http://localhost:5173

📘 Autor

Romanela Ricchiardi

Laboratorio de programacion II .NET

Tecnicatura Universitaria en Desarrollo de Software — Universidad de La Punta (ULP)

📧 roma.ricchiardi@gmail.com

💼 GitHub: [RomaRicchi](https://github.com/RomaRicchi)

## 🖥️ Vista del Sistema

<img width="1913" height="869" alt="image" src="https://github.com/user-attachments/assets/1e1b197c-4d4a-45c3-9736-2970085feec3" />
<img width="1895" height="880" alt="image" src="https://github.com/user-attachments/assets/923ab56c-114e-4bc4-8d8d-17583b5b3125" />
<img width="1913" height="860" alt="image" src="https://github.com/user-attachments/assets/2df13ed3-80c5-40a0-acd5-44c9ea2eca93" />
<img width="1906" height="868" alt="image" src="https://github.com/user-attachments/assets/41df292e-3eb8-4572-b8a6-f010a04d9f78" />
<img width="1906" height="866" alt="image" src="https://github.com/user-attachments/assets/094135b2-d838-47ad-800f-0842d7511d5b" />


