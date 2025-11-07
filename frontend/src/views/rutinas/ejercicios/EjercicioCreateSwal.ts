import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-card.css";

export async function EjercicioCreateSwal(onSuccess?: () => void) {
  try {
    // 🔹 Obtener grupos musculares
    const { data: grupos } = await gymApi.get("/grupomuscular");
    const lista = grupos.items || grupos;

    const options = lista
      .map((g: any) => `<option value="${g.id}">${g.nombre}</option>`)
      .join("");

    const { value: formValues } = await Swal.fire({
      title: "➕ Nuevo Ejercicio",
      html: `
        <form id="form-ejercicio" class="swal-form">
          <div class="mb-2 text-start">
            <label class="form-label fw-semibold">Nombre</label>
            <input id="nombre" class="form-control" placeholder="Nombre del ejercicio" required />
          </div>

          <div class="mb-2 text-start">
            <label class="form-label fw-semibold">Grupo Muscular</label>
            <select id="grupoMuscularId" class="form-select">
              <option value="" disabled selected>Seleccionar...</option>
              ${options}
            </select>
          </div>

          <div class="mb-2 text-start">
            <label class="form-label fw-semibold">Tips</label>
            <textarea id="tips" class="form-control" rows="2" placeholder="Consejos o técnica (opcional)"></textarea>
          </div>

          <div class="mb-2 text-start">
            <label class="form-label fw-semibold">Media URL</label>
            <input id="mediaUrl" class="form-control" placeholder="/img/press-banca.jpg" />
          </div>
        </form>
      `,
      focusConfirm: false,
      confirmButtonText: "Guardar",
      showCancelButton: true,
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#ff6600",
      preConfirm: () => {
        const nombre = (document.getElementById("nombre") as HTMLInputElement)?.value.trim();
        const grupoMuscularId = (document.getElementById("grupoMuscularId") as HTMLSelectElement)?.value;
        const tips = (document.getElementById("tips") as HTMLTextAreaElement)?.value.trim();
        const mediaUrl = (document.getElementById("mediaUrl") as HTMLInputElement)?.value.trim();

        if (!nombre || !grupoMuscularId) {
          Swal.showValidationMessage("⚠️ El nombre y el grupo muscular son obligatorios.");
          return false;
        }

        return { nombre, grupoMuscularId: Number(grupoMuscularId), tips, mediaUrl };
      },
    });

    if (formValues) {
      await gymApi.post("/ejercicios", formValues);
      Swal.fire("✅ Guardado", "Ejercicio creado correctamente.", "success");
      onSuccess?.();
    }
  } catch (err) {
    console.error(err);
    Swal.fire("Error", "No se pudo crear el ejercicio.", "error");
  }
}
