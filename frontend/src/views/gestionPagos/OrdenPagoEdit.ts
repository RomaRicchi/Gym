import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

// 🔹 Mostrar Swal para cambiar el estado de la orden
export async function editarOrden(id: number) {
  try {
    const { data: estados } = await gymApi.get("/estadoOrdenPago");

    const opciones = estados.reduce((acc: any, e: any) => {
      acc[e.id] = e.nombre;
      return acc;
    }, {});

    const { value: nuevoEstadoId } = await Swal.fire({
      title: "Cambiar estado",
      input: "select",
      inputOptions: opciones,
      inputPlaceholder: "Seleccione un estado",
      showCancelButton: true,
      confirmButtonText: "Guardar",
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#3085d6",
      cancelButtonColor: "#d33",
    });

    if (!nuevoEstadoId) return;

    await gymApi.put(`/ordenes/${id}/estado/simple`, { estadoId: nuevoEstadoId });

    await Swal.fire({
      icon: "success",
      title: "Estado actualizado",
      text: "El estado de la orden fue actualizado correctamente.",
      timer: 1500,
      showConfirmButton: false,
    });

    return true; // indica éxito
  } catch (err) {
    console.error(err);
    Swal.fire("Error", "No se pudo actualizar el estado de la orden.", "error");
    return false;
  }
}
