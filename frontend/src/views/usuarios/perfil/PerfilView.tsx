import { useEffect, useState } from "react";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface Avatar {
  id: number;
  url: string;
  nombre: string;
}

interface Perfil {
  id: number;
  alias: string;
  email: string;
  nombre?: string;
  telefono?: string;
  rol?: string;
  avatar?: Avatar | null;
}

export default function PerfilView() {
  const [perfil, setPerfil] = useState<Perfil | null>(null);
  const [loading, setLoading] = useState(true);

  const storedUser = localStorage.getItem("usuario");
  const userId = storedUser ? JSON.parse(storedUser).id : null;

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

      Swal.fire("✅ Avatar actualizado", res.data.message, "success");
      fetchPerfil();
    } catch (err) {
      console.error(err);
      Swal.fire("Error", "No se pudo subir el avatar", "error");
    }
  };

  // 🔁 Restablecer avatar
  const handleAvatarDefault = async () => {
    const confirm = await Swal.fire({
      title: "¿Quitar avatar actual?",
      text: "Se restablecerá el avatar por defecto.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, restablecer",
      cancelButtonText: "Cancelar",
    });

    if (!confirm.isConfirmed) return;

    try {
      await gymApi.post(`/perfil/${userId}/avatar/default`);
      Swal.fire("Restablecido", "Se asignó el avatar por defecto", "success");
      fetchPerfil();
    } catch (err) {
      console.error(err);
      Swal.fire("Error", "No se pudo restablecer el avatar", "error");
    }
  };

  // 🔐 Cambiar contraseña
  const handleCambiarPassword = async () => {
    const { value: formValues } = await Swal.fire({
      title: "🔒 Cambiar Contraseña",
      html: `
        <div class="text-start">
          <label class="form-label">Contraseña actual</label>
          <input id="actual" type="password" class="form-control" />
          <label class="form-label mt-2">Nueva contraseña</label>
          <input id="nueva" type="password" class="form-control" />
        </div>
      `,
      showCancelButton: true,
      confirmButtonText: "Actualizar",
      cancelButtonText: "Cancelar",
      preConfirm: () => {
        const actual = (document.getElementById("actual") as HTMLInputElement).value;
        const nueva = (document.getElementById("nueva") as HTMLInputElement).value;
        if (!actual || !nueva) Swal.showValidationMessage("Complete ambos campos");
        return { actual, nueva };
      },
    });

    if (formValues) {
      try {
        await gymApi.patch(`/perfil/${userId}/password`, formValues);
        Swal.fire("✅", "Contraseña actualizada correctamente", "success");
      } catch {
        Swal.fire("Error", "No se pudo cambiar la contraseña", "error");
      }
    }
  };

  if (loading) return <p>Cargando perfil...</p>;
  if (!perfil) return <p>No se encontró el perfil.</p>;

  return (
    <div className="container mt-4 text-center">
      <h2 className="fw-bold text-primary mb-3">
        👤 <span className="text-primary">Mi Perfil</span>
      </h2>

      <div
        className="card mx-auto shadow p-4 text-white"
        style={{
          maxWidth: 420,
          backgroundColor: "#ff6b00",
          border: "none",
          borderRadius: "1rem",
        }}
      >
        <h5 className="text-center mb-3">Avatar</h5>
        <img
          src={
            perfil.avatar?.url
                ? `http://localhost:5144${perfil.avatar.url}`
                : "http://localhost:5144/images/user.png"
          }
          alt="Avatar"
          className="rounded-circle mx-auto mb-3 border border-white"
          style={{ width: 120, height: 120, objectFit: "cover" }}
        />
        <h4 className="fw-bold">{perfil.nombre || perfil.alias}</h4>
        <p className="text-light">{perfil.email}</p>
        <p>
          <strong>Rol:</strong> {perfil.rol}
        </p>

        <div className="d-grid gap-2 mt-3">
          <button onClick={handleAvatarUpload} className="btn btn-light text-primary fw-semibold">
            📸 Cambiar Avatar
          </button>
          <button onClick={handleAvatarDefault} className="btn btn-outline-light fw-semibold">
            🔁 Restablecer Avatar
          </button>
          <button onClick={handleCambiarPassword} className="btn btn-dark fw-semibold">
            🔒 Cambiar Contraseña
          </button>
        </div>
      </div>
    </div>
  );
}
