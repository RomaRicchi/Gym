import React, { useEffect, useMemo, useState } from "react";
import { Plus, Search, Loader2, ChevronLeft, ChevronRight, X } from "lucide-react";

// API base (configurar en .env.local): VITE_API_BASE_URL=http://localhost:5144
const API_BASE: string = (import.meta as any).env?.VITE_API_BASE_URL || "http://localhost:5144";
console.log("API_BASE =", API_BASE);

// Tipos
interface Socio {
  id: number;
  dni: string;
  nombre: string;
  email: string;
  telefono?: string | null;
  activo?: boolean | null;
  creado_en?: string | null;
}

interface Paged<T> {
  total: number;
  page: number;
  pageSize: number;
  items: T[];
}

// Helpers
function cls(...xs: Array<string | false | null | undefined>) {
  return xs.filter(Boolean).join(" ");
}

function useDebounced<T>(value: T, delay = 400) {
  const [v, setV] = useState(value);
  useEffect(() => {
    const id = setTimeout(() => setV(value), delay);
    return () => clearTimeout(id);
  }, [value, delay]);
  return v;
}

// Form state para crear socio
type NewSocio = { dni: string; nombre: string; email: string; telefono?: string };
const emptySocio: NewSocio = { dni: "", nombre: "", email: "", telefono: "" };

