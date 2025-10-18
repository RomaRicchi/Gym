import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface SuscripcionForm {
  socio_id: string;
  plan_id: string;
  inicio: string;
  fin: string;
  estado: boolean;
}

export async function mostrarFormEditarSuscripcion(id: number): Promise<boolean> {
  try {
    // 🔹 Cargar datos actuales
    const [resSuscripcion, resSocios, resPlanes] = await Promise.all([
      gymApi.get(`/suscripciones/${id}`),
      gymApi.get("/socios"),
      gymApi.get("/planes"),
    ]);

    const s = resSuscripcion.data;
    const socios = resSocios.data.items || resSocios.data;
    const planes = resPlanes.data.items || resPlanes.data;

    // 🧩 Modal
    const { value: formValues } = await Swal.fire<SuscripcionForm>({
      title: "✏️ Editar Suscripción",
      html: `
        <form id="form-editar-suscripcion" style="text-align:left;overflow-x:hidden;margin-top:0.5rem;">
          <div style="margin-bottom:0.8rem;">
            <label for="socio_id" style="display:block;font-weight:600;color:#222;margin-bottom:0.3rem;">Socio</label>
            <select id="socio_id" style="width:100%;background:#fff;color:#222;border:1px solid #ccc;border-radius:6px;padding:0.6rem;font-size:0.95rem;">
              <option value="">Seleccionar socio...</option>
              ${socios
                .map(
                  (soc: any) =>
                    `<option value="${soc.id}" ${
                      soc.id === s.socio_id ? "selected" : ""
                    }>${soc.nombre}</option>`
                )
                .join("")}
            </select>
          </div>

          <div style="margin-bottom:0.8rem;">
            <label for="plan_id" style="display:block;font-weight:600;color:#222;margin-bottom:0.3rem;">Plan</label>
            <select id="plan_id" style="width:100%;background:#fff;color:#222;border:1px solid #ccc;border-radius:6px;padding:0.6rem;font-size:0.95rem;">
              <option value="">Seleccionar plan...</option>
              ${planes
                .map(
                  (p: any) =>
                    `<option value="${p.id}" ${
                      p.id === s.plan_id ? "selected" : ""
                    }>${p.nombre}</option>`
                )
                .join("")}
            </select>
          </div>

          <div style="margin-bottom:0.8rem;">
            <label for="inicio" style="display:block;font-weight:600;color:#222;margin-bottom:0.3rem;">Inicio</label>
            <input id="inicio" type="date" value="${
              s.inicio?.split("T")[0] || ""
            }"
              style="width:100%;background:#fff;color:#222;border:1px solid #ccc;border-radius:6px;padding:0.6rem;font-size:0.95rem;">
          </div>

          <div style="margin-bottom:0.8rem;">
            <label for="fin" style="display:block;font-weight:600;color:#222;margin-bottom:0.3rem;">Fin</label>
            <input id="fin" type="date" value="${
              s.fin?.split("T")[0] || ""
            }"
              style="width:100%;background:#fff;color:#222;border:1px solid #ccc;border-radius:6px;padding:0.6rem;font-size:0.95rem;">
          </div>

          <div style="display:flex;align-items:center;gap:0.5rem;margin-top:0.5rem;">
            <input type="checkbox" id="estado" ${
              s.estado ? "checked" : ""
            }>
            <label for="estado" style="font-weight:600;color:#222;margin:0;">Activa</label>
          </div>
        </form>
      `,
      showCancelButton: true,
      confirmButtonText: "💾 Guardar cambios",
      cancelButtonText: "Cancelar",
      focusConfirm: false,

      didOpen: () => {
        const popup = Swal.getPopup();
        if (popup) {
          popup.style.overflowX = "hidden";
          popup.style.maxWidth = "480px";
          popup.style.textAlign = "left";
        }
        // 🔧 eliminar estilo default de SweetAlert2
        document.querySelectorAll<HTMLInputElement>("#form-editar-suscripcion input, #form-editar-suscripcion select").forEach((el) => {
          el.classList.remove("swal2-input");
        });
      },

      preConfirm: () => {
        const socio_id = (document.getElementById("socio_id") as HTMLSelectElement)?.value;
        const plan_id = (document.getElementById("plan_id") as HTMLSelectElement)?.value;
        const inicio = (document.getElementById("inicio") as HTMLInputElement)?.value;
        const fin = (document.getElementById("fin") as HTMLInputElement)?.value;
        const estado = (document.getElementById("estado") as HTMLInputElement)?.checked ?? false;

        if (!socio_id || !plan_id || !inicio || !fin) {
          Swal.showValidationMessage("Todos los campos son obligatorios");
          return;
        }

        if (new Date(fin) < new Date(inicio)) {
          Swal.showValidationMessage("La fecha de fin no puede ser anterior al inicio");
          return;
        }

        return { socio_id, plan_id, inicio, fin, estado };
      },
    });

    if (!formValues) return false;

    // 🔹 Guardar cambios
    await gymApi.put(`/suscripciones/${id}`, {
      ...formValues,
      estado: formValues.estado ? 1 : 0,
    });

    await Swal.fire({
      icon: "success",
      title: "✅ Suscripción actualizada",
      text: "Los cambios fueron guardados correctamente.",
      timer: 1600,
      showConfirmButton: false,
    });

    return true;
  } catch (err) {
    console.error(err);
    await Swal.fire({
      icon: "error",
      title: "Error",
      text: "No se pudieron cargar o guardar los datos.",
    });
    return false;
  }
}
