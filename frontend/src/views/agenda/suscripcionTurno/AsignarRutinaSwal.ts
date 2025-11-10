// @ts-nocheck
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-card.css";

export async function AsignarRutinaSwal(
  turnoId: number,
  socioNombre: string,
  onSuccess?: () => void
) {
  try {
    // 🔹 Obtener todas las rutinas sin paginación
    const { data } = await gymApi.get("/rutinasplantilla/all");
    let rutinas = data || [];

    // 🔹 Mostrar SweetAlert con buscador + lista
    const { value: rutinaSeleccionada } = await Swal.fire({
      title: `<i class="fa-solid fa-dumbbell"></i> <strong>Asignar rutina a ${socioNombre}</strong>`,
      html: `
        <input id="buscadorRutina" type="text" placeholder="Buscar rutina..." 
          class="swal2-input" style="margin-bottom:10px; width:100%; text-align:center;">
        <select id="rutinaSelect" size="6" class="swal2-input" 
          style="width:100%; height:auto; text-align-last:center;">
          ${rutinas
            .map(
              (r: any) =>
                `<option value="${r.id}">
                  ${r.nombre} ${
                    r.grupoMuscularNombre
                      ? `(${r.grupoMuscularNombre})`
                      : ""
                  }
                </option>`
            )
            .join("")}
        </select>
      `,
      showCancelButton: true,
      confirmButtonText: "Guardar",
      cancelButtonText: "Cancelar",
      background: "#ffa940",
      color: "#222",
      customClass: {
        popup: "swal2-card-style",
        confirmButton: "btn btn-orange mx-2",
        cancelButton: "btn btn-secondary mx-2",
      },
      focusConfirm: false,
      didOpen: () => {
        const input = document.getElementById(
          "buscadorRutina"
        ) as HTMLInputElement;
        const select = document.getElementById(
          "rutinaSelect"
        ) as HTMLSelectElement;

        // 🔍 Filtrar mientras se escribe
        input.addEventListener("input", () => {
          const term = input.value.toLowerCase();
          select.innerHTML = rutinas
            .filter((r: any) => r.nombre.toLowerCase().includes(term))
            .map(
              (r: any) =>
                `<option value="${r.id}">
                  ${r.nombre} ${
                    r.grupoMuscularNombre
                      ? `(${r.grupoMuscularNombre})`
                      : ""
                  }
                </option>`
            )
            .join("");
        });
      },
      preConfirm: () => {
        const select = document.getElementById(
          "rutinaSelect"
        ) as HTMLSelectElement;
        return select.value || null;
      },
    });

    if (!rutinaSeleccionada) return;

    Swal.fire({
      title: "Guardando...",
      allowOutsideClick: false,
      didOpen: () => Swal.showLoading(),
      background: "#ffa940",
      showConfirmButton: false,
    });

    await gymApi.patch(`/suscripcionturno/${turnoId}/rutina`, rutinaSeleccionada);

    Swal.fire({
      icon: "success",
      title: "Rutina asignada",
      text: "La rutina fue asignada correctamente al turno.",
      confirmButtonColor: "#ff6600",
    });

    if (onSuccess) onSuccess();
  } catch (error: any) {
    Swal.fire(
      "Error",
      error.response?.data?.message || "No se pudo asignar la rutina.",
      "error"
    );
  }
}
