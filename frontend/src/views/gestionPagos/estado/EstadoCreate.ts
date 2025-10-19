// @ts-nocheck
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export async function crearEstado() {
  try {
    const { value: formValues } = await Swal.fire({
      title: "➕ Nuevo Estado de Pago",
      html: `
        <div class="text-start" style="font-size: 0.95rem;">
          <label class="fw-bold">Nombre</label>
          <input id="nombreInput" type="text" class="form-control mb-3" placeholder="Ej: Pagado, Pendiente, Rechazado" />

          <label class="fw-bold">Descripción</label>
          <textarea id="descripcionInput" class="form-control" rows="3" placeholder="Descripción opcional..."></textarea>
        </div>
      `,
      focusConfirm: false,
      showCancelButton: true,
      confirmButtonText: "💾 Guardar",
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#ff6b00",
      width: "500px",
      preConfirm: () => {
        const nombre = (document.getElementById("nombreInput") as HTMLInputElement).value.trim();
        const descripcion = (document.getElementById("descripcionInput") as HTMLTextAreaElement).value.trim();

        if (!nombre) {
          Swal.showValidationMessage("⚠️ El nombre es obligatorio");
          return false;
        }

        return { nombre, descripcion };
      },
    });

    if (!formValues) return;

    // 🔹 Guardar en backend
    await gymApi.post("/estadoOrdenPago", formValues);

    await Swal.fire({
      icon: "success",
      title: "✅ Guardado",
      text: "Estado creado correctamente.",
      confirmButtonColor: "#ff6b00",
    });

    window.location.reload();
  } catch (error) {
    console.error(error);
    Swal.fire("❌ Error", "No se pudo crear el estado.", "error");
  }
}
