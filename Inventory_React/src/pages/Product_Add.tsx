import { BrowserRouter, Routes, Route, Link } from "react-router-dom";
import ProductForm from '../components/productForm';

function Home() {
    return <h1>Home page</h1>;
}


export default function Product_Add() {
    return (
        <>
            <BrowserRouter>
                <nav>
                    <Link to="/Product_List">Product List</Link>
                </nav>

                <Routes>
                    <Route path="/Product_List" element={<Home />} />
                </Routes>
            </BrowserRouter>
            <ProductForm />
        </>
    );
}