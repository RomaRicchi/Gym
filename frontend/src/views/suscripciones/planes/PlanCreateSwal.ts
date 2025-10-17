import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export async function PlanCreateSwal(onSuccess?: () => void) {
  const { value: formValues } = await Swal.fire({
    title: "➕ Nuevo Plan",
    html: `
      <div class="swal2-card-style">
        <div class="mb-3 text-start">
          <label class="form-label">Nombre</label>
          <input id="nombre" type="text" class="form-control" placeholder="Ej: Plan mensual"/>
        </div>
        <div class="mb-3 text-start">
          <label class="form-label">Días por semana</label>
          <input id="dias_por_semana" type="number" class="form-control" min="1" max="7" placeholder="3"/>
        </div>
        <div class="mb-3 text-start">
          <label class="form-label">Precio</label>
          <input id="precio" type="number" class="form-control" min="0" step="0.01" placeholder="10000"/>
        </div>
        <div class="form-check text-start">
          <input id="activo" type="checkbox" class="form-check-input" checked/>
          <label class="form-check-label" for="activo">Activo</label>
        </div>
      </div>
    `,
    showCancelButton: true,
    confirmButtonText: "Guardar",
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
      Swal.fire("Guardado", "Plan creado correctamente", "success");
      onSuccess?.();
    } catch {
      Swal.fire("Error", "No se pudo crear el plan", "error");
    }
  }
}
