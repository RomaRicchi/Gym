import React, { useState } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";
import gymApi from "@/api/gymApi";
import Swal from "sweetalert2";

const ResetPassword: React.FC = () => {
  const [params] = useSearchParams();
  const token = params.get("token");
  const navigate = useNavigate();

  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!newPassword || !confirmPassword) {
      Swal.fire("Campos requeridos", "Completa ambos campos.", "warning");
      return;
    }

    if (newPassword !== confirmPassword) {
      Swal.fire("Error", "Las contraseñas no coinciden.", "error");
      return;
    }

    if (!token) {
      Swal.fire("Error", "El enlace no contiene un token válido.", "error");
      return;
    }

    try {
      setLoading(true);
      await gymApi.post("/usuarios/reset-password", { token, newPassword });
      await Swal.fire("✅ Éxito", "Tu contraseña fue restablecida correctamente.", "success");
      navigate("/login");
    } catch (error) {
      console.error(error);
      Swal.fire("❌ Error", "El enlace expiró o ya fue usado.", "error");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div
      style={{
        minHeight: "100vh",
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        background: "linear-gradient(135deg, #fff5ec 0%, #ffd7b3 100%)",
        padding: "2rem",
      }}
    >
      <div
        className="shadow-lg text-white"
        style={{
          backgroundColor: "#ff6b00",
          borderRadius: "1.2rem",
          width: "100%",
          maxWidth: 420,
          padding: "2.5rem",
          boxShadow: "0 10px 30px rgba(0,0,0,0.25)",
        }}
      >
        <h3
          className="text-center mb-5 fw-bold text-2xl flex items-center justify-center gap-2"
          style={{ display: "flex", alignItems: "center", justifyContent: "center" }}
        >
          <span role="img" aria-label="lock" style={{ fontSize: "1.8rem" }}>🔒</span>
          <span style={{ fontWeight: "700" }}>Nueva contraseña</span>
        </h3>

        <form onSubmit={handleSubmit}>
          <div className="mb-4 text-start">
            <label className="fw-semibold text-white mb-2 d-block">Nueva contraseña</label>
            <input
              type="password"
              className="form-control"
              placeholder="********"
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
              disabled={loading}
              style={{
                backgroundColor: "#fff",
                border: "none",
                borderRadius: "0.6rem",
                color: "#333",
                padding: "0.9rem 1rem",
                fontSize: "1rem",
                width: "100%",
              }}
            />
          </div>

          <div className="mb-5 text-start">
            <label className="fw-semibold text-white mb-2 d-block">Confirmar contraseña</label>
            <input
              type="password"
              className="form-control"
              placeholder="********"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              disabled={loading}
              style={{
                backgroundColor: "#fff",
                border: "none",
                borderRadius: "0.6rem",
                color: "#333",
                padding: "0.9rem 1rem",
                fontSize: "1rem",
                width: "100%",
              }}
            />
          </div>

          <button
            type="submit"
            className="btn w-100 fw-bold"
            disabled={loading}
            style={{
              backgroundColor: "#fff",
              color: "#ff6b00",
              border: "none",
              borderRadius: "0.6rem",
              padding: "0.9rem",
              fontSize: "1.05rem",
              fontWeight: "600",
              transition: "all 0.3s ease",
            }}
            onMouseEnter={(e) =>
              (e.currentTarget.style.backgroundColor = "#ffe3d1")
            }
            onMouseLeave={(e) =>
              (e.currentTarget.style.backgroundColor = "#fff")
            }
          >
            {loading ? "Restableciendo..." : "Restablecer contraseña"}
          </button>
        </form>
      </div>
    </div>
  );
};

export default ResetPassword;
