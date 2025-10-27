interface PaginationProps {
  currentPage: number;
  totalPages: number;
  totalItems?: number; // opcional: para mostrar "Mostrando X–Y de Z"
  pageSize?: number;
  onPageChange: (page: number) => void;
}

export default function Pagination({
  currentPage,
  totalPages,
  totalItems,
  pageSize = 10,
  onPageChange,
}: PaginationProps) {
  if (totalPages <= 1) return null;

  const handleClick = (page: number) => {
    if (page >= 1 && page <= totalPages && page !== currentPage) {
      onPageChange(page);
    }
  };

  // Calcula el rango actual (para "Mostrando 11–20 de 53 resultados")
  const start = (currentPage - 1) * pageSize + 1;
  const end = Math.min(currentPage * pageSize, totalItems || 0);

  return (
    <div className="d-flex flex-column flex-md-row justify-content-between align-items-center mt-3 gap-2">
      {/* 🟠 Info: rango visible */}
      {totalItems !== undefined && (
        <small className="text-muted">
          Mostrando <strong>{start}</strong>–<strong>{end}</strong> de{" "}
          <strong>{totalItems}</strong> resultados
        </small>
      )}

      {/* 🔸 Controles de paginación */}
      <nav>
        <ul className="pagination pagination-sm mb-0">
          <li className={`page-item ${currentPage === 1 ? "disabled" : ""}`}>
            <button
              className="page-link text-dark"
              style={{ borderColor: "#ff6b00" }}
              onClick={() => handleClick(currentPage - 1)}
            >
              «
            </button>
          </li>

          {Array.from({ length: totalPages }).map((_, i) => (
            <li
              key={i}
              className={`page-item ${currentPage === i + 1 ? "active" : ""}`}
            >
              <button
                className="page-link fw-semibold"
                style={{
                  borderColor: "#ff6b00",
                  backgroundColor:
                    currentPage === i + 1 ? "#ff6b00" : "transparent",
                  color: currentPage === i + 1 ? "white" : "#ff6b00",
                }}
                onClick={() => handleClick(i + 1)}
              >
                {i + 1}
              </button>
            </li>
          ))}

          <li
            className={`page-item ${currentPage === totalPages ? "disabled" : ""}`}
          >
            <button
              className="page-link text-dark"
              style={{ borderColor: "#ff6b00" }}
              onClick={() => handleClick(currentPage + 1)}
            >
              »
            </button>
          </li>
        </ul>
      </nav>
    </div>
  );
}
