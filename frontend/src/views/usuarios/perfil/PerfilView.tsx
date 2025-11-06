import { useEffect, useState } from "react";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import { PerfilEditSwal } from "@/views/usuarios/perfil/PerfilEditSwal";
import { PasswordEditSwal } from "@/views/usuarios/perfil/CambiarContraseña";

interface Avatar {
  id: number;
  url: string;
  nombre: string;
}

interface Personal {
  nombre?: string;
  telefono?: string;
  direccion?: string;
  especialidad?: string;
  
}

interface Perfil {
  id: number;
  alias: string;
  email: string;
  rol?: string;
  avatar?: Avatar | null;
  personal?: Personal | null;
}

export default function PerfilView() {
  const [perfil, setPerfil] = useState<Perfil | null>(null);
  const [loading, setLoading] = useState(true);
  const BASE_URL = import.meta.env.VITE_API_URL || "http://localhost:5144";

  const storedUser = localStorage.getItem("usuario");
  const userId = storedUser ? JSON.parse(storedUser).id : null;

  // 🔹 Cargar perfil
  const fetchPerfil = async () => {
    if (!userId) return;
    try {
      const res = await gymApi.get(`/perfil/${userId}`);
      setPerfil(res.data);
    } catch {
      Swal.fire("Error", "No se pudo cargar el perfil del usuario", "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPerfil();
  }, []);

  // 📸 Subir avatar
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
      const res = await gymApi.post(`/perfil/${userId}/avatar`, formData, {
        headers: { "Content-Type": "multipart/form-data" },
      });

      await new Promise((r) => setTimeout(r, 500));

      Swal.fire("✅ Avatar actualizado", "Tu nuevo avatar se aplicará al instante", "success");

      await fetchPerfil();

      const stored = localStorage.getItem("usuario");
      if (stored) {
        const user = JSON.parse(stored);
        user.avatar = { url: res.data.url || res.data.Url || "" };
        localStorage.setItem("usuario", JSON.stringify(user));
      }

      setTimeout(() => {
        window.dispatchEvent(new Event("authChange"));
      }, 200);
    } catch (err) {
      Swal.fire("Error", "No se pudo subir el avatar", "error");
      console.error(err);
    }
  };

  if (loading) return <p className="text-center mt-5">Cargando perfil...</p>;
  if (!perfil) return <p className="text-center mt-5">No se encontró el perfil.</p>;

  const personal = perfil.personal || {};

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
          src={
            perfil.avatar?.url
              ? `${BASE_URL}${perfil.avatar.url}`
              : `${BASE_URL}/images/user.png`
          }
          alt="Avatar"
          className="rounded-circle mx-auto mb-3 border border-white"
          style={{ width: 120, height: 120, objectFit: "cover" }}
        />

        <h4 className="fw-bold">{perfil.alias}</h4>
        <p className="text-light">{perfil.email}</p>
        <p>
          <strong>Rol:</strong> {perfil.rol}
        </p>

        {/* Datos personales */}
        <div className="text-start text-dark rounded p-3 mt-3 position-relative bg-light bg-opacity-25">
          <button
            onClick={() => PerfilEditSwal(perfil.id, fetchPerfil)}
            className="btn btn-sm btn-outline-dark position-absolute top-0 end-0 m-2"
          >
            ✏️
          </button>

          <p>
            <strong>Nombre:</strong> {personal.nombre || "—"}
          </p>
          <p>
            <strong>Teléfono:</strong> {personal.telefono || "—"}
          </p>
          <p>
            <strong>Dirección:</strong> {personal.direccion || "—"}
          </p>
          <p>
            <strong>Especialidad:</strong> {personal.especialidad || "—"}
          </p>
         
        </div>

        {/* Botones */}
        <div className="d-grid gap-2 mt-3">
          <button
            onClick={handleAvatarUpload}
            className="btn btn-warning text-black fw-semibold"
          >
            📸 Cambiar Avatar
          </button>
          <button
            onClick={() => PasswordEditSwal(userId)}
            className="btn btn-warning fw-semibold"
          >
            🔒 Cambiar Contraseña
          </button>
        </div>
      </div>
    </div>
  );
}
