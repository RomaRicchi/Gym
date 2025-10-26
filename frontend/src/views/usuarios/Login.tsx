import { useState } from "react";
import Swal from "sweetalert2";
import { useNavigate } from "react-router-dom";
import gymApi from "@/api/gymApi";

export default function Login() {
  const navigate = useNavigate();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!email || !password) {
      Swal.fire("Campos requeridos", "Complete email y contraseña", "warning");
      return;
    }

    try {
      setLoading(true);

      // 🔹 Petición al backend
      const res = await gymApi.post("/usuarios/login", { email, password });

      // ✅ Compatibilidad con Token / token
      const token = res.data.Token || res.data.token;
      const usuario = res.data.Usuario || res.data.usuario;

      if (!token) {
        throw new Error("No se recibió el token JWT del servidor.");
      }

      // 🔹 Guardar sesión en localStorage
      localStorage.setItem("token", token);
      localStorage.setItem("usuario", JSON.stringify(usuario));

      await Swal.fire({
        icon: "success",
        title: "Bienvenido",
        text: `Hola ${usuario?.alias || usuario?.email}!`,
        timer: 1500,
        showConfirmButton: false,
      });

      // 🔹 Redirigir al dashboard
      navigate("/");
      window.location.reload(); // para refrescar navbar o contexto
    } catch (err: any) {
      console.error(err);
      Swal.fire(
        "Error",
        err.response?.data?.message || "Credenciales incorrectas",
        "error"
      );
    } finally {
      setLoading(false);
    }
  };

  // 🔹 Swal de recuperación de contraseña
  const handleForgotPassword = async () => {
    const { value: email } = await Swal.fire<string>({
      title: "Recuperar contraseña",
      input: "email",
      inputPlaceholder: "correo@ejemplo.com",
      confirmButtonText: "Enviar enlace",
      showCancelButton: true,
      cancelButtonText: "Cancelar",
      preConfirm: (value) => {
        if (!value) Swal.showValidationMessage("El email es obligatorio");
        return value;
      },
    });

    if (email) {
      try {
        await gymApi.post("/usuarios/forgot-password", { email });
        Swal.fire(
          "📧 Correo enviado",
          "Revisa tu bandeja de entrada para continuar con el restablecimiento.",
          "success"
        );
      } catch (err: any) {
        console.error(err);
        Swal.fire(
          "Error",
          err.response?.data || "No se pudo enviar el correo. Verifica el email.",
          "error"
        );
      }
    }
  };

  return (
    <div
      className="d-flex align-items-center justify-content-center vh-100"
      style={{ backgroundColor: "#f8f9fa" }}
    >
      <div
        className="card shadow-lg p-4 text-white"
        style={{
          width: "100%",
          maxWidth: 420,
          borderRadius: "1rem",
          backgroundColor: "#ff6b00",
          border: "none",
        }}
      >
        <h3 className="text-center mb-4 fw-bold">🏋️‍♂️ FITGYM SYSTEM LOGIN</h3>

        <form onSubmit={handleSubmit}>
          <div className="mb-3 text-start">
            <label className="form-label fw-semibold text-white">Email</label>
            <input
              type="email"
              className="form-control"
              placeholder="correo@ejemplo.com"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              disabled={loading}
              style={{
                backgroundColor: "#fff",
                border: "none",
                borderRadius: "0.5rem",
              }}
            />
          </div>

          <div className="mb-3 text-start">
            <label className="form-label fw-semibold text-white">Contraseña</label>
            <input
              type="password"
              className="form-control"
              placeholder="********"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              disabled={loading}
              style={{
                backgroundColor: "#fff",
                border: "none",
                borderRadius: "0.5rem",
              }}
            />
          </div>

          <button
            type="submit"
            className="btn w-100 mt-3 fw-bold"
            disabled={loading}
            style={{
              backgroundColor: "#ffffff",
              color: "#ff6b00",
              border: "none",
            }}
          >
            {loading ? "Iniciando..." : "Ingresar"}
          </button>

          <div className="text-center mt-3">
            <a
              href="#"
              onClick={handleForgotPassword}
              className="text-decoration-none small text-white-50"
            >
              ¿Olvidaste tu contraseña?
            </a>
          </div>
        </form>
      </div>
    </div>
  );
}
