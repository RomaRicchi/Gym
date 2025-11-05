import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export async function EvaluacionCreateSwal(onSuccess?: () => void) {
  const { value: formValues } = await Swal.fire({
    title: "Nueva Evaluación",
    html: `
      <input id="idRutinaAsignada" class="swal2-input" placeholder="ID Rutina Asignada">
      <input id="idPersonal" class="swal2-input" placeholder="ID Profesor">
      <input id="fechaEvaluacion" type="date" class="swal2-input" value="${new Date().toISOString().slice(0, 10)}">
      <textarea id="observaciones" class="swal2-textarea" placeholder="Observaciones (opcional)"></textarea>
    `,
    showCancelButton: true,
    confirmButtonText: "Guardar",
    focusConfirm: false,
    preConfirm: () => {
      return {
        idRutinaAsignada: Number((document.getElementById("idRutinaAsignada") as HTMLInputElement).value),
        idPersonal: Number((document.getElementById("idPersonal") as HTMLInputElement).value),
        fechaEvaluacion: (document.getElementById("fechaEvaluacion") as HTMLInputElement).value,
        observaciones: (document.getElementById("observaciones") as HTMLTextAreaElement).value,
      };
    },
  });

  if (formValues) {
    try {
      await gymApi.post("/evaluaciones", formValues);
      Swal.fire("Éxito", "Evaluación registrada correctamente", "success");
      onSuccess?.();
    } catch {
      Swal.fire("Error", "No se pudo registrar la evaluación", "error");
    }
  }
}
