import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-suscripcion.css";

interface SuscripcionForm {
  inicio: string;
  fin: string;
  estado: boolean;
}

export async function mostrarFormEditarSuscripcion(id: number): Promise<boolean> {
  try {
    // 🔹 Cargar datos necesarios
    const [resSuscripcion, resSocios, resPlanes] = await Promise.all([
      gymApi.get(`/suscripciones/${id}`),
      gymApi.get("/socios"),
      gymApi.get("/planes"),
    ]);

    const s = resSuscripcion.data;
    const socios = resSocios.data.items || resSocios.data;
    const planes = resPlanes.data.items || resPlanes.data;

    // 🧩 Buscar socio y plan con fallback a los nombres del backend
    const socio = socios.find((soc: any) => soc.id === s.socioId);
    const socioNombre = s.socioNombre || socio?.nombre || "(sin socio)";

    const plan = planes.find((p: any) => p.id === s.planId);
    const planNombre =
      s.planNombre ||
      (plan ? `${plan.nombre} — 💰 $${plan.precio?.toLocaleString("es-AR") || "0"}` : "(sin plan)");

    // 🧡 Modal SweetAlert2
    const { value: formValues } = await Swal.fire<SuscripcionForm>({
      title: "✏️ Editar Suscripción",
      html: `
        <form id="form-editar-suscripcion" style="text-align:left;overflow-x:hidden;margin-top:0.5rem;">
          
          <div style="margin-bottom:1rem;">
            <p style="font-weight:600;color:#222;margin:0;">
              Socio: <span style="font-weight:700;color:#000;">${socioNombre}</span>
            </p>
          </div>

          <div style="margin-bottom:1rem;">
            <p style="font-weight:600;color:#222;margin:0;">
              Plan: <span style="font-weight:700;color:#000;">${planNombre}</span>
            </p>
          </div>

          <div style="margin-bottom:0.8rem;">
            <label for="inicio" style="display:block;font-weight:600;color:#222;margin-bottom:0.3rem;">Fecha de inicio</label>
            <input id="inicio" type="date" value="${s.inicio?.split("T")[0] || ""}"
              style="width:100%;background:#fff;color:#222;border:1px solid #ccc;border-radius:6px;
                     padding:0.7rem 1rem;font-size:1rem;box-sizing:border-box;">
          </div>

          <div style="margin-bottom:0.8rem;">
            <label for="fin" style="display:block;font-weight:600;color:#222;margin-bottom:0.3rem;">Fecha de fin</label>
            <input id="fin" type="date" value="${s.fin?.split("T")[0] || ""}"
              style="width:100%;background:#fff;color:#222;border:1px solid #ccc;border-radius:6px;
                     padding:0.7rem 1rem;font-size:1rem;box-sizing:border-box;">
          </div>

          <div style="display:flex;align-items:center;gap:0.6rem;margin-top:0.8rem;width:fit-content;">
            <input type="checkbox" id="estado" ${s.estado ? "checked" : ""}
              style="transform:scale(1.3);accent-color:#ff6600;cursor:pointer;margin:0;">
            <label for="estado" style="font-weight:600;color:#222;margin:0;line-height:1;">Activa</label>
          </div>
        </form>
      `,
      showCancelButton: true,
      confirmButtonText: "💾 Guardar cambios",
      cancelButtonText: "Cancelar",
      focusConfirm: false,
      customClass: {
        popup: "swal2-card-suscripcion",
        confirmButton: "btn btn-orange",
        cancelButton: "btn btn-secondary",
      },
      buttonsStyling: false,

      preConfirm: () => {
        const inicio = (document.getElementById("inicio") as HTMLInputElement)?.value;
        const fin = (document.getElementById("fin") as HTMLInputElement)?.value;
        const estado = (document.getElementById("estado") as HTMLInputElement)?.checked ?? false;

        if (!inicio || !fin) {
          Swal.showValidationMessage("Las fechas son obligatorias");
          return;
        }
        if (new Date(fin) < new Date(inicio)) {
          Swal.showValidationMessage("La fecha de fin no puede ser anterior al inicio");
          return;
        }

        return { inicio, fin, estado };
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
