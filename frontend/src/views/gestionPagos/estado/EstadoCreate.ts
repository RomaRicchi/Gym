
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/orden.css"; //  Estilo unificado con fondo naranja

export async function crearEstado() {
  try {
    const { value: formValues } = await Swal.fire({
      title: "➕ Nuevo Estado de Pago",
      width: 650,
      customClass: {
        popup: "swal2-card-style",       // fondo naranja redondeado
        confirmButton: "btn btn-orange", // botón naranja coherente
        cancelButton: "btn btn-secondary",
      },
      buttonsStyling: false,
      html: `
        <form class="swal-form">
          <div class="swal-input-group">
            <label class="swal-label">Nombre</label>
            <input 
              id="nombreInput"
              type="text"
              class="swal-field"
              placeholder="Ej: Pagado, Pendiente, Rechazado"
            >
          </div>

          <div class="swal-input-group">
            <label class="swal-label">Descripción</label>
            <textarea 
              id="descripcionInput"
              class="swal-textarea"
              placeholder="Descripción opcional..."
            ></textarea>
          </div>
        </form>
      `,
      showCancelButton: true,
      confirmButtonText: "💾 Guardar",
      cancelButtonText: "Cancelar",
      focusConfirm: false,

      preConfirm: () => {
        const nombre = (document.getElementById("nombreInput") as HTMLInputElement)?.value.trim();
        const descripcion = (document.getElementById("descripcionInput") as HTMLTextAreaElement)?.value.trim();

        if (!nombre) {
          Swal.showValidationMessage("⚠️ El nombre es obligatorio");
          const msg = document.querySelector(".swal2-validation-message");
          if (msg) {
            msg.setAttribute(
              "style",
              "background:#ff6600;color:#fff;border-radius:6px;padding:6px 12px;font-weight:600;"
            );
          }
          return false;
        }

        return { nombre, descripcion };
      },
    });

    if (!formValues) return;

    // ✅ Guardar en backend
    await gymApi.post("/estadoOrdenPago", formValues);

    await Swal.fire({
      icon: "success",
      title: "✅ Guardado",
      text: "Estado creado correctamente.",
      timer: 1500,
      showConfirmButton: false,
      customClass: { popup: "swal2-card-style swal-alert-simple" },
    });

    window.location.reload();
  } catch (error) {
    console.error(error);
    await Swal.fire({
      icon: "error",
      title: "❌ Error",
      text: "No se pudo crear el estado.",
      customClass: { popup: "swal2-card-style swal-alert-simple" },
    });
  }
}
