import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export async function RutinaAsignadaEditSwal(id: string, onSuccess?: () => void) {
  try {
    const { data } = await gymApi.get(`/rutinaasignada/${id}`);

    const { value: formValues } = await Swal.fire({
      title: "Editar Rutina Asignada",
      html: `
        <input id="fechaAsignacion" type="date" class="swal2-input" value="${data.fechaAsignacion?.split("T")[0] || ""}">
        <textarea id="observaciones" class="swal2-textarea" placeholder="Observaciones">${data.observaciones || ""}</textarea>
      `,
      showCancelButton: true,
      confirmButtonText: "Guardar cambios",
      preConfirm: () => {
        return {
          fechaAsignacion: (document.getElementById("fechaAsignacion") as HTMLInputElement).value,
          observaciones: (document.getElementById("observaciones") as HTMLTextAreaElement).value,
        };
      },
    });

    if (formValues) {
      await gymApi.put(`/rutinaasignada/${id}`, formValues);
      Swal.fire("Actualizado", "Rutina modificada correctamente", "success");
      onSuccess?.();
    }
  } catch {
    Swal.fire("Error", "No se pudo cargar la rutina asignada", "error");
  }
}
