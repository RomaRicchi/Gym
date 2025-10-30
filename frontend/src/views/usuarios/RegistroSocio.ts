import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export async function handleRegistroSocio() {
  try {
    // 🔹 Paso 1 — Crear socio
    const { value: socioForm } = await Swal.fire({
      title: `<div class="fw-bold" style="font-size:1.4rem;display:flex;align-items:center;justify-content:center;gap:8px;">
                🏋️‍♂️ Registro de Socio
              </div>`,
      html: `
        <div style="display:flex;flex-direction:column;align-items:center;gap:12px;width:100%;">
          <div style="width:90%;text-align:left;">
            <label style="font-weight:600;color:#fff;">Nombre completo</label>
            <input id="s-nombre" class="swal2-input" type="text" placeholder="Ej: Juan Pérez"
              style="width:100%;background:#fff;border:none;border-radius:8px;color:#333;margin:6px 0;" />
          </div>
          <div style="width:90%;text-align:left;">
            <label style="font-weight:600;color:#fff;">DNI</label>
            <input id="s-dni" class="swal2-input" type="text" placeholder="Ej: 40123456"
              style="width:100%;background:#fff;border:none;border-radius:8px;color:#333;margin:6px 0;" />
          </div>
          <div style="width:90%;text-align:left;">
            <label style="font-weight:600;color:#fff;">Correo electrónico</label>
            <input id="s-email" class="swal2-input" type="email" placeholder="correo@ejemplo.com"
              style="width:100%;background:#fff;border:none;border-radius:8px;color:#333;margin:6px 0;" />
          </div>
          <div style="width:90%;text-align:left;">
            <label style="font-weight:600;color:#fff;">Teléfono (opcional)</label>
            <input id="s-telefono" class="swal2-input" type="text" placeholder="Ej: 2664000000"
              style="width:100%;background:#fff;border:none;border-radius:8px;color:#333;margin:6px 0;" />
          </div>
          <div style="width:90%;text-align:left;">
            <label style="font-weight:600;color:#fff;">Fecha de nacimiento</label>
            <input id="s-fecha" class="swal2-input" type="date"
              style="width:100%;background:#fff;border:none;border-radius:8px;color:#333;margin:6px 0;" />
          </div>
        </div>
      `,
      background: "#ff6b00",
      color: "#fff",
      confirmButtonText: "Continuar",
      cancelButtonText: "Cancelar",
      showCancelButton: true,
      focusConfirm: false,
      customClass: {
        popup: "swal2-card-style",
        confirmButton: "fitgym-btn",
      },
      preConfirm: () => {
        const nombre = (document.getElementById("s-nombre") as HTMLInputElement).value.trim();
        const dni = (document.getElementById("s-dni") as HTMLInputElement).value.trim();
        const email = (document.getElementById("s-email") as HTMLInputElement).value.trim();
        const telefono = (document.getElementById("s-telefono") as HTMLInputElement).value.trim();
        const fechaNacimiento = (document.getElementById("s-fecha") as HTMLInputElement).value;

        if (!nombre || !dni || !email) {
          Swal.showValidationMessage("Complete los campos obligatorios: nombre, DNI y email");
          return false;
        }
        return { nombre, dni, email, telefono, fechaNacimiento };
      },
    });

    if (!socioForm) return;

    Swal.fire({
      title: "Registrando socio...",
      background: "#ff6b00",
      color: "#fff",
      didOpen: () => Swal.showLoading(),
      showConfirmButton: false,
    });

    const socioRes = await gymApi.post("/socios/registro-publico", {
      nombre: socioForm.nombre,
      dni: socioForm.dni,
      email: socioForm.email,
      telefono: socioForm.telefono,
      fechaNacimiento: socioForm.fechaNacimiento || null,
    });

    const socioId = socioRes.data.id;
    Swal.close();

    // 🔹 Paso 2 — Crear usuario vinculado
    const { value: userForm } = await Swal.fire({
      title: `<div class="fw-bold" style="font-size:1.3rem;">👤 Crear cuenta de acceso</div>`,
      html: `
        <div style="display:flex;flex-direction:column;align-items:center;gap:12px;width:100%;">
          <div style="width:90%;text-align:left;">
            <label style="font-weight:600;color:#fff;">Alias de usuario</label>
            <input id="u-alias" class="swal2-input" placeholder="Ej: juanp"
              style="width:100%;background:#fff;border:none;border-radius:8px;color:#333;margin:6px 0;" />
          </div>
          <div style="width:90%;text-align:left;">
            <label style="font-weight:600;color:#fff;">Contraseña</label>
            <input id="u-password" type="password" class="swal2-input" placeholder="********"
              style="width:100%;background:#fff;border:none;border-radius:8px;color:#333;margin:6px 0;" />
          </div>
        </div>
      `,
      background: "#ff6b00",
      color: "#fff",
      confirmButtonText: "Crear cuenta",
      cancelButtonText: "Cancelar",
      showCancelButton: true,
      focusConfirm: false,
      preConfirm: () => {
        const alias = (document.getElementById("u-alias") as HTMLInputElement).value.trim();
        const password = (document.getElementById("u-password") as HTMLInputElement).value.trim();
        if (!alias || !password) {
          Swal.showValidationMessage("Complete alias y contraseña");
          return false;
        }
        return { alias, password };
      },
    });

    if (!userForm) return;

    Swal.fire({
      title: "Creando usuario...",
      background: "#ff6b00",
      color: "#fff",
      didOpen: () => Swal.showLoading(),
      showConfirmButton: false,
    });

    await gymApi.post("/usuarios/register", {
      email: socioForm.email,
      alias: userForm.alias,
      password: userForm.password,
      socioId,
    });

    Swal.fire({
      icon: "success",
      title: "✅ Registro completado",
      text: "¡Tu cuenta fue creada exitosamente! Ya podés iniciar sesión.",
      background: "#ff6b00",
      color: "#fff",
      confirmButtonColor: "#fff",
    });
  } catch (err: any) {
    console.error(err);
    Swal.fire({
      icon: "error",
      title: "Error",
      text: err.response?.data?.message || "No se pudo completar el registro",
      background: "#ff6b00",
      color: "#fff",
      confirmButtonColor: "#fff",
    });
  }
}
