import React from "react";
import { createRoot } from "react-dom/client";
import GameClient from "./game-client";
import "./globals.css";

createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <GameClient />
  </React.StrictMode>,
);
