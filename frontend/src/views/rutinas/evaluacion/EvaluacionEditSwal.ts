import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export async function EvaluacionEditSwal(id: string, onSuccess?: () => void) {
  try {
    const { data } = await gymApi.get(`/evaluaciones/${id}`);

    const { value: formValues } = await Swal.fire({
      title: "Editar Evaluación",
      html: `
        <input id="fechaEvaluacion" type="date" class="swal2-input" value="${data.fechaEvaluacion?.split("T")[0] || ""}">
        <textarea id="observaciones" class="swal2-textarea" placeholder="Observaciones">${data.observaciones || ""}</textarea>
      `,
      showCancelButton: true,
      confirmButtonText: "Guardar cambios",
      preConfirm: () => {
        return {
          fechaEvaluacion: (document.getElementById("fechaEvaluacion") as HTMLInputElement).value,
          observaciones: (document.getElementById("observaciones") as HTMLTextAreaElement).value,
        };
      },
    });

    if (formValues) {
      await gymApi.put(`/evaluaciones/${id}`, formValues);
      Swal.fire("Actualizado", "Evaluación modificada correctamente", "success");
      onSuccess?.();
    }
  } catch {
    Swal.fire("Error", "No se pudo cargar la evaluación", "error");
  }
}
