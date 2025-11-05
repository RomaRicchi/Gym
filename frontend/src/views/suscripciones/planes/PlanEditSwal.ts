import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-planes.css";

export async function PlanEditSwal(id: string, onSuccess?: () => void) {
  try {
    const res = await gymApi.get(`/planes/${id}`);
    const data = res.data;

    const { value: formValues } = await Swal.fire({
      title: "✏️ Editar Plan",
      html: `
        <form class="swal-form-main" id="form-editar-plan" style="text-align:left;overflow-x:hidden;margin-top:0.5rem;">
          <div style="margin-bottom:0.8rem;">
            <label for="nombre" style="display:block;font-weight:600;color:#222;margin-bottom:0.3rem;">Nombre</label>
            <input id="nombre" type="text" value="${data.nombre || ""}" placeholder="Ingrese nombre"
              style="width:100%;background:#fff;color:#222;border:1px solid #ccc;border-radius:6px;
                     padding:0.7rem 1rem;font-size:1rem;box-sizing:border-box;">
          </div>

          <div style="margin-bottom:0.8rem;">
            <label for="dias_por_semana" style="display:block;font-weight:600;color:#222;margin-bottom:0.3rem;">Días por semana</label>
            <input id="dias_por_semana" type="number" value="${data.dias_por_semana || 1}" min="1" max="7"
              style="width:100%;background:#fff;color:#222;border:1px solid #ccc;border-radius:6px;
                     padding:0.7rem 1rem;font-size:1rem;box-sizing:border-box;">
          </div>

          <div style="margin-bottom:0.8rem;">
            <label for="precio" style="display:block;font-weight:600;color:#222;margin-bottom:0.3rem;">Precio</label>
            <input id="precio" type="number" value="${data.precio || 0}" min="0" step="0.01" placeholder="Ingrese precio"
              style="width:100%;background:#fff;color:#222;border:1px solid #ccc;border-radius:6px;
                     padding:0.7rem 1rem;font-size:1rem;box-sizing:border-box;">
          </div>

          <div
            style="
              display:flex;
              align-items:center;
              gap:0.6rem;
              margin-top:0.8rem;
              white-space:nowrap;
              width:fit-content;
            "
          >
            <input
              type="checkbox"
              id="activo"
              ${data.activo ? "checked" : ""}
              style="transform: scale(1.3); accent-color:#ff6600; cursor:pointer; margin:0;"
            >
            <label
              for="activo"
              style="font-weight:600;color:#222;margin:0;line-height:1;"
            >
              Activo
            </label>
          </div>
        </form>
      `,
      showCancelButton: true,
      confirmButtonText: "💾 Guardar cambios",
      cancelButtonText: "Cancelar",
      focusConfirm: false,
      customClass: {
        popup: "swal2-card-main",
        confirmButton: "btn btn-orange",
        cancelButton: "btn btn-secondary",
      },
      buttonsStyling: false,

      didOpen: () => {
        const popup = Swal.getPopup();
        if (popup) {
          popup.style.overflowX = "hidden";
          popup.style.maxWidth = "520px";
          popup.style.textAlign = "left";
        }

        // 🔧 Forzar ancho completo en todos los inputs
        const htmlContainer = popup?.querySelector(".swal2-html-container") as HTMLElement;
        if (htmlContainer) {
          htmlContainer.style.width = "100%";
          htmlContainer.style.maxWidth = "none";
          htmlContainer.style.display = "block";
          htmlContainer.style.textAlign = "left";
        }

        document.querySelectorAll<HTMLInputElement>("#form-editar-plan input").forEach((input) => {
          input.classList.remove("swal2-input");
          input.style.width = "100%";
          input.style.margin = "0.3rem 0";
          input.style.display = "block";
          input.style.boxSizing = "border-box";
          input.style.maxWidth = "none";
          input.style.fontSize = "1rem";
          input.style.padding = "0.7rem 1rem";
        });
      },

      preConfirm: () => {
        const nombre = (document.getElementById("nombre") as HTMLInputElement).value.trim();
        const dias_por_semana = (document.getElementById("dias_por_semana") as HTMLInputElement).value;
        const precio = (document.getElementById("precio") as HTMLInputElement).value;
        const activo = (document.getElementById("activo") as HTMLInputElement).checked;

        if (!nombre || !precio) {
          Swal.showValidationMessage("Debe ingresar un nombre y un precio válidos");
          return false;
        }

        return { nombre, dias_por_semana, precio, activo };
      },
    });

    if (formValues) {
      await gymApi.put(`/planes/${id}`, formValues);

      await Swal.fire({
        icon: "success",
        title: "✅ Plan actualizado",
        text: "Los cambios fueron guardados correctamente.",
        timer: 1600,
        showConfirmButton: false,
      });

      onSuccess?.();
    }
  } catch (err) {
    console.error(err);
    Swal.fire("Error", "No se pudo cargar o actualizar el plan", "error");
  }
}
