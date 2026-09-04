import './App.css'
import { BrowserRouter, Routes, Route, useNavigate } from "react-router-dom";
import Product_Add from "./pages/Product_Add";
import Product_List from './pages/Product_List';
import Product_Details from './pages/Product_Details';


function Navigation() {
    const navigate = useNavigate();

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
        </nav>
    );
}

function App() {

    return (
        <BrowserRouter>
            <main className="app-container">

                <Navigation />

                <hr />

                <Routes>
                    <Route path="/Product_Add" element={<Product_Add />} />
                    <Route path="/Product_List" element={<Product_List />} />
                    <Route path="/Product_Details/:id" element={<Product_Details />} />
                </Routes>

            </main>
        </BrowserRouter>
    );
}

export default App