export default function SociosView() {
  // filtros & paginación
  const [q, setQ] = useState("");
  const [onlyActive, setOnlyActive] = useState(true);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const dq = useDebounced(q);

  // data
  const [data, setData] = useState<Paged<Socio> | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // modal crear
  const [open, setOpen] = useState(false);
  const [saving, setSaving] = useState(false);
  const [form, setForm] = useState<NewSocio>({ ...emptySocio });
  const [formError, setFormError] = useState<string | null>(null);
  const [toast, setToast] = useState<string | null>(null);

  // fetch socios
  useEffect(() => {
    let aborted = false;
    async function run() {
      setLoading(true);
      setError(null);
      try {
        // Construcción segura de URL con base + path
        const url = new URL("/api/socios", API_BASE);
        url.searchParams.set("page", String(page));
        url.searchParams.set("pageSize", String(pageSize));
        if (dq) url.searchParams.set("q", dq);
        if (onlyActive) url.searchParams.set("activo", "true");

        const res = await fetch(url.toString(), { headers: { Accept: "application/json" } });
        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        const json = (await res.json()) as Paged<Socio> | Socio[];
        const normalized: Paged<Socio> = Array.isArray(json)
          ? { total: json.length, page: 1, pageSize: json.length, items: json }
          : json;
        if (!aborted) setData(normalized);
      } catch (e: any) {
        if (!aborted) setError(e?.message ?? "Error al cargar");
      } finally {
        if (!aborted) setLoading(false);
      }
    }
    run();
    return () => { aborted = true; };
  }, [dq, onlyActive, page, pageSize]);

  const totalPages = useMemo(() => {
    if (!data) return 1;
    return Math.max(1, Math.ceil(data.total / data.pageSize));
  }, [data]);

  // acciones
  function resetAndClose() {
    setForm({ ...emptySocio });
    setFormError(null);
    setOpen(false);
  }

  async function createSocio() {
    setFormError(null);
    if (!form.dni?.trim() || !form.nombre?.trim() || !form.email?.trim()) {
      setFormError("dni, nombre y email son obligatorios");
      return;
    }
    try {
      setSaving(true);
      const postUrl = new URL("/api/socios", API_BASE);
      const res = await fetch(postUrl.toString(), {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(form),
      });
      if (res.status === 409) {
        setFormError("Ya existe un socio con ese DNI o email");
        return;
      }
      if (!res.ok) throw new Error(`HTTP ${res.status}`);
      setToast("Socio creado con éxito");
      resetAndClose();
      // Refetch
      setPage(1);
      setQ("");
      setOnlyActive(true);
      const refUrl = new URL("/api/socios", API_BASE);
      refUrl.searchParams.set("page", "1");
      refUrl.searchParams.set("pageSize", String(pageSize));
      refUrl.searchParams.set("activo", "true");
      const ref = await fetch(refUrl.toString());
      const json = await ref.json();
      const normalized: Paged<Socio> = Array.isArray(json)
        ? { total: json.length, page: 1, pageSize: json.length, items: json }
        : json;
      setData(normalized);
    } catch (e: any) {
      setFormError(e?.message ?? "Error guardando socio");
    } finally {
      setSaving(false);
    }
  }

  return (
    <div className="min-h-screen bg-neutral-50 text-neutral-900">
      <div className="max-w-6xl mx-auto px-4 py-8">
        <header className="flex items-center justify-between gap-4 mb-6">
          <div>
            <h1 className="text-2xl font-semibold">Socios</h1>
            <p className="text-sm text-neutral-500">Listado, búsqueda y alta de socios</p>
          </div>
          <button
            onClick={() => setOpen(true)}
            className="inline-flex items-center gap-2 rounded-2xl px-4 py-2 shadow-sm border bg-white hover:bg-neutral-50 active:scale-[.99]"
          >
            <Plus className="w-4 h-4" /> Nuevo socio
          </button>
        </header>

        {/* Filtros */}
        <div className="rounded-2xl border bg-white p-4 mb-4 shadow-sm">
          <div className="flex flex-col md:flex-row gap-3 items-stretch md:items-center">
            <div className="relative flex-1">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-neutral-400" />
              <input
                value={q}
                onChange={(e) => { setPage(1); setQ(e.target.value); }}
                placeholder="Buscar por nombre, DNI o email"
                className="w-full pl-10 pr-3 py-2 rounded-xl border focus:outline-none focus:ring-2 focus:ring-neutral-300"
              />
            </div>
            <label className="inline-flex items-center gap-2 text-sm">
              <input type="checkbox" checked={onlyActive} onChange={(e) => { setPage(1); setOnlyActive(e.target.checked); }} />
              Solo activos
            </label>
            <select
              className="rounded-xl border py-2 px-3 text-sm"
              value={pageSize}
              onChange={(e) => { setPage(1); setPageSize(parseInt(e.target.value)); }}
            >
              {[10, 20, 50].map(n => <option key={n} value={n}>{n} por página</option>)}
            </select>
          </div>
        </div>

        {/* Tabla / estados */}
        <div className="rounded-2xl border bg-white shadow-sm overflow-hidden">
          {loading && (
            <div className="p-10 flex items-center justify-center gap-2 text-neutral-500">
              <Loader2 className="w-4 h-4 animate-spin" /> Cargando...
            </div>
          )}
          {error && !loading && (
            <div className="p-6 text-red-600">{error}</div>
          )}
          {!loading && !error && (
            <div className="overflow-x-auto">
              <table className="min-w-full text-sm">
                <thead className="bg-neutral-50 border-b">
                  <tr>
                    <th className="text-left py-3 px-4">ID</th>
                    <th className="text-left py-3 px-4">DNI</th>
                    <th className="text-left py-3 px-4">Nombre</th>
                    <th className="text-left py-3 px-4">Email</th>
                    <th className="text-left py-3 px-4">Teléfono</th>
                    <th className="text-left py-3 px-4">Estado</th>
                  </tr>
                </thead>
                <tbody>
                  {data?.items?.length ? (
                    data.items.map((s, idx) => (
                      <tr key={s.id} className={cls(idx % 2 === 1 && "bg-neutral-50/50")}>
                        <td className="py-3 px-4 font-mono text-xs text-neutral-600">{s.id}</td>
                        <td className="py-3 px-4">{s.dni}</td>
                        <td className="py-3 px-4 font-medium">{s.nombre}</td>
                        <td className="py-3 px-4">{s.email}</td>
                        <td className="py-3 px-4">{s.telefono ?? "—"}</td>
                        <td className="py-3 px-4">
                          <span className={cls(
                            "inline-flex items-center rounded-full px-2 py-0.5 text-xs border",
                            s.activo ? "bg-green-50 border-green-200 text-green-700" : "bg-neutral-100 border-neutral-300 text-neutral-600"
                          )}>
                            {s.activo ? "Activo" : "Inactivo"}
                          </span>
                        </td>
                      </tr>
                    ))
                  ) : (
                    <tr>
                      <td className="py-10 px-4 text-center text-neutral-500" colSpan={6}>Sin resultados</td>
                    </tr>
                  )}
                </tbody>
              </table>
            </div>
          )}

          {/* Paginación */}
          <div className="flex items-center justify-between p-3 border-t bg-neutral-50">
            <div className="text-xs text-neutral-500">
              {data ? `Total: ${data.total}` : " "}
            </div>
            <div className="flex items-center gap-2">
              <button
                onClick={() => setPage(p => Math.max(1, p - 1))}
                disabled={page <= 1}
                className="inline-flex items-center gap-1 px-3 py-1.5 rounded-xl border bg-white disabled:opacity-50"
              >
                <ChevronLeft className="w-4 h-4" /> Anterior
              </button>
              <span className="text-sm text-neutral-600">{page} / {totalPages}</span>
              <button
                onClick={() => setPage(p => Math.min(totalPages, p + 1))}
                disabled={page >= totalPages}
                className="inline-flex items-center gap-1 px-3 py-1.5 rounded-xl border bg-white disabled:opacity-50"
              >
                Siguiente <ChevronRight className="w-4 h-4" />
              </button>
            </div>
          </div>
        </div>

        {/* Modal crear socio */}
        {open && (
          <div className="fixed inset-0 z-50 grid place-items-center bg-black/30 p-4">
            <div className="w-full max-w-lg rounded-2xl bg-white shadow-xl border">
              <div className="flex items-center justify-between p-4 border-b">
                <h2 className="text-lg font-semibold">Nuevo socio</h2>
                <button onClick={resetAndClose} className="p-1 rounded-lg hover:bg-neutral-100">
                  <X className="w-5 h-5" />
                </button>
              </div>

              <div className="p-4 flex flex-col gap-3">
                {formError && (
                  <div className="rounded-xl border border-red-200 bg-red-50 text-red-700 text-sm px-3 py-2">
                    {formError}
                  </div>
                )}
                <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                  <div>
                    <label className="text-sm text-neutral-600">DNI</label>
                    <input
                      value={form.dni}
                      onChange={e => setForm(f => ({ ...f, dni: e.target.value }))}
                      className="mt-1 w-full rounded-xl border px-3 py-2 focus:outline-none focus:ring-2 focus:ring-neutral-300"
                      placeholder="33539061"
                    />
                  </div>
                  <div>
                    <label className="text-sm text-neutral-600">Nombre</label>
                    <input
                      value={form.nombre}
                      onChange={e => setForm(f => ({ ...f, nombre: e.target.value }))}
                      className="mt-1 w-full rounded-xl border px-3 py-2 focus:outline-none focus:ring-2 focus:ring-neutral-300"
                      placeholder="Roma"
                    />
                  </div>
                  <div>
                    <label className="text-sm text-neutral-600">Email</label>
                    <input
                      type="email"
                      value={form.email}
                      onChange={e => setForm(f => ({ ...f, email: e.target.value }))}
                      className="mt-1 w-full rounded-xl border px-3 py-2 focus:outline-none focus:ring-2 focus:ring-neutral-300"
                      placeholder="roma@example.com"
                    />
                  </div>
                  <div>
                    <label className="text-sm text-neutral-600">Teléfono (opcional)</label>
                    <input
                      value={form.telefono}
                      onChange={e => setForm(f => ({ ...f, telefono: e.target.value }))}
                      className="mt-1 w-full rounded-xl border px-3 py-2 focus:outline-none focus:ring-2 focus:ring-neutral-300"
                      placeholder="+54 261 ..."
                    />
                  </div>
                </div>
              </div>

              <div className="flex items-center justify-end gap-2 p-4 border-t">
                <button onClick={resetAndClose} className="px-4 py-2 rounded-xl border bg-white">Cancelar</button>
                <button
                  onClick={createSocio}
                  disabled={saving}
                  className="px-4 py-2 rounded-xl bg-black text-white hover:opacity-90 disabled:opacity-50 inline-flex items-center gap-2"
                >
                  {saving && <Loader2 className="w-4 h-4 animate-spin" />} Guardar
                </button>
              </div>
            </div>
          </div>
        )}

        {/* Toast */}
        {toast && (
          <div className="fixed bottom-4 right-4 z-50">
            <div className="rounded-xl bg-black text-white text-sm px-4 py-2 shadow-lg">
              {toast}
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
