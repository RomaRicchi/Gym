import { useEffect, useState } from "react";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import { PasswordEditSwal } from "@/views/usuarios/perfil/CambiarContraseña";

interface Usuario {
  id: number;
  alias: string;
  email: string;
  avatarUrl?: string;
}

interface Socio {
  id: number;
  nombre: string;
  dni: string;
  telefono?: string;
  fechaNacimiento?: string;
  activo: boolean;
  planActual?: string;
  usuario?: Usuario;
}

export default function PerfilSocio() {
  const [perfil, setPerfil] = useState<Socio | null>(null);
  const [loading, setLoading] = useState(true);
  const BASE_URL = import.meta.env.VITE_API_URL || "http://localhost:5144";

  const storedUser = localStorage.getItem("usuario");
  const userId = storedUser ? JSON.parse(storedUser).id : null;

  // 🔹 Obtener perfil del socio
  const fetchPerfil = async () => {
    try {
      const res = await gymApi.get("/perfil/socio");
      setPerfil(res.data);
    } catch (err) {
      console.error(err);
      Swal.fire("Error", "No se pudo cargar el perfil del socio", "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPerfil();
  }, []);

  // 📸 Cambiar avatar
  const handleAvatarUpload = async () => {
    const { value: file } = await Swal.fire({
      title: "📸 Cambiar Avatar",
      input: "file",
      inputAttributes: { accept: "image/*" },
      showCancelButton: true,
      confirmButtonText: "Subir",
      cancelButtonText: "Cancelar",
      preConfirm: (value) => {
        if (!value) Swal.showValidationMessage("Debe seleccionar una imagen");
        return value;
      },
    });

    if (!file) return;

    const formData = new FormData();
    formData.append("archivo", file);

    try {
      const uploadRes = await gymApi.post(`/perfil/${userId}/avatar`, formData, {
        headers: { "Content-Type": "multipart/form-data" },
      });

      Swal.fire("✅ Avatar actualizado", "Tu nuevo avatar se aplicará al instante", "success");

      // Actualizar localStorage
      const stored = localStorage.getItem("usuario");
      if (stored) {
        const user = JSON.parse(stored);
        user.avatar = uploadRes.data.url;
        localStorage.setItem("usuario", JSON.stringify(user));
      }

      await fetchPerfil();
      setTimeout(() => window.dispatchEvent(new Event("authChange")), 300);
    } catch (err) {
      console.error(err);
      Swal.fire("Error", "No se pudo subir el avatar", "error");
    }
  };

  // ✏️ Editar datos del socio + usuario
  const handleEditDatos = async () => {
    if (!perfil || !perfil.usuario) return;

    const socio = perfil;
    const usuario = perfil.usuario;

    const { value: formValues } = await Swal.fire({
      title: "✏️ Editar datos del socio",
      html: `
        <input id="swal-nombre" class="swal2-input" placeholder="Nombre" value="${socio.nombre || ""}">
        <input id="swal-dni" class="swal2-input" placeholder="DNI" value="${socio.dni || ""}">
        <input id="swal-telefono" class="swal2-input" placeholder="Teléfono" value="${socio.telefono || ""}">
        <input id="swal-fecha" type="date" class="swal2-input" value="${socio.fechaNacimiento ? socio.fechaNacimiento.split("T")[0] : ""}">
        <hr class="mt-3 mb-2" />
        <input id="swal-alias" class="swal2-input" placeholder="Alias" value="${usuario.alias || ""}">
        <input id="swal-email" type="email" class="swal2-input" placeholder="Correo electrónico" value="${usuario.email || ""}">
      `,
      focusConfirm: false,
      showCancelButton: true,
      confirmButtonText: "Guardar",
      cancelButtonText: "Cancelar",
      customClass: { popup: "swal2-card-style" },
      preConfirm: () => {
        const nombre = (document.getElementById("swal-nombre") as HTMLInputElement)?.value;
        const dni = (document.getElementById("swal-dni") as HTMLInputElement)?.value;
        const telefono = (document.getElementById("swal-telefono") as HTMLInputElement)?.value;
        const fechaNacimiento = (document.getElementById("swal-fecha") as HTMLInputElement)?.value;
        const alias = (document.getElementById("swal-alias") as HTMLInputElement)?.value;
        const email = (document.getElementById("swal-email") as HTMLInputElement)?.value;

        if (!nombre || !dni || !alias || !email) {
          Swal.showValidationMessage("Nombre, DNI, alias y email son obligatorios");
          return false;
        }

        return { nombre, dni, telefono, fechaNacimiento, alias, email };
      },
    });

    if (!formValues) return;

    try {
      // Actualizar socio
      await gymApi.put(`/socios/${socio.id}`, {
        nombre: formValues.nombre,
        dni: formValues.dni,
        telefono: formValues.telefono,
        fechaNacimiento: formValues.fechaNacimiento,
      });

      // Actualizar usuario (perfil)
      await gymApi.patch(`/perfil/${usuario.id}`, {
        alias: formValues.alias,
        email: formValues.email,
      });

      Swal.fire("✅ Datos actualizados", "Los cambios se guardaron correctamente.", "success");
      await fetchPerfil();
    } catch (err) {
      console.error(err);
      Swal.fire("Error", "No se pudieron actualizar los datos", "error");
    }
  };

  if (loading) return <p className="text-center mt-5">Cargando perfil...</p>;
  if (!perfil) return <p className="text-center mt-5">No se encontró el perfil.</p>;

  const avatarUrl =
    perfil.usuario?.avatarUrl && perfil.usuario.avatarUrl.trim() !== ""
      ? `${BASE_URL}${perfil.usuario.avatarUrl}?v=${Date.now()}`
      : `${BASE_URL}/images/user.png`;

  return (
    <div className="container mt-4 text-center">
      <div
        className="card mx-auto shadow p-4 text-white position-relative"
        style={{
          maxWidth: 420,
          backgroundColor: "#ff6b00",
          border: "none",
          borderRadius: "1rem",
        }}
      >
        {/* Avatar */}
        <img
          src={avatarUrl}
          alt="Avatar"
          className="rounded-circle mx-auto mb-3 border border-white"
          style={{ width: 120, height: 120, objectFit: "cover" }}
        />

        <h4 className="fw-bold text-capitalize">{perfil.nombre}</h4>
        <p className="text-light">{perfil.usuario?.email}</p>
        <p><strong>DNI:</strong> {perfil.dni}</p>

        {/* Datos personales */}
        <div className="text-start text-dark rounded p-3 mt-3 position-relative bg-light bg-opacity-25">
          <button
            onClick={handleEditDatos}
            className="btn btn-sm btn-outline-dark position-absolute top-0 end-0 m-2"
          >
            ✏️
          </button>
          <p><strong>Alias:</strong> {perfil.usuario?.alias || "—"}</p>
          <p><strong>Teléfono:</strong> {perfil.telefono || "—"}</p>
          <p><strong>Fecha de nacimiento:</strong> {perfil.fechaNacimiento ? new Date(perfil.fechaNacimiento).toLocaleDateString() : "—"}</p>
          <p><strong>Plan actual:</strong> {perfil.planActual || "Sin plan activo"}</p>
          <p><strong>Estado:</strong> {perfil.activo ? "Activo" : "Inactivo"}</p>
        </div>

        {/* Botones */}
        <div className="d-grid gap-2 mt-3">
          <button onClick={handleAvatarUpload} className="btn btn-warning text-black fw-semibold">
            📸 Cambiar Avatar
          </button>
          <button onClick={() => PasswordEditSwal(userId)} className="btn btn-warning fw-semibold">
            🔒 Cambiar Contraseña
          </button>
        </div>
      </div>
    </div>
  );
}
