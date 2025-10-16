import React from 'react';
import ReactDOM from 'react-dom/client';
import 'bootstrap/dist/css/bootstrap.min.css';
import App from './App';

// ✅ jQuery global (requerido por Select2)
import $ from "jquery";
;(window as any).$ = $;
;(window as any).jQuery = $;

import "select2/dist/js/select2.full.min.js";
import "select2/dist/css/select2.min.css";



// ✅ Estilos propios
import "./styles/main.css";

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>,
);
