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
      const { token, usuario } = res.data;

      // 🔹 Guardar sesión en localStorage
      localStorage.setItem("token", token);
      localStorage.setItem("usuario", JSON.stringify(usuario));

      await Swal.fire({
        icon: "success",
        title: "Bienvenido",
        text: `Hola ${usuario?.Alias || usuario?.Email}!`,
        timer: 1500,
        showConfirmButton: false,
      });

      // 🔹 Redirigir al inicio o dashboard
      navigate("/");
      window.location.reload(); // para actualizar el navbar con el nombre
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

  return (
    <div className="d-flex align-items-center justify-content-center vh-100 bg-light">
      <div
        className="card shadow-lg p-4"
        style={{
          width: "100%",
          maxWidth: 420,
          borderRadius: "1rem",
          background: "white",
        }}
      >
        <h3 className="text-center text-primary mb-4 fw-bold">
          🏋️‍♂️ GYM SYSTEM LOGIN
        </h3>

        <form onSubmit={handleSubmit}>
          <div className="mb-3 text-start">
            <label className="form-label fw-semibold">Email</label>
            <input
              type="email"
              className="form-control"
              placeholder="correo@ejemplo.com"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              disabled={loading}
            />
          </div>

          <div className="mb-3 text-start">
            <label className="form-label fw-semibold">Contraseña</label>
            <input
              type="password"
              className="form-control"
              placeholder="••••••••"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              disabled={loading}
            />
          </div>

          <button
            type="submit"
            className="btn btn-primary w-100 mt-3 fw-bold"
            disabled={loading}
          >
            {loading ? "Iniciando..." : "Ingresar"}
          </button>

          <div className="text-center mt-3">
            <a
              href="#"
              onClick={() => navigate("/recuperar")}
              className="text-decoration-none small text-muted"
            >
              ¿Olvidaste tu contraseña?
            </a>
          </div>
        </form>
      </div>
    </div>
  );
}
