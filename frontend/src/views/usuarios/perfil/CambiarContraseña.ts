import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export async function PasswordEditSwal(id: string, onSuccess?: () => void) {
  const { value: formValues } = await Swal.fire({
    title: "🔒 Cambiar Contraseña",
    html: `
      <div class="swal2-card-style text-start">
        <label class="form-label">Contraseña actual</label>
        <input id="actual" type="password" class="form-control" placeholder=" " />

        <label class="form-label mt-3">Nueva contraseña</label>
        <input id="nueva" type="password" class="form-control" placeholder=" " />

        <label class="form-label mt-3">Confirmar nueva contraseña</label>
        <input id="confirmar" type="password" class="form-control" placeholder=" " />
      </div>
    `,
    showCancelButton: true,
    confirmButtonText: "💾 Actualizar",
    cancelButtonText: "Cancelar",
    focusConfirm: false,

    preConfirm: () => {
      const actual = (document.getElementById("actual") as HTMLInputElement).value.trim();
      const nueva = (document.getElementById("nueva") as HTMLInputElement).value.trim();
      const confirmar = (document.getElementById("confirmar") as HTMLInputElement).value.trim();

      if (!actual || !nueva || !confirmar) {
        Swal.showValidationMessage("Debe completar todos los campos");
        return false;
      }

      if (nueva !== confirmar) {
        Swal.showValidationMessage("Las contraseñas nuevas no coinciden");
        return false;
      }

      if (nueva.length < 6) {
        Swal.showValidationMessage("La nueva contraseña debe tener al menos 6 caracteres");
        return false;
      }

      return { actual, nueva };
    },
  });

  if (!formValues) return;

  try {
    if (!id) {
      Swal.fire("Error", "No se encontró el usuario en sesión", "error");
      return;
    }

    await gymApi.patch(`/perfil/${id}/password`, {
      actual: formValues.actual,
      nueva: formValues.nueva,
    });

    await Swal.fire( "Contraseña actualizada correctamente", "success");

    onSuccess?.();
  } catch (err: any) {
    console.error(err);
    const msg = err.response?.data?.message || "No se pudo cambiar la contraseña";
    Swal.fire("Error", msg, "error");
  }
}
