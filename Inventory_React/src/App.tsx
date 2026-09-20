import './App.css'
import { BrowserRouter, Routes, Route, useNavigate } from "react-router-dom";
import { AuthProvider } from "./context/AuthContext";
import { useAuth } from "./context/AuthContext";

import ProtectedRoute from "./components/ProtectedRoute";

import Login from "./pages/Login";
import Product_Add from "./pages/Product_Add";
import Product_List from './pages/Product_List';
import Product_Details from './pages/Product_Details';


function Navigation() {
    const navigate = useNavigate();
    const { token, logout } = useAuth();

    async function handleLogout() {
        const token = localStorage.getItem("access_token");

        try {
            if (token) {
                await fetch(`${import.meta.env.VITE_API_URL}/api/auth/logout`, {
                    method: "POST",
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                });
            }
        } finally {
            localStorage.removeItem("access_token");
            logout();
            navigate("/login");
        }
    }


    return (

        // <nav>
        //     <Link to="/Product_Add">Product_Add</Link>{" "}
        //     <Link to="/Product_List">Product_List</Link>
        // </nav>
        <nav className="navigation">
            <button type="button" onClick={() => navigate("/Product_Add")}>
                Ajouter un produit
            </button>

            <button type="button" onClick={() => navigate("/Product_List")}>
                Liste des produits
            </button>

            {token && (
                <button type="button" onClick={handleLogout}>
                    Logout
                </button>
            )}
        </nav>
    );
}

function App() {

    return (
        <BrowserRouter>
            <AuthProvider>
            <main className="app-container">

                <Navigation />

                <hr />

                <Routes>

                    <Route path="/login" element={<Login />} />
                    <Route path="/" element={<Login />} />

                    <Route element={<ProtectedRoute />}>
                        <Route path="/Product_Add" element={<Product_Add />} />
                        <Route path="/Product_List" element={<Product_List />} />
                        <Route path="/Product_Details/:id" element={<Product_Details />} />
                    </Route>

                </Routes>

                </main>
            </AuthProvider>
        </BrowserRouter>
    );
}

export default App
