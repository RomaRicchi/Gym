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
          <label class="form-label">Cupo</label>
          <input id="cupo" type="number" class="form-control" min="1" placeholder="10"/>
        </div>
        <div class="form-check mb-3 text-start">
          <input id="activa" type="checkbox" class="form-check-input" checked/>
          <label class="form-check-label" for="activa">Activa</label>
        </div>
      </div>
    `,
    showCancelButton: true,
    confirmButtonText: "Guardar",
    cancelButtonText: "Cancelar",
    preConfirm: () => {
      const nombre = (document.getElementById("nombre") as HTMLInputElement).value.trim();
      const cupoStr = (document.getElementById("cupo") as HTMLInputElement).value;
      const activa = (document.getElementById("activa") as HTMLInputElement).checked;

      if (!nombre) {
        Swal.showValidationMessage("El nombre es obligatorio");
        return null;
      }

      const cupo = Number(cupoStr);
      if (!Number.isFinite(cupo) || cupo < 1) {
        Swal.showValidationMessage("El cupo debe ser un número válido mayor a 0");
        return null;
      }

      // devolvemos un objeto válido
      return { nombre, cupo, activa };
    },
  });

  if (formValues) {
    try {
      await gymApi.post("/salas", formValues);
      await Swal.fire("✅ Guardada", "Sala creada correctamente", "success");
      onSuccess?.();
    } catch (err: any) {
      const message =
        err.response?.data?.message ||
        err.response?.data?.error ||
        "No se pudo crear la sala";
      await Swal.fire("❌ Error", message, "error");
    }
  }
}
