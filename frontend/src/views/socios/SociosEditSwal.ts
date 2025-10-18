import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface SocioForm {
  dni: string;
  nombre: string;
  email: string;
  telefono: string;
  activo: boolean;
}

export async function mostrarFormEditarSocio(id: number): Promise<boolean> {
  try {
    // 🔹 Cargar los datos actuales del socio
    const res = await gymApi.get(`/socios/${id}`);
    const socio: SocioForm = res.data;

    const { value: formValues } = await Swal.fire<SocioForm>({
      title: "✏️ Editar Socio",
      html: `
        <form id="form-editar-socio" style="text-align:left;overflow-x:hidden;margin-top:0.5rem;">
          <div style="margin-bottom:0.8rem;">
            <label for="dni" style="display:block;font-weight:600;color:#222;margin-bottom:0.3rem;">DNI</label>
            <input id="dni" type="text" value="${socio.dni || ""}" placeholder="Ingrese DNI"
              style="width:100%;background:#fff;color:#222;border:1px solid #ccc;border-radius:6px;
                     padding:0.6rem 0.8rem;font-size:0.95rem;box-sizing:border-box;">
          </div>

          <div style="margin-bottom:0.8rem;">
            <label for="nombre" style="display:block;font-weight:600;color:#222;margin-bottom:0.3rem;">Nombre</label>
            <input id="nombre" type="text" value="${socio.nombre || ""}" placeholder="Ingrese nombre"
              style="width:100%;background:#fff;color:#222;border:1px solid #ccc;border-radius:6px;
                     padding:0.6rem 0.8rem;font-size:0.95rem;box-sizing:border-box;">
          </div>

          <div style="margin-bottom:0.8rem;">
            <label for="email" style="display:block;font-weight:600;color:#222;margin-bottom:0.3rem;">Email</label>
            <input id="email" type="email" value="${socio.email || ""}" placeholder="Ingrese email"
              style="width:100%;background:#fff;color:#222;border:1px solid #ccc;border-radius:6px;
                     padding:0.6rem 0.8rem;font-size:0.95rem;box-sizing:border-box;">
          </div>

          <div style="margin-bottom:0.8rem;">
            <label for="telefono" style="display:block;font-weight:600;color:#222;margin-bottom:0.3rem;">Teléfono</label>
            <input id="telefono" type="text" value="${socio.telefono || ""}" placeholder="Ingrese teléfono"
              style="width:100%;background:#fff;color:#222;border:1px solid #ccc;border-radius:6px;
                     padding:0.6rem 0.8rem;font-size:0.95rem;box-sizing:border-box;">
          </div>

          <div style="display:flex;align-items:center;gap:0.5rem;margin-top:0.5rem;">
            <input type="checkbox" id="activo" ${socio.activo ? "checked" : ""}>
            <label for="activo" style="font-weight:600;color:#222;margin:0;">Activo</label>
          </div>
        </form>
      `,
      focusConfirm: false,
      showCancelButton: true,
      confirmButtonText: "💾 Guardar Cambios",
      cancelButtonText: "Cancelar",

      didOpen: () => {
        const popup = Swal.getPopup();
        if (popup) {
          popup.style.overflowX = "hidden";
          popup.style.maxWidth = "460px";
          popup.style.textAlign = "left";
        }

        // 🔧 Eliminar estilos nativos de SweetAlert2 para inputs
        document.querySelectorAll<HTMLInputElement>("#form-editar-socio input").forEach((input) => {
          input.classList.remove("swal2-input");
        });
      },

      preConfirm: () => {
        const dni = (document.getElementById("dni") as HTMLInputElement)?.value.trim();
        const nombre = (document.getElementById("nombre") as HTMLInputElement)?.value.trim();
        const email = (document.getElementById("email") as HTMLInputElement)?.value.trim();
        const telefono = (document.getElementById("telefono") as HTMLInputElement)?.value.trim();
        const activo = (document.getElementById("activo") as HTMLInputElement)?.checked ?? true;

        if (!dni || !nombre || !email) {
          Swal.showValidationMessage("DNI, Nombre y Email son obligatorios");
          return;
        }

        return { dni, nombre, email, telefono, activo };
      },
    });

    if (!formValues) return false;

    // 🔸 Enviar actualización al backend
    await gymApi.put(`/socios/${id}`, formValues);

    await Swal.fire({
      icon: "success",
      title: "✅ Cambios guardados",
      text: "El socio fue actualizado correctamente.",
      timer: 1600,
      showConfirmButton: false,
    });

    return true;
  } catch (err) {
    console.error(err);
    await Swal.fire({
      icon: "error",
      title: "Error",
      text: "No se pudo actualizar el socio.",
    });
    return false;
  }
}
