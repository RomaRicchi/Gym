import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-card.css";

export async function GrupoMuscularCreateSwal(onSuccess?: () => void) {
  try {
    const { value: formValues } = await Swal.fire({
      title: "➕ Nuevo Grupo Muscular",
      html: `
        <div class="mb-2 text-start">
          <label class="form-label fw-semibold">Nombre</label>
          <input id="nombre" class="form-control" placeholder="Ej: Pecho, Piernas, Espalda" />
        </div>
       
      `,
      confirmButtonText: "Guardar",
      showCancelButton: true,
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#ff6600",
      focusConfirm: false,
      preConfirm: () => {
        const nombre = (document.getElementById("nombre") as HTMLInputElement).value.trim();

        if (!nombre) {
          Swal.showValidationMessage("⚠️ El nombre es obligatorio.");
          return false;
        }

        return { nombre };
      },
    });

    if (formValues) {
      await gymApi.post("/grupomuscular", formValues);
      Swal.fire("✅ Guardado", "Grupo muscular creado correctamente.", "success");
      onSuccess?.();
    }
  } catch (err) {
    console.error(err);
    Swal.fire("Error", "No se pudo crear el grupo muscular.", "error");
  }
}
