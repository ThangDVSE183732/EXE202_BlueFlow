import "./App.css";
import Homepage from "../pages/Homepage";
import { BrowserRouter, Route, Router, Routes } from "react-router-dom";
import Login from "../pages/Login";
import SignUp from "../pages/SignUp";
import OrganizerPage from "../pages/Organizer/OrganizerPage";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route index element={<OrganizerPage />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
