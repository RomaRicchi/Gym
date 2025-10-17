import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

// 🧱 Interfaz para los datos del socio
interface SocioForm {
  dni: string;
  nombre: string;
  email: string;
  telefono: string;
  activo: boolean;
}

export async function mostrarFormEditarSocio(id: number): Promise<boolean> {
  try {
    // 🔹 Cargar los datos del socio actual
    const res = await gymApi.get(`/socios/${id}`);
    const socio: SocioForm = res.data;

    const { value: formValues } = await Swal.fire<SocioForm>({
      title: "✏️ Editar Socio",
      html: `
        <div class="swal2-card-style text-start">
          <label class="form-label mt-2">DNI</label>
          <input id="dni" class="swal2-input" value="${socio.dni}" placeholder="Ingrese DNI">

          <label class="form-label mt-2">Nombre</label>
          <input id="nombre" class="swal2-input" value="${socio.nombre}" placeholder="Ingrese nombre">

          <label class="form-label mt-2">Email</label>
          <input id="email" type="email" class="swal2-input" value="${socio.email}" placeholder="Ingrese email">

          <label class="form-label mt-2">Teléfono</label>
          <input id="telefono" class="swal2-input" value="${socio.telefono || ""}" placeholder="Ingrese teléfono">

          <div class="form-check mt-3">
            <input type="checkbox" id="activo" class="form-check-input" ${socio.activo ? "checked" : ""}>
            <label class="form-check-label" for="activo">Activo</label>
          </div>
        </div>
      `,
      focusConfirm: false,
      showCancelButton: true,
      confirmButtonText: "💾 Guardar Cambios",
      cancelButtonText: "Cancelar",
      preConfirm: () => {
        const dni = (document.getElementById("dni") as HTMLInputElement)?.value.trim();
        const nombre = (document.getElementById("nombre") as HTMLInputElement)?.value.trim();
        const email = (document.getElementById("email") as HTMLInputElement)?.value.trim();
        const telefono = (document.getElementById("telefono") as HTMLInputElement)?.value.trim();
        const activo = (document.getElementById("activo") as HTMLInputElement)?.checked ?? true;

        if (!dni || !nombre || !email)
          Swal.showValidationMessage("DNI, Nombre y Email son obligatorios");

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
