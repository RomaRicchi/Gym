import { useEffect, useState } from "react";
import {
  LineChart,
  Line,
  BarChart,
  Bar,
  XAxis,
  YAxis,
  Tooltip,
  CartesianGrid,
  ResponsiveContainer,
  Legend,
} from "recharts";
import gymApi from "@/api/gymApi";

export default function Dashboard() {
  const [suscripciones, setSuscripciones] = useState([]);
  const [salas, setSalas] = useState([]);

  useEffect(() => {
    const fetchData = async () => {
      const [suscRes, salasRes] = await Promise.all([
        gymApi.get("/dashboard/suscripciones-por-mes"),
        gymApi.get("/dashboard/salas-mas-reservadas"),
      ]);
      setSuscripciones(suscRes.data);
      setSalas(salasRes.data);
    };
    fetchData();
  }, []);

  const formatMonth = (dateString: string) =>
    new Date(dateString).toLocaleString("es-AR", { month: "short", year: "2-digit" });

  return (
    <div className="container mt-5">
      <h1 className="text-center fw-bold mb-4" style={{ color: "#ff6600" }}>
        Bienvenido a FitGym
      </h1>

      {/* 📅 Suscripciones */}
      <h4 className="mb-3" style={{ color: "#ff6600" }}>
        Suscripciones activas por mes
      </h4>
      <div
        style={{
          backgroundColor: "rgba(0, 0, 0, 0.6)",
          borderRadius: "12px",
          boxShadow: "0 4px 12px rgba(0, 0, 0, 0.3)",
          padding: "1rem",
          marginBottom: "2rem",
        }}
      >
        <ResponsiveContainer width="100%" height={300}>
          <LineChart data={suscripciones}>
            <CartesianGrid strokeDasharray="3 3" stroke="#444" />
            <XAxis dataKey="nombreMes" stroke="#fff" />
            <YAxis stroke="#fff" />
            <Tooltip
              contentStyle={{
                backgroundColor: "#000",
                borderRadius: "8px",
                color: "#fff",
              }}
            />
            <Legend wrapperStyle={{ color: "#fff" }} />
            <Line
              type="monotone"
              dataKey="cantidad"
              stroke="#ff6600"
              strokeWidth={3}
              name="Suscripciones"
            />
          </LineChart>
        </ResponsiveContainer>
      </div>

      {/* 🏋️‍♀️ Salas */}
      <h4 className="mb-3" style={{ color: "#ff6600" }}>
        Salas con más reservas
      </h4>
      <div
        style={{
          backgroundColor: "rgba(0, 0, 0, 0.6)",
          borderRadius: "12px",
          boxShadow: "0 4px 12px rgba(0, 0, 0, 0.3)",
          padding: "1rem",
        }}
      >
        <ResponsiveContainer width="100%" height={300}>
          <BarChart data={salas}>
            <CartesianGrid strokeDasharray="3 3" stroke="#444" />
            <XAxis dataKey="sala" stroke="#fff" />
            <YAxis stroke="#fff" />
            <Tooltip
              contentStyle={{
                backgroundColor: "#000",
                borderRadius: "8px",
                color: "#fff",
              }}
            />
            <Legend wrapperStyle={{ color: "#fff" }} />
            <Bar dataKey="reservas" fill="#ff6600" name="Reservas" />
          </BarChart>
        </ResponsiveContainer>
      </div>
    </div>
  );
}
