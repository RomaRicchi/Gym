// @ts-nocheck
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-card.css";

/**
 * Abre un Swal para registrar el check-in de un socio con observaciones del profesor.
 * @param socioId ID del socio
 * @param turnoPlantillaId ID del turno plantilla
 * @param socioNombre Nombre del socio (para mostrar en el título)
 * @param onSuccess Callback opcional para refrescar datos
 */
export async function CheckinSwal(
  socioId: number,
  turnoPlantillaId: number,
  socioNombre: string,
  onSuccess?: () => void
) {
  try {
    // 🔹 Paso 1: abrir formulario con textarea
    const { value: observaciones } = await Swal.fire({
      title: `<i class="fa-solid fa-user-check"></i> <strong>Check-in de ${socioNombre}</strong>`,
      html: `
        <p class="mb-2">Podés dejar observaciones sobre el desempeño del alumno:</p>
        <textarea id="obsInput" class="swal2-textarea" rows="4"
          placeholder="Ejemplo: Buena técnica en sentadillas, mejorar postura en press militar."></textarea>
      `,
      showCancelButton: true,
      confirmButtonText: "Guardar asistencia",
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#ff6600",
      background: "#ffa940",
      color: "#222",
      customClass: {
        popup: "swal2-card-style",
        confirmButton: "btn btn-orange mx-2",
        cancelButton: "btn btn-secondary mx-2",
      },
      focusConfirm: false,
      preConfirm: () => {
        const obs = (document.getElementById("obsInput") as HTMLTextAreaElement)
          ?.value;
        return obs?.trim() || null;
      },
    });

    if (observaciones === undefined) return; // usuario canceló

    // 🔹 Paso 2: mostrar loading mientras se guarda
    Swal.fire({
      title: "Guardando asistencia...",
      allowOutsideClick: false,
      didOpen: () => Swal.showLoading(),
      background: "#ffa940",
      showConfirmButton: false,
    });

    // 🔹 Paso 3: registrar el check-in
    await gymApi.post("/Checkin", {
      socioId,
      turnoPlantillaId,
      observaciones, // si el backend lo soporta
    });

    // 🔹 Paso 4: éxito
    Swal.fire({
      icon: "success",
      title: "✅ Check-in registrado",
      text: observaciones
        ? "Asistencia guardada con observaciones."
        : "Asistencia registrada correctamente.",
      confirmButtonColor: "#ff6600",
    });

    if (onSuccess) onSuccess();
  } catch (error: any) {
    Swal.fire(
      "Error",
      error.response?.data?.message || "No se pudo registrar el check-in.",
      "error"
    );
  }
}
