import { BrowserRouter, Routes, Route, Link } from "react-router-dom";

function Home() {
    return <h1>Home page</h1>;
}

function About() {
    return <h1>About page</h1>;
}

export default function Product_Details() {
    return (
        <BrowserRouter>
            <nav>
                <Link to="/Product_Add">Add a product</Link>{" "}
                <Link to="/Product_List">Product List</Link>
            </nav>

            <Routes>
                <Route path="/Product_Add" element={<Home />} />
                <Route path="/Product_List" element={<About />} />
            </Routes>
        </BrowserRouter>
    );
}