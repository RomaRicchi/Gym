import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

/**
 * Formulario SweetAlert2 para registrar nueva evolución física
 */
export async function mostrarFormularioEvolucion(socioId: number) {
  const { value: formValues, isConfirmed } = await Swal.fire({
    title: "Nuevo Registro Físico 💪",
    html: `
      <div class="swal2-card-style text-start">
        <label class="form-label fw-bold mt-2">Peso (kg)</label>
        <input id="peso" type="number" step="0.01" class="form-control" placeholder="Ej: 75.5">

        <label class="form-label fw-bold mt-3">Altura (cm)</label>
        <input id="altura" type="number" step="0.1" class="form-control" placeholder="Ej: 175">

        <label class="form-label fw-bold mt-3">Pecho (cm)</label>
        <input id="pecho" type="number" step="0.1" class="form-control">

        <label class="form-label fw-bold mt-3">Cintura (cm)</label>
        <input id="cintura" type="number" step="0.1" class="form-control">

        <label class="form-label fw-bold mt-3">Cadera (cm)</label>
        <input id="cadera" type="number" step="0.1" class="form-control">

        <label class="form-label fw-bold mt-3">Brazo (cm)</label>
        <input id="brazo" type="number" step="0.1" class="form-control">

        <label class="form-label fw-bold mt-3">Pierna (cm)</label>
        <input id="pierna" type="number" step="0.1" class="form-control">

        <label class="form-label fw-bold mt-3">Gemelo (cm)</label>
        <input id="gemelo" type="number" step="0.1" class="form-control">

        <label class="form-label fw-bold mt-3">Observación</label>
        <textarea id="observacion" class="form-control" rows="2" placeholder="Opcional..."></textarea>
      </div>
    `,
    focusConfirm: false,
    confirmButtonText: "Guardar",
    confirmButtonColor: "#ff6b00",
    showCancelButton: true,
    cancelButtonText: "Cancelar",
    preConfirm: () => {
      const peso = parseFloat((document.getElementById("peso") as HTMLInputElement).value);
      const altura = parseFloat((document.getElementById("altura") as HTMLInputElement).value);

      if (isNaN(peso) || isNaN(altura)) {
        Swal.showValidationMessage("Debe ingresar peso y altura válidos");
        return null;
      }

      const pecho = parseFloat((document.getElementById("pecho") as HTMLInputElement).value) || null;
      const cintura = parseFloat((document.getElementById("cintura") as HTMLInputElement).value) || null;
      const cadera = parseFloat((document.getElementById("cadera") as HTMLInputElement).value) || null;
      const brazo = parseFloat((document.getElementById("brazo") as HTMLInputElement).value) || null;
      const pierna = parseFloat((document.getElementById("pierna") as HTMLInputElement).value) || null;
      const gemelo = parseFloat((document.getElementById("gemelo") as HTMLInputElement).value) || null;
      const observacion = (document.getElementById("observacion") as HTMLInputElement).value || null;

      return { peso, altura, pecho, cintura, cadera, brazo, pierna, gemelo, observacion };
    },
  });

  if (isConfirmed && formValues) {
    try {
      await gymApi.post("/evolucionfisica", {
        socioId,
        ...formValues,
      });

      await Swal.fire({
        icon: "success",
        title: "Registro guardado",
        text: "Se agregó la evolución física correctamente",
        confirmButtonColor: "#ff6b00",
      });
    } catch (error) {
      console.error("❌ Error al registrar evolución física:", error);
      Swal.fire({
        icon: "error",
        title: "Error",
        text: "No se pudo guardar el registro. Intente nuevamente.",
        confirmButtonColor: "#ff6b00",
      });
    }
  }

  return { isConfirmed };
}
