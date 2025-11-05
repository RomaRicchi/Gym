import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export async function RutinaAsignadaCreateSwal(onSuccess?: () => void) {
  const { value: formValues } = await Swal.fire({
    title: "Asignar Rutina",
    html: `
      <input id="rutinaPlantillaId" class="swal2-input" placeholder="ID Rutina Plantilla">
      <input id="socioId" class="swal2-input" placeholder="ID Socio">
      <input id="personalId" class="swal2-input" placeholder="ID Profesor">
      <input id="fechaAsignacion" type="date" class="swal2-input" value="${new Date().toISOString().slice(0, 10)}">
      <textarea id="observaciones" class="swal2-textarea" placeholder="Observaciones (opcional)"></textarea>
    `,
    focusConfirm: false,
    showCancelButton: true,
    confirmButtonText: "Guardar",
    preConfirm: () => {
      return {
        idRutinaPlantilla: Number((document.getElementById("rutinaPlantillaId") as HTMLInputElement).value),
        idSocio: Number((document.getElementById("socioId") as HTMLInputElement).value),
        idPersonal: Number((document.getElementById("personalId") as HTMLInputElement).value),
        fechaAsignacion: (document.getElementById("fechaAsignacion") as HTMLInputElement).value,
        observaciones: (document.getElementById("observaciones") as HTMLTextAreaElement).value,
      };
    },
  });

  if (formValues) {
    try {
      await gymApi.post("/rutinaasignada", formValues);
      Swal.fire("Éxito", "Rutina asignada correctamente", "success");
      onSuccess?.();
    } catch {
      Swal.fire("Error", "No se pudo asignar la rutina", "error");
    }
  }
}
