// @ts-nocheck
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-card.css";
import { RutinaPlantillaEjercicioCreateSwal } from "@/views/rutinas/rutina-ejercicios/RutinaPlantillaEjercicioCreateSwal";
import { RutinaPlantillaEjercicioEditSwal } from "@/views/rutinas/rutina-ejercicios/RutinaPlantillaEjercicioEditSwal";

export async function RutinaMostrarEjerciciosSwal(rutinaId: number, rutinaNombre: string) {
  try {
    const { data: ejercicios } = await gymApi.get(
      `/rutinasplantillaejercicios?page=1&pageSize=100&q=${encodeURIComponent(rutinaNombre)}`
    );

    const lista = (ejercicios.items || ejercicios)
      .map(
        (e: any) => `
        <div class="ej-card" style="display:flex;align-items:center;gap:10px;margin-bottom:8px;">
          <img 
            src="${
              e.imagenUrl
                ? `${import.meta.env.VITE_API_BASE_URL.replace(/\/api$/, "")}/${e.imagenUrl}`
                : "/placeholder.png"
            }"
            data-fullimg="${
              e.imagenUrl
                ? `${import.meta.env.VITE_API_BASE_URL.replace(/\/api$/, "")}/${e.imagenUrl}`
                : "/placeholder.png"
            }"
            class="img-miniatura"
            style="width:70px;height:70px;object-fit:cover;border-radius:8px;border:2px solid #ff6600;cursor:pointer;"
          />
          <div style="flex:1;">
            <div class="ej-nombre">${e.ejercicio}</div>
            <div class="ej-detalle">
              Series: ${e.series} | Reps: ${e.repeticiones} | Descanso: ${e.descansoSeg}s
            </div>
          </div>
          <div class="ej-acciones" style="display:flex;gap:8px;">
            <button class="btn-editar" data-id="${e.id}" 
              style="background:#ffcc00;border:none;border-radius:6px;padding:4px 8px;cursor:pointer;">✏️</button>
            <button class="btn-eliminar" data-id="${e.id}" 
              style="background:#ff4d4d;border:none;border-radius:6px;padding:4px 8px;cursor:pointer;">🗑️</button>
          </div>
        </div>`
      )
      .join("");

    const contenido = `
      <div style="display:flex;flex-direction:column;align-items:center;gap:12px;">
        <button id="btn-nuevo-ej" 
          style="background:#ff6600;color:white;border:none;border-radius:8px;
                 padding:8px 18px;font-weight:600;cursor:pointer;align-self:center;">
          ➕ Nuevo ejercicio
        </button>
        <div id="lista-ejercicios" style="width:100%;margin-top:10px;">
          ${
            lista ||
            "<p class='text-muted text-center mt-3'>Esta rutina no tiene ejercicios asignados.</p>"
          }
        </div>
      </div>
    `;

    await Swal.fire({
      title: `<strong>${rutinaNombre}</strong>`,
      html: contenido,
      width: 700,
      customClass: { popup: "swal2-card-style" },
      showCancelButton: true,
      confirmButtonText: "Cerrar",
      cancelButtonText: "Cancelar",
      buttonsStyling: false,
      scrollbarPadding: false,
      didOpen: () => {
        // ➕ Nuevo ejercicio
        document.getElementById("btn-nuevo-ej")?.addEventListener("click", async () => {
          Swal.close();
          await RutinaPlantillaEjercicioCreateSwal(
            () => RutinaMostrarEjerciciosSwal(rutinaId, rutinaNombre),
            { id: rutinaId, nombre: rutinaNombre }
          );
        });

        //  Editar
        document.querySelectorAll(".btn-editar").forEach((btn) => {
          btn.addEventListener("click", async () => {
            const id = btn.getAttribute("data-id");
            Swal.close();
            await RutinaPlantillaEjercicioEditSwal(id, () =>
              RutinaMostrarEjerciciosSwal(rutinaId, rutinaNombre)
            );
          });
        });

        // Eliminar
        document.querySelectorAll(".btn-eliminar").forEach((btn) => {
          btn.addEventListener("click", async () => {
            const id = btn.getAttribute("data-id");
            const confirm = await Swal.fire({
              title: "¿Eliminar ejercicio?",
              text: "Esta acción no se puede deshacer.",
              icon: "warning",
              showCancelButton: true,
              confirmButtonText: "Sí, eliminar",
              cancelButtonText: "Cancelar",
              buttonsStyling: false,
              customClass: {
                popup: "swal2-card-style",
                confirmButton: "btn btn-orange",
                cancelButton: "btn btn-secondary",
              },
            });
            if (confirm.isConfirmed) {
              await gymApi.delete(`/rutinasplantillaejercicios/${id}`);
              await RutinaMostrarEjerciciosSwal(rutinaId, rutinaNombre);
            }
          });
        });

        //  Ampliar imagen y volver al listado
        document.querySelectorAll(".img-miniatura").forEach((img) => {
          img.addEventListener("click", async () => {
            const src = img.getAttribute("data-fullimg");
            const alt =
              img.closest(".ej-card")?.querySelector(".ej-nombre")?.textContent ||
              "Ejercicio";

            // Cierro el swal actual
            Swal.close();

            // Muestro la imagen grande
            await Swal.fire({
              title: `<strong>${alt}</strong>`,
              imageUrl: src,
              imageAlt: alt,
              width: "auto",
              background: "#1e1e1e",
              showConfirmButton: false,
              showCloseButton: true,
              customClass: { popup: "swal2-card-style" },
            });

            // Al cerrar la imagen, vuelvo a abrir el listado
            await RutinaMostrarEjerciciosSwal(rutinaId, rutinaNombre);
          });
        });
      },
    });
  } catch (error) {
    console.error("Error al mostrar ejercicios:", error);
    Swal.fire("Error", "No se pudieron cargar los ejercicios", "error");
  }
}
