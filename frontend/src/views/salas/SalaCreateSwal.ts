import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export async function SalaCreateSwal(onSuccess?: () => void) {
  const { value: formValues } = await Swal.fire({
    title: "➕ Nueva Sala",
    html: `
      <div class="swal2-card-style">
        <div class="mb-3 text-start">
          <label class="form-label">Nombre</label>
          <input id="nombre" type="text" class="form-control" placeholder="Ej: Sala de pesas"/>
        </div>
        <div class="mb-3 text-start">
          <label class="form-label">Capacidad</label>
          <input id="capacidad" type="number" class="form-control" min="1" placeholder="10"/>
        </div>
        <div class="form-check mb-3 text-start">
          <input id="activa" type="checkbox" class="form-check-input" checked/>
          <label class="form-check-label" for="activa">Activa</label>
        </div>
      </div>
    `,
    showCancelButton: true,
    confirmButtonText: "Guardar",
    preConfirm: () => {
      const nombre = (document.getElementById("nombre") as HTMLInputElement).value.trim();
      const capacidad = (document.getElementById("capacidad") as HTMLInputElement).value;
      const activa = (document.getElementById("activa") as HTMLInputElement).checked;

      if (!nombre) {
        Swal.showValidationMessage("El nombre es obligatorio");
        return false;
      }

      return { nombre, capacidad, activa };
    },
  });

  if (formValues) {
    try {
      await gymApi.post("/salas", formValues);
      Swal.fire("Guardada", "Sala creada correctamente", "success");
      onSuccess?.();
    } catch {
      Swal.fire("Error", "No se pudo crear la sala", "error");
    }
  }
}
