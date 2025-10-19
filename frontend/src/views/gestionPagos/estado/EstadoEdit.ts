// @ts-nocheck
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export async function editarEstado(id: number) {
  try {
    // 🔹 Obtener datos actuales del estado
    const { data: estado } = await gymApi.get(`/estadoOrdenPago/${id}`);

    const { value: formValues } = await Swal.fire({
      title: "✏️ Editar Estado de Pago",
      html: `
        <div class="text-start" style="font-size: 0.95rem;">
          <label class="fw-bold">Nombre</label>
          <input id="nombreInput" type="text" class="form-control mb-3" value="${
            estado.nombre || ""
          }" placeholder="Nombre del estado" />

          <label class="fw-bold">Descripción</label>
          <textarea id="descInput" class="form-control" rows="3" placeholder="Descripción...">${
            estado.descripcion || ""
          }</textarea>
        </div>
      `,
      focusConfirm: false,
      showCancelButton: true,
      confirmButtonText: "💾 Guardar cambios",
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#ff6b00",
      width: "500px",
      preConfirm: () => {
        const nombre = (document.getElementById("nombreInput") as HTMLInputElement).value.trim();
        const descripcion = (document.getElementById("descInput") as HTMLTextAreaElement).value.trim();

        if (!nombre) {
          Swal.showValidationMessage("⚠️ El nombre es obligatorio");
          return false;
        }

        return { nombre, descripcion };
      },
    });

    if (!formValues) return;

    // 🔹 Enviar cambios al backend
    await gymApi.put(`/estadoOrdenPago/${id}`, formValues);

    await Swal.fire({
      icon: "success",
      title: "✅ Actualizado",
      text: "Estado modificado correctamente.",
      confirmButtonColor: "#ff6b00",
    });

    // Recargar lista de estados
    window.location.reload();
  } catch (err) {
    console.error(err);
    Swal.fire("❌ Error", "No se pudo actualizar el estado.", "error");
  }
}
