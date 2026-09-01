import { BrowserRouter, Routes, Route, Link } from "react-router-dom";
import ProductList from '../components/productList';

function Home() {
    return <h1>Home page</h1>;
}


export default function Product_List() {
    return (
        <>
            <BrowserRouter>
                <nav>
                    <Link to="/Product_Add">Add a product</Link>{" "}
                </nav>

                <Routes>
                    <Route path="/Product_Add" element={<Home />} />
                </Routes>
            </BrowserRouter>
            <section className="product-list-container">
                <ProductList />
            </section>
        </>
    );
}