import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export async function cancelarTurnoModal(turnoId: number, onSuccess: () => void) {
  const result = await Swal.fire({
    title: "¿Cancelar turno?",
    text: "Podés cancelar con al menos 1 hora de anticipación.",
    icon: "warning",
    showCancelButton: true,
    confirmButtonText: "Sí, cancelar",
    cancelButtonText: "No",
    confirmButtonColor: "#d33",
  });

  if (!result.isConfirmed) return;

  try {
    const res = await gymApi.post(`/suscripcionturno/${turnoId}/cancelar`);
    Swal.fire("✅ Turno cancelado", res.data.message, "success");
    onSuccess();
  } catch (error: any) {
    const msg = error.response?.data?.message || "No se pudo cancelar el turno.";
    Swal.fire("⚠️ Error", msg, "warning");
  }
}
