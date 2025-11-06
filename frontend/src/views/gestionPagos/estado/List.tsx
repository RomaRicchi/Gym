
import { useEffect, useState } from "react";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/orden.css"; 

interface Estado {
  id: number;
  nombre: string;
  descripcion: string;
}

export default function EstadosList() {
  const [estados, setEstados] = useState<Estado[]>([]);
  const [loading, setLoading] = useState(true);

  //  Cargar estados desde el backend
  const fetchEstados = async () => {
    try {
      const res = await gymApi.get("/estadoOrdenPago");
      setEstados(res.data.items || res.data);
    } catch {
      Swal.fire("Error", "No se pudieron cargar los estados", "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchEstados();
  }, []);

  // Crear nuevo estado
  const crearEstado = async () => {
    const { value: formValues } = await Swal.fire({
      title: "➕ Nuevo Estado de Pago",
      html: `
        <div class="text-start" style="font-size: 0.95rem;">
          <label class="fw-bold">Nombre</label>
          <input id="nombreInput" type="text" class="form-control mb-3" placeholder="Ej: Pendiente, Pagado..." />

          <label class="fw-bold">Descripción</label>
          <textarea id="descripcionInput" class="form-control" rows="3" placeholder="Descripción opcional..."></textarea>
        </div>
      `,
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

    try {
      const res = await gymApi.post("/estadoOrdenPago", formValues);
      setEstados((prev) => [...prev, res.data]);
      Swal.fire("✅ Guardado", "Estado creado correctamente", "success");
    } catch (err) {
      console.error(err);
      Swal.fire("❌ Error", "No se pudo crear el estado", "error");
    }
  };

  // 🔹 Editar estado existente
  const editarEstado = async (estado: Estado) => {
    const { value: formValues } = await Swal.fire({
      title: "✏️ Editar Estado de Pago",
      html: `
        <div class="text-start" style="font-size: 0.95rem;">
          <label class="fw-bold">Nombre</label>
          <input id="nombreInput" type="text" class="form-control mb-3" value="${estado.nombre}" />

          <label class="fw-bold">Descripción</label>
          <textarea id="descripcionInput" class="form-control" rows="3">${estado.descripcion || ""}</textarea>
        </div>
      `,
      showCancelButton: true,
      confirmButtonText: "💾 Guardar cambios",
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

    try {
      await gymApi.put(`/estadoOrdenPago/${estado.id}`, formValues);
      setEstados((prev) =>
        prev.map((e) => (e.id === estado.id ? { ...e, ...formValues } : e))
      );
      Swal.fire("✅ Actualizado", "Estado modificado correctamente", "success");
    } catch (err) {
      console.error(err);
      Swal.fire("❌ Error", "No se pudo actualizar el estado", "error");
    }
  };

  // Eliminar estado
  const handleDelete = async (id: number) => {
    const confirm = await Swal.fire({
      title: "¿Eliminar estado?",
      text: "Esto puede afectar órdenes existentes.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, eliminar",
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#d33",
    });

    if (confirm.isConfirmed) {
      try {
        await gymApi.delete(`/estadoOrdenPago/${id}`);
        setEstados((prev) => prev.filter((e) => e.id !== id));
        Swal.fire("✅ Eliminado", "Estado eliminado correctamente", "success");
      } catch {
        Swal.fire("❌ Error", "No se pudo eliminar el estado", "error");
      }
    }
  };

  if (loading)
    return (
      <div className="text-center mt-5">
        <div className="spinner-border text-warning" role="status"></div>
        <p className="mt-3 text-muted">Cargando estados...</p>
      </div>
    );

  return (
    <div className="mt-4 container">
      <h1
        className="text-center fw-bold mb-4"
        style={{ color: "#ff6600", fontSize: "2.5rem", letterSpacing: "2px" }}
      >
        ESTADOS DEL PAGO
      </h1>

      {/* Botón de crear */}
      <div className="d-flex justify-content-between align-items-center mb-3">
        <button
          className="btn btn-success fw-bold"
          onClick={crearEstado}
        >
          ➕ Nuevo Estado
        </button>
      </div>

      {/*Tabla */}
      <table className="table table-striped table-hover align-middle text-center">
        <thead className="table-dark">
          <tr>
            <th>Nombre</th>
            <th>Descripción</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          {estados.length === 0 ? (
            <tr>
              <td colSpan={3} className="text-muted">
                No hay estados registrados
              </td>
            </tr>
          ) : (
            estados.map((e) => (
              <tr key={e.id}>
                <td>{e.nombre}</td>
                <td>{e.descripcion || "—"}</td>
                <td>
                  <div className="d-flex justify-content-center gap-2">
                    <button
                      className="btn btn-warning btn-sm fw-bold"
                      onClick={() => editarEstado(e)}
                    >
                      ✏️
                    </button>
                    <button
                      className="btn btn-danger btn-sm fw-bold"
                      onClick={() => handleDelete(e.id)}
                    >
                      🗑️
                    </button>
                  </div>
                </td>
              </tr>
            ))
          )}
        </tbody>
      </table>
    </div>
  );
}
