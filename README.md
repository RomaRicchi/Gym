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

El socio elige un plan (plan_id).

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

El socio realiza check-in (checkin).

El sistema valida:

Que la suscripcion esté activa.

Que tenga turno reservado.

Se actualiza el registro de asistencia.

## 🧱 Arquitectura del Proyecto

Gym/
├── GymApi/ → Backend (ASP.NET Core 9.0, Web API, EF Core, MariaDB)
│ ├── Controllers/ → Controladores REST
│ ├── Data/ → Contexto EF Core y modelos
│ ├── Services/ → Servicios auxiliares (archivos, storage, etc.)
│ ├── Program.cs → Configuración principal
│ ├── appsettings.json
│ └── GymApi.csproj
│
├── gym-web/ → Frontend (React + Vite + Tailwind)
│ ├── src/
│ ├── public/
│ ├── package.json
│ └── vite.config.ts
│
├── backup_qym_oram.sql → Script de base de datos (MySQL/MariaDB)
└── Sistema de Gestión para Gimnasios.docx → Documentación técnica original

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

- Profesor → Rutinas y turnos.

- Socio → Consultas personales.

🖼️ Manejo de Archivos

Implementado mediante los servicios:

- IFileStorage.cs

- LocalFileStorage.cs

Permite almacenar comprobantes o archivos relacionados.

Pendiente: campo avatar_url en usuario (para imagen de perfil).

⚛️ CRUD React + AJAX

El frontend (carpeta gym-web) está desarrollado con React + Vite + Tailwind.
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

🔑 API con JWT

Configuración recomendada (a implementar):

En Program.cs:

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
En appsettings.json:
"Jwt": {
  "Key": "ClaveSuperSecretaParaGym123!"
}

🧪 Pruebas y Colección Postman

Iniciar el backend con: dotnet run --project GymApi

dotnet run --project GymApi
Acceder a Swagger:
👉 http://localhost:5000/swagger

Exportar la colección desde Postman: docs/GymAPI.postman_collection.json

| Rol           | Email                                 | Contraseña |
| ------------- | ------------------------------------- | ---------- |
| Administrador | [admin@gym.com](mailto:admin@gym.com) | admin123   |
| Profesor      | [profe@gym.com](mailto:profe@gym.com) | profe123   |
| Socio         | [socio@gym.com](mailto:socio@gym.com) | socio123   |

🚀 Instrucciones de Ejecución
🔧 Backend
cd Gym/GymApi
dotnet restore
dotnet ef database update
dotnet run

⚛️ Frontend
cd Gym/gym-web
npm install
npm run dev


Abrir en el navegador:
👉 http://localhost:5173

📘 Autor

Romanela Ricchiardi

Laboratorio de programacion II .NET

Tecnicatura Universitaria en Desarrollo de Software — Universidad de La Punta (ULP)

📧 roma.ricchiardi@gmail.com

💼 GitHub: [RomaRicchi](https://github.com/RomaRicchi)


