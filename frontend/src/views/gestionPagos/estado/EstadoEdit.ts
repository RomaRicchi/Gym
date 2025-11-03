// @ts-nocheck
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export async function editarEstado(id: number) {
  try {
    // 🔹 Obtener datos actuales
    const { data: estado } = await gymApi.get(`/estadoOrdenPago/${id}`);

    const { value: formValues } = await Swal.fire({
      title: "✏️ Editar Estado de Pago",
      html: `
        <form id="form-editar-estado" style="text-align:left;overflow-x:hidden;margin-top:0.5rem;">
          <div style="margin-bottom:0.8rem;">
            <label for="nombreInput" style="display:block;font-weight:600;color:#222;margin-bottom:0.3rem;">
              Nombre
            </label>
            <input id="nombreInput" type="text"
              value="${estado.nombre || ""}"
              placeholder="Nombre del estado"
              style="width:100%;background:#fff;color:#222;border:1px solid #ccc;border-radius:6px;
                     padding:0.7rem 1rem;font-size:1rem;box-sizing:border-box;">
          </div>

          <div style="margin-bottom:0.8rem;">
            <label for="descInput" style="display:block;font-weight:600;color:#222;margin-bottom:0.3rem;">
              Descripción
            </label>
            <textarea id="descInput" rows="3"
              placeholder="Descripción..."
              style="width:100%;background:#fff;color:#222;border:1px solid #ccc;border-radius:6px;
                     padding:0.7rem 1rem;font-size:1rem;box-sizing:border-box;resize:vertical;">${
                estado.descripcion || ""
              }</textarea>
          </div>
        </form>
      `,
      showCancelButton: true,
      confirmButtonText: "💾 Guardar cambios",
      cancelButtonText: "Cancelar",
      focusConfirm: false,
      confirmButtonColor: "#ff6b00",

      didOpen: () => {
        const popup = Swal.getPopup();
        if (popup) {
          popup.style.overflowX = "hidden";
          popup.style.maxWidth = "520px";
          popup.style.textAlign = "left";
        }

        const htmlContainer = popup?.querySelector(".swal2-html-container") as HTMLElement;
        if (htmlContainer) {
          htmlContainer.style.width = "100%";
          htmlContainer.style.maxWidth = "none";
          htmlContainer.style.display = "block";
          htmlContainer.style.textAlign = "left";
        }

        const form = document.getElementById("form-editar-estado") as HTMLElement;
        if (form) {
          form.style.width = "100%";
          form.style.maxWidth = "480px";
          form.style.display = "block";
        }

        // 🔧 Ajustar inputs y textarea
        document
          .querySelectorAll<HTMLInputElement | HTMLTextAreaElement>(
            "#form-editar-estado input, #form-editar-estado textarea"
          )
          .forEach((el) => {
            el.classList.remove("swal2-input");
            el.style.width = "100%";
            el.style.margin = "0.3rem 0";
            el.style.display = "block";
            el.style.boxSizing = "border-box";
            el.style.maxWidth = "none";
            el.style.fontSize = "1rem";
            el.style.padding = "0.7rem 1rem";
          });
      },

      preConfirm: () => {
        const nombre = (document.getElementById("nombreInput") as HTMLInputElement)?.value.trim();
        const descripcion = (document.getElementById("descInput") as HTMLTextAreaElement)?.value.trim();

        if (!nombre) {
          Swal.showValidationMessage("⚠️ El nombre es obligatorio");
          return;
        }

        return { nombre, descripcion };
      },
    });

    if (!formValues) return;

    // 🔹 Guardar cambios
    await gymApi.put(`/estadoOrdenPago/${id}`, formValues);

    await Swal.fire({
      icon: "success",
      title: "✅ Estado actualizado",
      text: "Los cambios fueron guardados correctamente.",
      confirmButtonColor: "#ff6b00",
      timer: 1600,
      showConfirmButton: false,
    });

    window.location.reload();
  } catch (err) {
    console.error(err);
    Swal.fire("❌ Error", "No se pudo actualizar el estado.", "error");
  }
}
