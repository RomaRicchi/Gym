import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export async function PlanCreateSwal(onSuccess?: () => void) {
  const { value: formValues } = await Swal.fire({
    title: "➕ Nuevo Plan",
    html: `
      <form id="form-crear-plan" style="text-align:left;overflow-x:hidden;margin-top:0.5rem;">
        <div style="margin-bottom:0.8rem;">
          <label for="nombre" style="display:block;font-weight:600;color:#222;margin-bottom:0.3rem;">Nombre</label>
          <input id="nombre" type="text" placeholder="Ej: Plan mensual"
            style="width:100%;background:#fff;color:#222;border:1px solid #ccc;border-radius:6px;
                   padding:0.7rem 1rem;font-size:1rem;box-sizing:border-box;">
        </div>

        <div style="margin-bottom:0.8rem;">
          <label for="dias_por_semana" style="display:block;font-weight:600;color:#222;margin-bottom:0.3rem;">Días por semana</label>
          <input id="dias_por_semana" type="number" min="1" max="7" placeholder="3"
            style="width:100%;background:#fff;color:#222;border:1px solid #ccc;border-radius:6px;
                   padding:0.7rem 1rem;font-size:1rem;box-sizing:border-box;">
        </div>

        <div style="margin-bottom:0.8rem;">
          <label for="precio" style="display:block;font-weight:600;color:#222;margin-bottom:0.3rem;">Precio</label>
          <input id="precio" type="number" min="0" step="0.01" placeholder="10000"
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
            checked
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
    confirmButtonText: "💾 Guardar",
    cancelButtonText: "Cancelar",
    focusConfirm: false,

    didOpen: () => {
      const popup = Swal.getPopup();
      if (popup) {
        popup.style.overflowX = "hidden";
        popup.style.maxWidth = "520px";
        popup.style.textAlign = "left";
      }

      // 🔧 Alinear y expandir el contenedor interno
      const htmlContainer = popup?.querySelector(".swal2-html-container") as HTMLElement;
      if (htmlContainer) {
        htmlContainer.style.width = "100%";
        htmlContainer.style.maxWidth = "none";
        htmlContainer.style.display = "block";
        htmlContainer.style.textAlign = "left";
      }

      // 🔧 Inputs a ancho completo
      document.querySelectorAll<HTMLInputElement>("#form-crear-plan input").forEach((input) => {
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
        Swal.showValidationMessage("Debe ingresar un nombre y precio válidos");
        return false;
      }

      return { nombre, dias_por_semana, precio, activo };
    },
  });

  if (formValues) {
    try {
      await gymApi.post("/planes", formValues);

      await Swal.fire({
        icon: "success",
        title: "✅ Plan creado",
        text: "El nuevo plan se guardó correctamente.",
        timer: 1600,
        showConfirmButton: false,
      });

      onSuccess?.();
    } catch (err) {
      console.error(err);
      Swal.fire("Error", "No se pudo crear el plan", "error");
    }
  }
}
